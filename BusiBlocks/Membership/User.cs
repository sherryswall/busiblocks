using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using BusiBlocks.PersonLayer;

namespace BusiBlocks.Membership
{
    /// <summary>
    /// Class entity of a user, used for the membership provider
    /// </summary>
    public class User : IAudit
    {
        private string mName;

        protected User()
        {
        }

        public User(string applicationName, string userName)
        {
            ApplicationName = applicationName;
            Name = userName;
        }

        public virtual string Id { get; protected set; }

        public virtual string Name
        {
            get { return mName; }
            set
            {
                EntityHelper.ValidateCode("Name", value);
                mName = value;
            }
        }

        public virtual string Email { get; set; }

        protected virtual string Password { get; set; }

        public virtual string PasswordQuestion { get; protected set; }

        public virtual string PasswordAnswer { get; protected set; }

        public virtual string ApplicationName { get; protected set; }

        public virtual string Comment { get; set; }

        public virtual bool Enabled { get; set; }

        public virtual string Location { get; set; }

        /// <summary>
        /// Last invalid login password date
        /// </summary>
        public virtual DateTime? LastFailedPasswordDate { get; protected set; }

        /// <summary>
        /// Last successfully login date
        /// </summary>
        public virtual DateTime? LastLogOnDate { get; protected set; }

        public virtual DateTime? LastActivityDate { get; set; }

        public virtual DateTime? LastPasswordChangedDate { get; protected set; }

        public virtual bool IsLockedOut { get; protected set; }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OutDate",
            Justification = "LockedOutdate has a different meaning to LockedOutDate")]
        public virtual DateTime? LockedOutDate { get; protected set; }

        public virtual int FailedPasswordAttemptCount { get; protected set; }

        /// <summary>
        /// Field that can be used for user defined extensions.
        /// </summary>
        public virtual string Tag { get; set; }

        public virtual bool PasswordChangeRequired { get; set; }

        public virtual bool Deleted { get; set; }
        public virtual Person Person { get; set; }

        #region IAudit Members

        public virtual DateTime InsertDate { get; set; }

        public virtual DateTime UpdateDate { get; set; }

        #endregion

        /// <summary>
        /// Change the password and the LastPasswordChangedDate date
        /// </summary>
        /// <param name="password"></param>
        /// 
        public virtual void ChangePassword(string password)
        {
            ChangePassword(password, false);
        }


        public virtual void ChangePassword(string password, bool requiresChange)
        {
            Password = EncodePassword(password, PasswordEncoding.Hashed);

            LastPasswordChangedDate = DateTime.Now;

            // If someone's password is changed, unlock them if they were locked out.
            IsLockedOut = false;

            PasswordChangeRequired = requiresChange;
        }

        /// <summary>
        /// Change the password question and answer
        /// </summary>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        public virtual void ChangePasswordQuestionAnswer(string passwordQuestion, string passwordAnswer)
        {
            PasswordQuestion = passwordQuestion;
            PasswordAnswer = passwordAnswer;
        }

        /// <summary>
        /// Check if the password specified is valid
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual bool CheckPassword(string password)
        {
            return Password == EncodePassword(password, PasswordEncoding.Hashed);
        }

        /// <summary>
        /// Check if the password answer specified is valid
        /// </summary>
        /// <param name="passwordAnswer"></param>
        /// <returns></returns>
        public virtual bool CheckPasswordAnswer(string passwordAnswer)
        {
            return string.Equals(passwordAnswer, passwordAnswer, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Check if the password is valid and if valid update the LastLoginDate otherwise update the FailedPwdAttemptCount
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual bool LogOn(string password, int minAttemptWindow, int maxInvalidAttempts)
        {
            if (IsLockedOut || Enabled == false)
                return false;

            bool valid = CheckPassword(password);

            if (valid == false)
            {
                IncrementFailedPasswordAttempt(minAttemptWindow, maxInvalidAttempts);
            }
            else
            {
                FailedPasswordAttemptCount = 0;
                LastLogOnDate = DateTime.Now;
            }

            return valid;
        }

        protected virtual void IncrementFailedPasswordAttempt(int minAttemptWindow, int maxInvalidAttempts)
        {
            var timeFromLastFailedLogin = new TimeSpan(0);
            if (LastFailedPasswordDate != null)
                timeFromLastFailedLogin = DateTime.Now - LastFailedPasswordDate.Value;

            if (timeFromLastFailedLogin.TotalMinutes < minAttemptWindow)
                FailedPasswordAttemptCount++;

            if (FailedPasswordAttemptCount > maxInvalidAttempts)
                IsLockedOut = true;

            LastFailedPasswordDate = DateTime.Now;
        }

        public virtual void Unlock()
        {
            FailedPasswordAttemptCount = 0;
            IsLockedOut = false;
        }

        /// <summary>
        /// Check if the password answer is valid and if not valid update the FailedPwdAttemptCount
        /// </summary>
        /// <param name="passwordAnswer"></param>
        /// <returns></returns>
        public virtual bool ValidatePasswordAnswer(string passwordAnswer, int minAttemptWindow, int maxInvalidAttempts)
        {
            if (IsLockedOut || Enabled == false)
                return false;

            bool valid = CheckPasswordAnswer(passwordAnswer);

            if (valid == false)
            {
                IncrementFailedPasswordAttempt(minAttemptWindow, maxInvalidAttempts);
            }
            else
                FailedPasswordAttemptCount = 0;

            return valid;
        }

        private static string EncodePassword(string password, PasswordEncoding encoding)
        {
            if (encoding != PasswordEncoding.Hashed)
                throw new BusiBlocksException("Password format not valid");

            var sha1CryptoService = new SHA1CryptoServiceProvider();
            byte[] byteValue = Encoding.UTF8.GetBytes(password);
            byte[] hashValue = sha1CryptoService.ComputeHash(byteValue);

            return Convert.ToBase64String(hashValue);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            var user = obj as User;
            if (user == null)
                return false;

            return user.Id == Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum PasswordEncoding
    {
        Hashed = 1
    }
}