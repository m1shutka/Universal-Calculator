namespace UniversalCalculator
{
    abstract class AEditor// абстрактный класс редактор
    {
        public abstract string DoEdit(int operation);
    }

    class TEditor : AEditor//редактор p-ичных чисел
    {
        private string number = "0";//поле числа
        private const string delim = ",";//разделитель
        private const string zero = "0";//ноль
        private bool negative = false;//отрицательность числа
        private bool real = false;//вещественное число

        //сво-во получения/задания числа 
        public string Number
        {
            get { return number; }
            set { number = value; }
        }

        //добавление символа
        private void AddDigit(int n)
        {
            string symbols = "0123456789ABCDEF";

            if (number == zero)
            {
                number = symbols[n].ToString();
            }
            else
            {
                number += symbols[n];
            }
        }

        //добваление нуля
        private void AddZero()
        {
            if (number != zero)
            {
                number += zero;
            }
        }

        //добавление раззделителя
        private void AddDelim()
        {
            if (!real)
            {
                real = true;
                number += delim;
            }
        }

        //изменение знака
        private void ChangeSign()
        {
            if (number == zero)
            {
                return;
            }
            else if (negative)
            {
                negative = false;
                number = number.Remove(0, 1);
            }
            else
            {
                negative = true;
                number = "-" + number;
            }
        }

        //удаление правого символа
        private void BackSpace()
        {
            if (number == zero)
            {
                return;
            }
            else
            {
                switch (number[number.Length - 1])
                {
                    case '-':
                        negative = false;
                        break;
                    case ',':
                        real = false;
                        break;
                    default:
                        break;
                }

                if (number.Length > 1)
                {
                    number = number.Remove(number.Length - 1, 1);
                }
                else
                {
                    number = zero;
                }
            }
        }

        //Очистка поля числа и обнуление парматеров
        private void Clear()
        {
            negative = false;
            real = false;
            number = zero;
        }

        //выполнение команды с кодом operation
        public override string DoEdit(int operation)
        {
            if (operation == 0)
            {
                AddZero();
            }
            else if (operation >= 1 && operation <= 15)
            {
                AddDigit(operation);
            }
            else if (operation == 16)
            {
                ChangeSign();
            }
            else if (operation == 17)
            {
                AddDelim();
            }
            else if (operation == 18)
            {
                BackSpace();
            }
            else if (operation == 19 || operation == 20)
            {
                Clear();
            }

            return number;
        }
    }

    class FEditor: AEditor//класс редактор дроби
    {
        private TEditor numEditor = new TEditor();
        private TEditor dnomEditor = new TEditor();
        private bool isFrac = false;

        public override string DoEdit(int operation)
        {
            if (operation == 20 || operation == 19)
            {
                dnomEditor.DoEdit(operation);
                numEditor.DoEdit(operation);
                isFrac = false;
            }
            else
            {
                if (isFrac == false)
                {
                    if (operation != 55) numEditor.DoEdit(operation);
                    else isFrac = true;
                }
                else
                {
                    if (operation != 55) dnomEditor.DoEdit(operation);
                    else isFrac = false;
                }
            }

            if (dnomEditor.Number == "0" && !isFrac) return numEditor.Number;
            else if(dnomEditor.Number == "0" && isFrac) return numEditor.Number + "/";
            return numEditor.Number + "/" + dnomEditor.Number;
        }
    }

    class CEditor : AEditor// класс редактор клмпдексного числа
    {
        private TEditor reEditor = new TEditor();
        private TEditor imEditor = new TEditor();
        private bool isComplex = false;

        public override string DoEdit(int operation)
        {
            if (operation == 20 || operation == 19)
            {
                reEditor.DoEdit(operation);
                imEditor.DoEdit(operation);
                isComplex = false;
            }
            else
            {
                if (isComplex == false)
                {
                    if (operation != 55) reEditor.DoEdit(operation);
                    else isComplex = true;
                }
                else
                {
                    if (operation != 55) imEditor.DoEdit(operation);
                    else isComplex = false;
                }
            }

            if (imEditor.Number == "0" && !isComplex) return reEditor.Number;
            else if (imEditor.Number == "0" && isComplex) return reEditor.Number + "i";
            return reEditor.Number + "i" + imEditor.Number;
        }
    }
}