using System;
using static UniversalCalculator.TCtrl;

namespace UniversalCalculator
{
    internal class TCtrl
    {
        /// Набор состояний калькулятора
        public enum TCtrlState { cStart, cEditing, cExpDone, cOpDone, cValDone, cOpChange, cFuncDone, cError }

        /// Свойство для получения и установки значений состояний калькулятора
        public TCtrlState CtrlSt { get; set; }

        public enum TCtrlMode { pNum, fNum, cNum }

        public TCtrlMode CtrlMd { get; set; }

        /// Объект класса Редактор
        private AEditor editor;

        /// Объект класса Процессор
        private TProc proc;

        /// Объект класса Память
        private TMemory memory;

        /// Объект класса p-ичное число
        internal TANumber pNumber;

        /// Максимальная длина целой части числа
        private const int MAXINTLEN = 12;

        /// Максимальная длина дробной части числа
        private const int MAXFRACTLEN = 8;

        private bool determ = false;

        /// Конструктор
        public TCtrl()
        {
            CtrlSt = TCtrlState.cStart;
            CtrlMd = TCtrlMode.pNum;
            editor = new TEditor();
            proc = new TProc(1);
            memory = new TMemory(1);
            pNumber = new TPNumber(0, 10, 8);
        }


        /// Выполнить команду калькулятора
        public void DoCommandCalculator(int command)
        {
            string editorResult;
            TANumber procResult;

            // Обработка ввода и редактирования числа
            if (command <= 19)
            {
                if (command == 19) determ = false;

                editorResult = DoCommandEditor(command);
                if (command == 18 && CtrlMd == TCtrlMode.fNum && editorResult.IndexOf('/') == -1 && pNumber.StrNumber.IndexOf('/') != -1) determ = false;
                if (command == 18 && CtrlMd == TCtrlMode.cNum && editorResult.IndexOf('i') == -1 && pNumber.StrNumber.IndexOf('i') != -1) determ = false;

                if (CheckLenNumber(editorResult))
                {
                    throw new Exception("Превышена максимально возможная длина числа");
                }
                else
                {
                    CtrlSt = TCtrlState.cValDone;
                    pNumber.StrNumber = editorResult;
                }
            }
            else if (command == 28 && CtrlMd == TCtrlMode.fNum && determ == false)
            {
                editorResult = DoCommandEditor(55);
                CtrlSt = TCtrlState.cValDone;
                pNumber.StrNumber = editorResult;
                determ = true;
            }
            else if (command == 32 && CtrlMd == TCtrlMode.cNum && determ == false)
            {
                editorResult = DoCommandEditor(55);
                CtrlSt = TCtrlState.cValDone;
                pNumber.StrNumber = editorResult;
                determ = true;
            }
            // Обработка команды для кнопки C
            else if (command == 20)
            {
                ResetTCtrl();
            }
            // Обработка взаимодействий с памятью
            else if (command >= 21 && command <= 24)
            {
                DoCommandMemory(command);
                if (CheckLenNumber(pNumber.StrNumber))
                {
                    throw new Exception("Превышена максимально возможная длина числа");
                }
            }
            // Обратобка ввода операций
            else if (command >= 25 && command <= 28)
            {   
                procResult = DoProcOper(command);
                if (CtrlSt == TCtrlState.cError)
                {
                    throw new Exception("Деление на ноль невозможно");
                }
                else if (CheckLenNumber(procResult.StrNumber))
                {
                    throw new Exception("Превышена максимально возможная длина числа");
                }
                //else if (isZero(procResult))
                //{
                //throw new Exception("Слишком маленькое число");
                //}
                else
                {
                    pNumber = procResult;
                }
                determ = false;
            }
            // Обратобка ввода функций
            else if (command == 29 || command == 30)
            {
                procResult = DoProcFunc(command);
                if (CtrlSt == TCtrlState.cError)
                {
                    throw new Exception("Деление на ноль невозможно");
                }
                else if (CheckLenNumber(procResult.StrNumber))
                {
                    throw new Exception("Превышена максимально возможная длина числа");
                }
                else if (procResult.isZero())
                {
                    throw new Exception("Слишком маленькое число");
                }
                else
                {
                    CtrlSt = TCtrlState.cFuncDone;
                    pNumber = procResult;
                }
            }
            // Обработка вычисления всего выражения
            else if (command == 31)
            {
                procResult = CalcExpr();
                if (CtrlSt == TCtrlState.cError)
                {
                    throw new Exception("Деление на ноль невозможно");
                }
                else if (CheckLenNumber(procResult.StrNumber))
                {
                    throw new Exception("Превышена максимально возможная длина числа");
                }
                //else if (isZero(procResult))
                //{
                //throw new Exception("Слишком маленькое число");
                //}
                else
                {
                    CtrlSt = TCtrlState.cExpDone;
                    pNumber = procResult;
                }
                determ = false;
            }
        }


