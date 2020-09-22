using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Text.RegularExpressions;
using System.Web.Security;
using BusiBlocks.PersonLayer;
using BusiBlocks.Roles;

namespace BusiBlocks.Membership
{
    /// <summary>
    /// A implementation of a System.Web.Security.MembershipProvider class that use the BusiBlocks classes.
    /// See MSDN System.Web.Security.MembershipProvider documentation for more informations about MembershipProvider.
    /// 
    /// For now implements only the Hashed password format.
    /// 
    /// You must use the BusiBlocksMembershipProvider with the BusiBlocksRoleProvider.
    /// 
    /// Some methods don't returns an exception but a true or false value. In this case the exception is logged used the log4net configuration (see Log class).
    /// 
    /// For more implementation details look at: "Implementing a Membership Provider" http://msdn2.microsoft.com/en-us/library/f1kyba5e.aspx
    /// 
    /// Note that the name and email field are used as case insensitive, the pasword is case sensitive.
    /// </summary>
    public class BusiBlocksMembershipProvider : System.Web.Security.MembershipProvider
    {
        private ConnectionParameters _mConfiguration;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "BusiBlocksMembershipProvider";

            base.Initialize(name, config);

            mProviderName = name;
            _mApplicationName = ExtractConfigValue(config, "applicationName", ConnectionParameters.DefaultApp);
                //System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath
            _mEnablePasswordReset = bool.Parse(ExtractConfigValue(config, "enablePasswordReset", "true"));
            mEnablePasswordRetrieval = false; //bool.Parse(GetConfigValue(config["enablePasswordRetrieval"], "false"));
            mMaxInvalidPasswordAttempts = int.Parse(ExtractConfigValue(config, "maxInvalidPasswordAttempts", "5"));
            mMinRequiredNonAlphanumericCharacters =
                int.Parse(ExtractConfigValue(config, "minRequiredNonAlphanumericCharacters", "1"));
            mMinRequiredPasswordLength = int.Parse(ExtractConfigValue(config, "minRequiredPasswordLength", "7"));
            mPasswordAttemptWindow = int.Parse(ExtractConfigValue(config, "passwordAttemptWindow", "10"));
            mPasswordFormat = MembershipPasswordFormat.Hashed;
                //Enum.Parse(typeof(MembershipPasswordFormat), GetConfigValue(config["passwordFormat"], "Hashed"));
            mPasswordStrengthRegularExpression = ExtractConfigValue(config, "passwordStrengthRegularExpression", "");
            mRequiresQuestionAndAnswer = bool.Parse(ExtractConfigValue(config, "requiresQuestionAndAnswer", "false"));
            mRequiresUniqueEmail = bool.Parse(ExtractConfigValue(config, "requiresUniqueEmail", "true"));

