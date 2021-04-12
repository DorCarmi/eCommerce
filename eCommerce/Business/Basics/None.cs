namespace eCommerce.Business.Basics
{
    public class None<T>: AnswerValue<T>
    {
        public None()
        {
            
        }

        public bool isNone()
        {
            return true;
        }
    }
}