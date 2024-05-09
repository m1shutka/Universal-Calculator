namespace UniversalCalculator
{
    internal class TMemory
    {
        TANumber fNumber;//поле числа

        public enum FState { On, Off };//состояния памяти
        public FState St { get; set; }//сво-во для получения/задания состяния помяти

        //конструктор
        public TMemory(int mode)
        {
            if (mode == 1)
            {
                fNumber = new TPNumber(0, 10, 8);
            }
            else if (mode == 2)
            {
                fNumber = new Frac(0, 0);
            }
            else
            {
                fNumber = new Complex(0, 0);
            }
            St = FState.Off;
        }

        //сво-во для получения/задания числа
        public TANumber FNumber
        {
            set
            {
                fNumber = value.Copy();
                St = FState.On;
            }
            get
            {
                St = FState.On;
                return fNumber.Copy();
            }
        }

        //сложение переданнгого числа и числа находящегося в памяти
        public void AddTANumber(TANumber number)
        {
            if (St == FState.Off)
            {
                FNumber = number;
                St = FState.On;
            }
            else
            {
                FNumber = fNumber.Add(number);
            }
        }

        // Очистка памяти
        public void Clear(int mode)
        {
            if (mode == 1)
            {
                fNumber = new TPNumber(0, 10, 8);
            }
            else if (mode == 2)
            {
                fNumber = new Frac(0, 0);
            }
            else
            {
                fNumber = new Complex(0, 0);
            }
            St = FState.Off;
        }
    }
}