            string connName = ExtractConfigValue(config, "connectionStringName", null);
            _mConfiguration = ConnectionParameters.Create(connName);

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException("Unrecognized attribute: " +
                                                attr);
            }
        }

        /// <summary>
        /// A helper function to retrieve config values from the configuration file and remove the entry.
        /// </summary>
        /// <returns></returns>
        private string ExtractConfigValue(NameValueCollection config, string key, string defaultValue)
        {
            string val = config[key];
            if (val == null)
                return defaultValue;

            config.Remove(key);
            return val;
        }

        #region Properties

        private string _mApplicationName;
        private bool _mEnablePasswordReset;
        private bool mEnablePasswordRetrieval;
        private int mMaxInvalidPasswordAttempts;
        private int mMinRequiredNonAlphanumericCharacters;
        private int mMinRequiredPasswordLength;
        private int mPasswordAttemptWindow;
        private MembershipPasswordFormat mPasswordFormat;
        private string mPasswordStrengthRegularExpression;
        private string mProviderName;
        private bool mRequiresQuestionAndAnswer;
        private bool mRequiresUniqueEmail;

        public string ProviderName
        {
            get { return mProviderName; }
            set { mProviderName = value; }
        }

        public override string ApplicationName
        {
            get { return _mApplicationName; }
            set { _mApplicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return _mEnablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return mEnablePasswordRetrieval; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return mMaxInvalidPasswordAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return mMinRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return mMinRequiredPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            get { return mPasswordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return mPasswordFormat; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return mPasswordStrengthRegularExpression; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return mRequiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return mRequiresUniqueEmail; }
        }

        #endregion

        #region Methods

        private MembershipUser UserToMembershipUser(User user)
        {
            return new MembershipUser(
                ProviderName, user.Name, user.Id, user.Email, user.PasswordQuestion, user.Comment,
                user.Enabled, user.IsLockedOut, SafeDate(user.InsertDate), SafeDate(user.LastLogOnDate),
                SafeDate(user.LastActivityDate), SafeDate(user.LastPasswordChangedDate), SafeDate(user.LockedOutDate)
                );
        }

        private DateTime SafeDate(DateTime? date)
        {
            if (date != null)
                return date.Value;
            return new DateTime();
        }

        private void LogException(Exception exception, string action)
        {
            Log.Error(GetType(), "Exception on " + action, exception);
        }

        /// <summary>
        /// Check if the password support the required strength.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="isNew"></param>
        private void ValidatePassword(string user, string password, bool isNew)
        {
            //Check if empty
            if (password == null)
                throw new MembershipPasswordException("Password validation failed");

            //Check minimum length
            if (password.Length < MinRequiredPasswordLength)
                throw new MembershipPasswordException("Password validation failed");

            //Check minimum number of digits
            int count = 0;
            for (int i = 0; i < password.Length; i++)
            {
                if (!char.IsLetterOrDigit(password, i))
                    count++;
            }
            if (count < MinRequiredNonAlphanumericCharacters)
                throw new MembershipPasswordException("Password validation failed");

            //Check with the regular expression
            if (PasswordStrengthRegularExpression.Length > 0)
            {
                if (!Regex.IsMatch(password, PasswordStrengthRegularExpression))
                    throw new MembershipPasswordException("Password validation failed");
            }

            //Use a custom check if defined
            var args =
                new ValidatePasswordEventArgs(user, password, isNew);

            OnValidatingPassword(args);
            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Password validation failed");
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                ValidatePassword(username, newPassword, false);

                using (var transaction = new TransactionScope(_mConfiguration))
                {
                    var dataStore = new UserDataStore(transaction);
                    User user = dataStore.FindByName(ApplicationName, username);
                    if (user == null)
                        throw new UserNotFoundException(username);

                    if (user.CheckPassword(oldPassword) == false)
                        throw new UserNotFoundException(username);

                    user.ChangePassword(newPassword);

                    transaction.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, "ChangePassword");
                return false;
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
                                                             string newPasswordQuestion, string newPasswordAnswer)
        {
            try
            {
                using (var transaction = new TransactionScope(_mConfiguration))
                {
                    var dataStore = new UserDataStore(transaction);
                    User user = dataStore.FindByName(ApplicationName, username);
                    if (user == null)
                        throw new UserNotFoundException(username);

                    if (user.CheckPassword(password) == false)
                        throw new UserNotFoundException(username);

                    user.ChangePasswordQuestionAnswer(newPasswordQuestion, newPasswordAnswer);

                    transaction.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, "ChangePasswordQuestionAndAnswer");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="isApproved"></param>
        /// <param name="providerUserKey">Not used</param>
        /// <param name="status"></param>
        /// <returns></returns>
        public override MembershipUser CreateUser(string username, string password,
                                                  string email, string passwordQuestion,
                                                  string passwordAnswer, bool isApproved,
                                                  object providerUserKey,
                                                  out MembershipCreateStatus status)
        {
            try
            {
                //Validate password
                ValidatePassword(username, password, true);

                using (var transaction = new TransactionScope(_mConfiguration))
                {
                    var dataStore = new UserDataStore(transaction);

                    //Check name
                    if (dataStore.FindByName(ApplicationName, username) != null)
                    {
                        status = MembershipCreateStatus.DuplicateUserName;
                        return null;
                    }

                    //Check email
                    if (RequiresUniqueEmail)
                    {
                        if (string.IsNullOrEmpty(email))
                        {
                            status = MembershipCreateStatus.InvalidEmail;
                            return null;
                        }
                        if (dataStore.FindByEmail(ApplicationName, email).Count > 0)
                        {
                            status = MembershipCreateStatus.DuplicateEmail;
                            return null;
                        }
                    }


                    var user = new User(ApplicationName, username);
                    user.Email = email;
                    user.ChangePassword(password);
                    user.ChangePasswordQuestionAnswer(passwordQuestion, passwordAnswer);
                    user.Enabled = isApproved;

                    dataStore.Insert(user);

                    transaction.Commit();

                    status = MembershipCreateStatus.Success;
                    return UserToMembershipUser(user);
                }
            }
            catch (CodeInvalidCharsException ex) //this exception is caused by an invalid user Name
            {
                LogException(ex, "CreateUser");
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            catch (MembershipPasswordException ex)
            {
                LogException(ex, "CreateUser");
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            catch (Exception ex)
            {
                LogException(ex, "CreateUser");
                status = MembershipCreateStatus.ProviderError;
                return null;
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            try
            {
                using (var transaction = new TransactionScope(_mConfiguration))
                {
                    if (deleteAllRelatedData)
                    {
                        var dataStore = new UserDataStore(transaction);
                        User user = dataStore.FindByName(ApplicationName, username);
                        var userInRoleStore = new UserInRoleDataStore(transaction);
                        IList<UserInRole> userInRoles = userInRoleStore.FindForUser(ApplicationName, user);
                        foreach (UserInRole ur in userInRoles)
                        {
                            ur.Deleted = true;
                            userInRoleStore.Update(ur);
                        }
                    }

                    var dataStore1 = new UserDataStore(transaction);
                    User user1 = dataStore1.FindByName(ApplicationName, username);
                    if (user1 == null)
                        throw new UserNotFoundException(username);

                    // Rename the user before deleting, this will allow a new user with the same name.
                    user1.Name = user1.Name + DateTimeHelper.GetCurrentTimestamp();
                    user1.Deleted = true;
                    dataStore1.Update(user1);

                    transaction.Commit();
                }

                return true;
            }
            catch (UserNotDeletedException ex)
            {
                LogException(ex, "DeleteUser");
                return false;
            }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch,
                                                                  int pageIndex, int pageSize, out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();

            using (var transaction = new TransactionScope(_mConfiguration))
            {
                var dataStore = new UserDataStore(transaction);

                var paging = new PagingInfo(pageSize, pageIndex);
                IList<User> users = dataStore.FindByEmailLike(ApplicationName, emailToMatch, paging);
                totalRecords = (int) paging.RowCount;

                foreach (User u in users)
                    membershipUsers.Add(UserToMembershipUser(u));
            }

            return membershipUsers;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch,
                                                                 int pageIndex, int pageSize, out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();

            using (var transaction = new TransactionScope(_mConfiguration))
            {
                var dataStore = new UserDataStore(transaction);

                var paging = new PagingInfo(pageSize, pageIndex);
                IList<User> users = dataStore.FindByNameLike(ApplicationName, usernameToMatch, paging);
                totalRecords = (int) paging.RowCount;

                foreach (User u in users)
                    membershipUsers.Add(UserToMembershipUser(u));
            }

            return membershipUsers;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();

            using (var transaction = new TransactionScope(_mConfiguration))
            {
                var dataStore = new UserDataStore(transaction);

                var paging = new PagingInfo(pageSize, pageIndex);
                IList<User> users = dataStore.FindAll(ApplicationName, paging);
                totalRecords = (int) paging.RowCount;

                foreach (User u in users)
                {
                    Person p = PersonManager.GetPersonByUserId(u.Id);

                    if (p != null)
                        membershipUsers.Add(UserToMembershipUser(u));
                }
            }

            return membershipUsers;
        }

        public override int GetNumberOfUsersOnline()
        {
            var onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);

            using (var transaction = new TransactionScope(_mConfiguration))
            {
                var dataStore = new UserDataStore(transaction);
                return dataStore.NumbersOfLoggedInUsers(ApplicationName, onlineSpan);
            }
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException("Password retrieval not supported");
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (var transaction = new TransactionScope(_mConfiguration))
            {
                var dataStore = new UserDataStore(transaction);
                User user = dataStore.FindByName(ApplicationName, username);
                if (user == null)
                    return null;

                if (userIsOnline)
                    user.LastActivityDate = DateTime.Now;

                transaction.Commit();

                return UserToMembershipUser(user);
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            using (var transaction = new TransactionScope(_mConfiguration))
            {
                var dataStore = new UserDataStore(transaction);
                User user = dataStore.FindByKey((string) providerUserKey);
                if (user == null)
                    return null;

                if (userIsOnline)
                    user.LastActivityDate = DateTime.Now;

                transaction.Commit();

                return UserToMembershipUser(user);
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            using (var transaction = new TransactionScope(_mConfiguration))
            {
                var dataStore = new UserDataStore(transaction);
                IList<User> users = dataStore.FindByEmail(ApplicationName, email);
                if (users.Count == 0)
                    return null;

                return users[0].Name;
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException();
            }

            using (var transaction = new TransactionScope(_mConfiguration))
            {
                var dataStore = new UserDataStore(transaction);
                User user = dataStore.FindByName(ApplicationName, username);
                if (user == null)
                    throw new UserNotFoundException(username);

                if (RequiresQuestionAndAnswer &&
                    user.ValidatePasswordAnswer(answer, PasswordAttemptWindow, MaxInvalidPasswordAttempts) == false)
                {
                    transaction.Commit();
                    throw new MembershipPasswordException();
                }
                else
                {
                    string newPassword = System.Web.Security.Membership.GeneratePassword(MinRequiredPasswordLength,
                                                                                         MinRequiredNonAlphanumericCharacters);
                    user.ChangePassword(newPassword);
                    transaction.Commit();
                    return newPassword;
                }
            }
        }

        public override bool UnlockUser(string userName)
        {
            using (var transaction = new TransactionScope(_mConfiguration))
            {
                var dataStore = new UserDataStore(transaction);
                User user = dataStore.FindByName(ApplicationName, userName);
                if (user == null)
                    throw new UserNotFoundException(userName);

                user.Unlock();
                transaction.Commit();
            }

            return true;
        }

        public override void UpdateUser(MembershipUser user)
        {
            using (var transaction = new TransactionScope(_mConfiguration))
            {
                var dataStore = new UserDataStore(transaction);
                User dbUser = dataStore.FindByName(ApplicationName, user.UserName);
                if (dbUser == null)
                    throw new UserNotFoundException(user.UserName);

                //Check email
                if (RequiresUniqueEmail)
                {
                    if (string.IsNullOrEmpty(user.Email))
                    {
                        throw new EmailNotValidException(user.Email);
                    }

                    IList<User> emailUsers = dataStore.FindByEmail(ApplicationName, user.Email);
                    if (emailUsers.Count > 0 && emailUsers[0].Id != dbUser.Id)
                    {
                        throw new EmailDuplicatedException(user.Email);
                    }
                }

                dbUser.Comment = user.Comment;
                dbUser.Email = user.Email;
                dbUser.Enabled = user.IsApproved;

                transaction.Commit();
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            using (var transaction = new TransactionScope(_mConfiguration))
            {
                var dataStore = new UserDataStore(transaction);
                User dbUser = dataStore.FindByName(ApplicationName, username);
                if (dbUser == null)
                    return false; //throw new UserNotFoundException(username);

                bool valid = dbUser.LogOn(password, PasswordAttemptWindow, MaxInvalidPasswordAttempts);

                transaction.Commit();
                return valid;
            }
        }

        #endregion
    }
}