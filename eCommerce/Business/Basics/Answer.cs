using System;
using System.Drawing;

namespace eCommerce.Business.Basics
{
    public class Answer<T>
    {
        private bool ans;
        private String reason;
        private AnswerValue<T> returnVal;

        public Answer(String reason)
        {
            this.ans = false;
            this.reason = reason;
            this.returnVal = new None<T>();

        }
        public Answer(T returnVal)
        {
            this.ans = true;
            this.returnVal = new Some<T>(returnVal);
            reason = "";
        }

        public bool isOk()
        {
            return ans;
        }

        public AnswerValue<T> getValue()
        {
            return returnVal;
        }

        public string getReason()
        {
            return reason;
        }
    }
}