        /// Выполнить команду редактора
        private string DoCommandEditor(int command)
        {
            // Уставнавливаем состояние калькулятора на редактирование числа
            CtrlSt = TCtrlState.cEditing;
            string result = editor.DoEdit(command);
            return result;
        }


        /// Выполнение команды процессора
        private TANumber DoProcOper(int operation)
        {
            TANumber result;
            // Если на предыдущем шаге не определяли операцию
            if (CtrlSt != TCtrlState.cOpChange)
            {
                // Если операция не задана или на предыдушем шаге было вычислено выражение 
                if (proc.Oper == TProc.TOper.None || CtrlSt == TCtrlState.cExpDone)
                {
                    proc.LopRes = pNumber;
                    DoCommandEditor(20);
                    proc.Oper = (TProc.TOper)operation;
                    CtrlSt = TCtrlState.cOpChange;
                }
                else
                {
                    // Если для правого ввели значение или на предыдущем шаге была вычислена функция
                    if (CtrlSt == TCtrlState.cValDone || CtrlSt == TCtrlState.cFuncDone)
                    {
                        proc.Rop = pNumber;
                        DoCommandEditor(20);

                        if (CheckDivZero(proc.Rop))
                        {
                            if (CtrlMd == TCtrlMode.pNum) return new TPNumber(0, 10, 8);
                            else if (CtrlMd == TCtrlMode.fNum) return new Frac(0, 0);
                            else return new Complex(0, 0);
                        }

                        proc.CalcOper();

                        proc.Oper = (TProc.TOper)operation;
                        CtrlSt = TCtrlState.cOpChange;
                    }
                }
            }
            else
            {
                proc.Oper = (TProc.TOper)operation;
                CtrlSt = TCtrlState.cOpChange;
            }

            result = proc.LopRes;
            return result;
        }


        // Переменная для хранения в каком операнде хранится результат функции
        static string operResFunc = "";


