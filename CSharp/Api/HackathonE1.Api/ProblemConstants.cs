using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api
{
	public class ProblemConstants
	{
		public const string UserNotFound = "User does not exist";
		public const string UserEmailTaken = "Given email is already taken";
		public const string UnknownError = "Unidentified error occured";

		public const string IssueNotFound = "Issue does not exist";
		public const string IssueExists= "Issue already exists";

		public const string IssueTypeNotFound = "Issue type does not exist";
		public const string IssueTypeExists= "Issue type already exists";

		public const string AreaNotFound = "Aera is not registered";
		public const string AreaNotObserved = "Aera not observed by this user";
		public const string AreaObserved = "Aera is already observed by this user";
	}
}
