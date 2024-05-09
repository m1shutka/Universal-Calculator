using System;

namespace UniversalCalculator
{
    abstract class TANumber//абстрактный класс число
    {
        public abstract TANumber Add(TANumber n);
        public abstract TANumber Mul(TANumber n);
        public abstract TANumber Sub(TANumber n);
        public abstract TANumber Div(TANumber n);
        public abstract TANumber Sqr();
        public abstract TANumber Rev();
        public abstract TANumber Copy();
        public abstract bool Equals(TANumber n);
        public abstract override string ToString();
        public abstract string StrNumber { get; set; }
        public abstract int IntP { get; set;}
        public abstract bool isZero();
    }

    class TPNumber : TANumber//класс p-ичное число
    {
        private double number;//поле число
        private int p;//основание сс
        private int accuracy;//точность задания числа

        //конструктор для числа в десятичной сс
        public TPNumber(double n, int p, int accuracy)
        {
            number = n;
            this.p = p;
            this.accuracy = accuracy;
        }

        //конструктор для числа заданного в виде строки
        public TPNumber(string value, int p, int accuracy)
        {
            number = Convert_p_10.Do(value, p);
            this.p = p;
            this.accuracy = accuracy;
        }

        //св-во для получения числа в строковом виде
        public override string StrNumber
        {
            get { return Convert_10_p.Do(number, p, accuracy); }
            set { number = Convert_p_10.Do(value, p); }
        }

        //св-во для получения числа в десятичном виде
        public double DecimalNumber
        {
            get { return number; }
            set { number = value; }
        }

        //св-во для получения/задания основания в строковом виде
        public string StrP
        {
            get { return p.ToString(); }
            set { p = Convert.ToInt32(value); }
        }

        //св-во для получения/задания основания в числовом виде
        public override int IntP
        {
            get { return p; }
            set { p = value; }
        }

        //св-во для получения/задания точности в числовом виде
        public int IntAcc
        {
            get { return accuracy; }
            set { accuracy = value; }
        }

        //св-во для получения/задания точности в строковом виде
        public string StrAcc
        {
            get { return accuracy.ToString(); }
            set { accuracy = Convert.ToInt32(value); }
        }

        //операция сложения
        public override TANumber Add(TANumber n)
        {
            return new TPNumber(number + (n as TPNumber).DecimalNumber, p, Math.Max(accuracy, (n as TPNumber).IntAcc));
        }

        //операция произведения
        public override TANumber Mul(TANumber n)
        {
            return new TPNumber(number * (n as TPNumber).DecimalNumber, p, Math.Max(accuracy, (n as TPNumber).IntAcc));
        }

        //операция вычитания
        public override TANumber Sub(TANumber n)
        {
            return new TPNumber(number - (n as TPNumber).DecimalNumber, p, Math.Max(accuracy, (n as TPNumber).IntAcc));
        }

        //операция деления
        public override TANumber Div(TANumber n)
        {
            return new TPNumber(number / (n as TPNumber).DecimalNumber, p, Math.Max(accuracy, (n as TPNumber).IntAcc));
        }

        //операция сравнения
        public override bool Equals(TANumber n)
        {
            if (number == (n as TPNumber).DecimalNumber && accuracy == (n as TPNumber).IntAcc)
            {
                return true;
            }
            return false;
        }

        //возведение в квадрат
        public override TANumber Sqr()
        {
            return new TPNumber(number * number, p, accuracy);
        }

        //обращение числа
        public override TANumber Rev()
        {
            return new TPNumber(1 / number, p, accuracy);
        }

        //перобразование числа в строку
        public override string ToString()
        {
            return Convert_10_p.Do(number, p, accuracy);
        }

        //создание копии
        public override TANumber Copy()
        {
            return new TPNumber(number, p, accuracy);
        }

        public override bool isZero()
        {
            return number == 0;
        }
    }

    class Frac: TANumber//класс дробь
    {
        private TPNumber num;
        private TPNumber dnom;

        public Frac(double n, double dn)
        {
            num = new TPNumber(n, 10, 8);
            dnom = new TPNumber(dn, 10, 8);
        }

        public override string StrNumber
        {
            get { return ToString(); }
            set { FromString(value); }
        }

        public override int IntP
        {
            get { return 10; }
            set { }
        }

        public override TANumber Add(TANumber n)
        {
            return new Frac(num.DecimalNumber * (n as Frac).dnom.DecimalNumber + dnom.DecimalNumber * (n as Frac).num.DecimalNumber, dnom.DecimalNumber * (n as Frac).dnom.DecimalNumber);
        }

        public override TANumber Mul(TANumber n)
        {
            return new Frac(num.DecimalNumber * (n as Frac).num.DecimalNumber, dnom.DecimalNumber * (n as Frac).dnom.DecimalNumber);
        }

        public override TANumber Sub(TANumber n)
        {
            return new Frac(num.DecimalNumber * (n as Frac).dnom.DecimalNumber - dnom.DecimalNumber * (n as Frac).num.DecimalNumber, dnom.DecimalNumber * (n as Frac).dnom.DecimalNumber);
        }