        /// Вычисление функции процессора
        private TANumber DoProcFunc(int func)
        {
            TANumber defaultValue;
            TANumber result;
            if (CtrlMd == TCtrlMode.pNum)
            {
                defaultValue = new TPNumber(0, 10, 8);
                result = new TPNumber(0, 10, 8);
            }
            else if (CtrlMd == TCtrlMode.fNum)
            {
                defaultValue = new Frac(0, 0);
                result = new Frac(0, 0);
            }
            else 
            {
                defaultValue = new Complex(0, 0);
                result = new Complex(0, 0);
            }
            proc.Func = (TProc.TFunc)func;

            // Если в левом операнде установлено значение по умолчанию
            if (proc.LopRes.Equals(defaultValue))
            {
                proc.LopRes = pNumber;

                if (CheckDivZero(proc.LopRes))
                {
                    if (CtrlMd == TCtrlMode.pNum) return new TPNumber(0, 10, 8);
                    else if (CtrlMd == TCtrlMode.fNum) return new Frac(0, 0);
                    else return new Complex(0, 0);
                }

                swapOperands();
                proc.CalcFunc();
                swapOperands();
                operResFunc = "LopRes";
                result = proc.LopRes;
            }
            else
            {
                // Если в левом и в правом операнде установлено некоторое значение
                if (!proc.Rop.Equals(defaultValue))
                {
                    // Если ввели новое значение для правого операнда
                    if (CtrlSt == TCtrlState.cValDone)
                    {
                        proc.Rop = pNumber;
                        DoCommandEditor(20);

                        if (CheckDivZero(proc.Rop))
                        {
                            if (CtrlMd == TCtrlMode.pNum) return new TPNumber(0, 10, 8);
                            else if (CtrlMd == TCtrlMode.fNum) return new Frac(0, 0);
                            else return new Complex(0, 0);
                        }

                        proc.CalcFunc();
                        operResFunc = "Rop";
                        result = proc.Rop;
                    }

                    // Если было вычислено выражение, и в левом операнде хранится результат
                    if (CtrlSt == TCtrlState.cExpDone)
                    {
                        if (CheckDivZero(proc.LopRes))
                        {
                            if (CtrlMd == TCtrlMode.pNum) return new TPNumber(0, 10, 8);
                            else if (CtrlMd == TCtrlMode.fNum) return new Frac(0, 0);
                            else return new Complex(0, 0);
                        }

                        swapOperands();
                        proc.CalcFunc();
                        swapOperands();

                        operResFunc = "LopRes";
                        result = proc.LopRes;
                    }

                    // Если было вычислена функция, и в левом / правом операнде хранится результат
                    if (CtrlSt == TCtrlState.cFuncDone)
                    {
                        // Если результат вычисления функции сохранен в левом операнде
                        if (operResFunc == "LopRes")
                        {
                            if (CheckDivZero(proc.LopRes))
                            {
                                if (CtrlMd == TCtrlMode.pNum) return new TPNumber(0, 10, 8);
                                else if (CtrlMd == TCtrlMode.fNum) return new Frac(0, 0);
                                else return new Complex(0, 0);
                            }

                            swapOperands();
                            proc.CalcFunc();
                            swapOperands();

                            operResFunc = "LopRes";
                            result = proc.LopRes;
                        }
                        // Если результат вычисления функции сохранен в правом операнде
                        else
                        {
                            if (CheckDivZero(proc.LopRes))
                            {
                                if (CtrlMd == TCtrlMode.pNum) return new TPNumber(0, 10, 8);
                                else if (CtrlMd == TCtrlMode.fNum) return new Frac(0, 0);
                                else return new Complex(0, 0);
                            }

                            proc.CalcFunc();
                            operResFunc = "Rop";
                            result = proc.Rop;
                        }
                    }
                }
                // Если в левом операнде установлено некоторое значение, а в правом - значение по умолчанию
                else
                {
                    // Если ввели значение для правого операнда
                    if (CtrlSt == TCtrlState.cValDone)
                    {
                        proc.Rop = pNumber;
                        DoCommandEditor(20);

                        if (CheckDivZero(proc.Rop))
                        {
                            if (CtrlMd == TCtrlMode.pNum) return new TPNumber(0, 10, 8);
                            else if (CtrlMd == TCtrlMode.fNum) return new Frac(0, 0);
                            else return new Complex(0, 0);
                        }

                        proc.CalcFunc();
                        operResFunc = "Rop";
                        result = proc.Rop;
                    }
                    else
                    {
                        // Если а в правом установлено значение по умолчанию
                        if (CheckDivZero(proc.LopRes))
                        {
                            if (CtrlMd == TCtrlMode.pNum) return new TPNumber(0, 10, 8);
                            else if (CtrlMd == TCtrlMode.fNum) return new Frac(0, 0);
                            else return new Complex(0, 0);
                        }

                        swapOperands();
                        proc.CalcFunc();
                        swapOperands();

                        operResFunc = "LopRes";
                        result = proc.LopRes;
                    }
                }
            }
            proc.Oper = TProc.TOper.None;
            return result;
        }


