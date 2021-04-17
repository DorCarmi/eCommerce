
namespace eCommerce.Business.Basics
{
    public class Some<T> : AnswerValue<T>
    {
        private T myValue; 

        public Some(T val)
        {
            myValue = val;
        }

        public T getValue()
        {
            return myValue;
        }

        public bool isNone()
        {
            return false;
        }
    }
}