        public override TANumber Div(TANumber n)
        {
            return new Frac(num.DecimalNumber * (n as Frac).dnom.DecimalNumber, dnom.DecimalNumber * (n as Frac).num.DecimalNumber);
        }

        public override bool Equals(TANumber n)
        {
            if (num.Equals((n as Frac).num))
            {
                return true;
            }
            return false;
        }

        public override TANumber Sqr()
        {
            return new Frac(num.DecimalNumber * num.DecimalNumber, dnom.DecimalNumber * dnom.DecimalNumber);
        }

        public override TANumber Rev()
        {
            return new Frac(dnom.DecimalNumber, num.DecimalNumber);
        }

        public override string ToString()
        {
            return num + "/" + dnom;
        }

        public override TANumber Copy()
        {
            return new Frac(num.DecimalNumber, dnom.DecimalNumber);
        }

        private void FromString(string s)
        {
            if (s.IndexOf("/") != -1 && s.IndexOf("/") < s.Length - 1)
            {
                num.StrNumber = s.Substring(0, s.IndexOf("/"));
                dnom.StrNumber = s.Substring(s.IndexOf("/") + 1);
            }
            else if (s.IndexOf("/") == s.Length - 1)
            {
                num.StrNumber = s.Substring(0, s.IndexOf("/"));
                dnom.StrNumber = "0";
            }
            else
            {
                num.StrNumber = s;
                dnom.StrNumber = "0";
            }
        }

        public override bool isZero()
        {
            return num.DecimalNumber == 0 && dnom.DecimalNumber == 0;
        }
    }

    class Complex : TANumber//класс комплексное число
    {
        private TPNumber Re;
        private TPNumber Im;

        public Complex(double re, double im)
        {
            Re = new TPNumber(re, 10, 8);
            Im = new TPNumber(im, 10, 8);
        }

        public override string StrNumber
        {
            get { return ToString(); }
            set { FromString(value); }
        }

        public override int IntP
        {
            get { return 10; }
            set { }
        }

        public override TANumber Add(TANumber n)
        {
            return new Complex(Re.DecimalNumber + (n as Complex).Re.DecimalNumber, Im.DecimalNumber + (n as Complex).Im.DecimalNumber);
        }

        public override TANumber Mul(TANumber n)
        {
            return new Complex(Re.DecimalNumber * (n as Complex).Re.DecimalNumber - Im.DecimalNumber * (n as Complex).Im.DecimalNumber, 
                               Re.DecimalNumber * (n as Complex).Im.DecimalNumber + Im.DecimalNumber * (n as Complex).Re.DecimalNumber);
        }

        public override TANumber Sub(TANumber n)
        {
            return new Complex(Re.DecimalNumber - (n as Complex).Re.DecimalNumber, Im.DecimalNumber - (n as Complex).Im.DecimalNumber);
        }

        public override TANumber Div(TANumber n)
        {
            return new Complex((Re.DecimalNumber * (n as Complex).Re.DecimalNumber + Im.DecimalNumber * (n as Complex).Im.DecimalNumber)/
                               (Math.Pow((n as Complex).Im.DecimalNumber, 2) + Math.Pow((n as Complex).Re.DecimalNumber, 2)), 
                               (Im.DecimalNumber * (n as Complex).Re.DecimalNumber - Re.DecimalNumber * (n as Complex).Im.DecimalNumber) /
                               (Math.Pow((n as Complex).Im.DecimalNumber, 2) + Math.Pow((n as Complex).Re.DecimalNumber, 2)));
        }

        public override bool Equals(TANumber n)
        {
            if (Re.Equals((n as Complex).Re) && Im.Equals((n as Complex).Im))
            {
                return true;
            }
            return false;
        }

        public override TANumber Sqr()
        {
            return new Complex(Re.DecimalNumber * Re.DecimalNumber - Im.DecimalNumber * Im.DecimalNumber, Re.DecimalNumber * Im.DecimalNumber + Re.DecimalNumber * Im.DecimalNumber);
        }

        public override TANumber Rev()
        {
            return new Complex(Re.DecimalNumber/(Math.Pow(Re.DecimalNumber, 2) + Math.Pow(Im.DecimalNumber, 2)), - Im.DecimalNumber / (Math.Pow(Re.DecimalNumber, 2) + Math.Pow(Im.DecimalNumber, 2)));
        }

        public override string ToString()
        {
            return Re + "i" + Im;
        }

        public override TANumber Copy()
        {
            return new Complex(Re.DecimalNumber, Im.DecimalNumber);
        }

        public override bool isZero()
        {
            return Re.DecimalNumber == 0 && Im.DecimalNumber == 0;
        }

        private void FromString(string s)
        {
            if (s.IndexOf("i") != -1 && s.IndexOf("i") < s.Length - 1)
            {
                Re.StrNumber = s.Substring(0, s.IndexOf("i"));
                Im.StrNumber = s.Substring(s.IndexOf("i") + 1);
            }
            else if (s.IndexOf("i") == s.Length - 1)
            {
                Re.StrNumber = s.Substring(0, s.IndexOf("i"));
                Im.StrNumber = "0";
            }
            else
            {
                Re.StrNumber = s;
                Im.StrNumber = "0";
            }
        }
    }
}