        /// Вычисление выражение
        private TANumber CalcExpr()
        {
            // Если выражение не было вычисленно ранее
            if (CtrlSt != TCtrlState.cExpDone)
            {
                // Если ввели значение для правого операнда
                if (CtrlSt == TCtrlState.cValDone)
                {
                    proc.Rop = pNumber;
                }
                // Если хотим сложить результат с самим собой
                else if (CtrlSt == TCtrlState.cOpChange)
                {
                    proc.Rop = proc.LopRes;
                }

                // Проверка деления на ноль
                if (CtrlSt != TCtrlState.cFuncDone && CheckDivZero(proc.Rop))
                {
                    if (CtrlMd == TCtrlMode.pNum) return new TPNumber(0, 10, 8);
                    else if (CtrlMd == TCtrlMode.fNum) return new Frac(0, 0);
                    else return new Complex(0, 0);
                }
                proc.CalcOper();
                return proc.LopRes;
            }
            else
            {
                // Повторное выполнение последней операции
                proc.CalcOper();
                return proc.LopRes;
            }
        }


        /// Поменять операнды местами
        private void swapOperands()
        {
            TANumber tempPNumber = proc.LopRes;
            proc.LopRes = proc.Rop;
            proc.Rop = tempPNumber;
        }

        /// Проверка деления на ноль
        private bool CheckDivZero(TANumber number)
        {
            if (proc.Func == TProc.TFunc.Rev && number.isZero() ||
                proc.Oper == TProc.TOper.Dvd && number.isZero())
            {
                CtrlSt = TCtrlState.cError;
                return true;
            }

            return false;
        }


        /// Получить точность числа
        public int GetAccuracy(string number)
        {
            int indexDelim = number.IndexOf(',');

            if (indexDelim == -1)
            {
                return 0;
            }

            return number.Length - indexDelim - 1;
        }


        /// Проверка длины числа
        private bool CheckLenNumber(string number)
        {
            int fractLen = GetAccuracy(number);
            int intLen = number.Length - fractLen - 1;
            if (intLen > MAXINTLEN || fractLen > MAXFRACTLEN)
            {
                CtrlSt = TCtrlState.cError;
                return true;
            }

            return false;
        }


        /// Выполнить команду памяти
        private void DoCommandMemory(int command)
        {
            switch (command)
            {
                case 21:
                    if (CtrlMd == TCtrlMode.pNum) memory.Clear(1);
                    else if (CtrlMd == TCtrlMode.fNum) memory.Clear(2);
                    else memory.Clear(3);
                    break;
                case 22:
                    pNumber = memory.FNumber;
                    CtrlSt = TCtrlState.cValDone;
                    break;
                case 23:
                    memory.FNumber = pNumber;
                    break;
                case 24:
                    memory.AddTANumber(pNumber);
                    break;
            }
        }


        /// Получить состояние пямяти
        public string GetMemoryState()
        {
            return memory.St.ToString();
        }


        /// Сбросить введенные параметры
        private void ResetTCtrl()
        {
            DoCommandEditor(20);
            CtrlSt = TCtrlState.cStart;
            if (CtrlMd == TCtrlMode.pNum)
            {
                pNumber = new TPNumber(0, 10, 8);
                editor = new TEditor();
                proc.ResetTProc(1);
            }
            else if (CtrlMd == TCtrlMode.fNum)
            {
                pNumber = new Frac(0, 0);
                editor = new FEditor();
                proc.ResetTProc(2);
            }
            else
            {
                pNumber = new Complex(0, 0);
                editor = new CEditor();
                proc.ResetTProc(3);
            }
            determ = false;
        }

        public void ChangeTCtrlMode(int mode)
        {
            if (mode == 1)
            {
                CtrlMd = TCtrlMode.pNum;
                pNumber = new TPNumber(0, 10, 8);
                editor = new TEditor();
                proc.ResetTProc(1);
                memory.Clear(1);
            }
            else if (mode == 2)
            {
                CtrlMd = TCtrlMode.fNum;
                pNumber = new Frac(0, 0);
                editor = new FEditor();
                proc.ResetTProc(2);
                memory.Clear(2);
            }
            else
            {
                CtrlMd = TCtrlMode.cNum;
                pNumber = new Complex(0, 0);
                editor = new CEditor();
                proc.ResetTProc(3);
                memory.Clear(3);
            }
        }
    }
}
