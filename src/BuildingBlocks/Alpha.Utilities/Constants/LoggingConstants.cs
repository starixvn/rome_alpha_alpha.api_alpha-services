namespace Alpha.Utilities.Constants
{
    public static class LoggingConstants
    {
        
        #region 400
        public const string DateIsInvalid = "Input date(s) is invalid";
        public const string ProductIsInvalid = "Product is invalid";
        #endregion

        #region 401
        public const string KeyUsernamePasswordIsNotProvided = "Key, username or password is not provided";
        public const string KeyPassPhraseSignatureTimestampIsNotProvided = "Key, pass phrase, signature or timestamp is not provided";
        public const string KeyUsernamePasswordIsInvalid = "Key, username or password is invalid";
        public const string SearchAccountMustNotBeMember = "Search account role must not be a member";
        public const string SearchAccountMustBeMember = "Search account role must be a member";
        public const string ViewAccountMustNotBeMember = "View account role must not be a member";
        #endregion

        #region 403
        public const string SearchAccountMustNotBeProtectedMode = "Search account status must not be protected mode";
        #endregion

    }
}