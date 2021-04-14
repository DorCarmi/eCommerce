namespace eCommerce.Business
{
    public class MemberData
    {
        public string Username { get => MemberInfo.Username; }
        public MemberInfo MemberInfo;

        public MemberData(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }
    }
}