namespace eCommerce.Business
{
    public class Admin : Member
    {
        private static readonly Admin state = new Admin();  
        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit  
        static Admin(){}  
        private Admin(){}  
        public static Admin State  
        {  
            get  
            {  
                return state;  
            }  
        }
    }
}