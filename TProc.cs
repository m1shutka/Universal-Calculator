namespace UniversalCalculator
{
    internal class TProc
    {
        private TANumber lopRes;/// Левый операнд и результат

        /// Свойство для получения и установки значения левого операнда и результата
        public TANumber LopRes
        {
            get { return lopRes.Copy(); }
            set { lopRes = value.Copy(); }
        }

        private TANumber rop;/// Правый операнд

        /// Свойство для получения и установки значения правого операнда
        public TANumber Rop
        {
            get { return rop.Copy(); }
            set { rop = value.Copy(); }
        }

        public enum TOper { None = 21, Add = 25, Sub = 26, Mul = 27, Dvd = 28 };/// Набор двухоперадовых операций

        /// Свойство для получения и установки текущей операции
        public TOper Oper { get; set; }

        public enum TFunc { Rev = 30, Sqr = 29 };/// Набор однооперандовых функций

        /// Свойство для получения текущей функции
        public TFunc Func { get; set; }

        /// Конструктор
        public TProc(int mode)
        {
            if (mode == 1)
            {
                lopRes = new TPNumber(0, 10, 8);
                rop = new TPNumber(0, 10, 8);
            }
            else if (mode == 2)
            {
                lopRes = new Frac(0, 0);
                rop = new Frac(0, 0);
            }
            else
            {
                lopRes = new Complex(0, 0);
                rop = new Complex(0, 0);
            }
            Oper = TOper.None;
        }

        /// Состояние процессора по умолчанию
        public void ResetTProc(int mode)
        {
            if (mode == 1)
            {
                lopRes = new TPNumber(0, 10, 8);
                rop = new TPNumber(0, 10, 8);
            }
            else if (mode == 2)
            {
                lopRes = new Frac(0, 0);
                rop = new Frac(0, 0);
            }
            else
            {
                lopRes = new Complex(0, 0);
                rop = new Complex(0, 0);
            }
            Oper = TOper.None;
        }

        /// Сброс операции
        public void ResetTOper()
        {
            Oper = TOper.None;
        }

        /// Вычислить двухоперандовую операцию. Результат сохранить в левый операнд
        public void CalcOper()
        {
            switch (Oper)
            {
                case TOper.None:
                    break;
                case TOper.Add:
                    lopRes = lopRes.Add(rop);
                    break;
                case TOper.Sub:
                    lopRes = lopRes.Sub(rop);
                    break;
                case TOper.Mul:
                    lopRes = lopRes.Mul(rop);
                    break;
                case TOper.Dvd:
                    lopRes = lopRes.Div(rop);
                    break;
            }
        }

        /// Вычислить значение функции для правого операнда. Результат сохранить в правый операнд
        public void CalcFunc()
        {
            switch (Func)
            {
                case TFunc.Sqr:
                    rop = rop.Sqr();
                    break;
                case TFunc.Rev:
                    rop = rop.Rev();
                    break;
            }
        }
    }
}
