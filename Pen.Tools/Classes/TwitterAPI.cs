using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pen.Tools
{
    class TwitterAPI
    {
        /// <summary>
        /// Sends updates to your account on Twitter.com
        /// </summary>
        /// <param name="post">Message to be sent</param>
        /// <returns>Result of the Post Message Request. Normally = OK</returns>
        public static string SendTweet(string post)
        {
            string requestResult = "OK";

            Twitterizer.OAuthTokens tokens = new Twitterizer.OAuthTokens();
            tokens.AccessToken = "212403183-CrJbOIuXBtM05e6rJ7kkbEKEafWqzsIpcsVan6UG";
            tokens.AccessTokenSecret = "GXx1vvqxCkhLui04mH6AUiYdwMURuaRT5PxFc6Wloks";
            tokens.ConsumerKey = "jbeKsWF7fDrrTE9AgYlAVw";
            tokens.ConsumerSecret = "wtpOzazb5PcIDmgLBQNoXAzBmd8fspL41wsEuxi2nY";

            Twitterizer.StatusUpdateOptions options = new Twitterizer.StatusUpdateOptions();
            options.UseSSL = true;

            Twitterizer.TwitterResponse<Twitterizer.TwitterStatus> response = Twitterizer.TwitterStatus.Update(tokens, post,options);
            
            if (response.Result != Twitterizer.RequestResult.Success)
                requestResult = response.Result.ToString();
                       
            return requestResult;

            //TwitterUser user = new TwitterUser();
            //TwitterResponse<TwitterUser> response = TwitterUser.Show("nreldien");
            //user = response.ResponseObject;
            //requestResult = user.Name;
        }

        public void SendTweets(string post)
        {
        //    //var authinticated = TweetSharp.Twitter.Fluent.FluentTwitter.CreateRequest().

        //    var twitter = FluentTwitter.CreateRequest()
        //.AuthenticateAs("USERNAME", "PASSWORD")
        //.Statuses().Update("testing, one, two, three!")
        //.AsJson();

        //    var response = twitter.Request();

        }

        private void AsyncRequest()
        {
            //IEnumerable<TwitterStatus> statuses;
            //var twitter = FluentTwitter.CreateRequest()
            //                .Statuses().OnPublicTimeline()
            //                .AsJson()
            //                .CallbackTo((s, e) =>
            //                {
            //                    // Set the statuses on a different thread
            //                    statuses = e.AsStatuses();
            //                });

            //var asyncResult = twitter.BeginRequest();

            //// Wait ten seconds for the query to complete (this is a blocking call for demonstration purposes)
            //var finished = asyncResult.AsyncWaitHandle.WaitOne(10000);

            //// Work with the statuses object
            //foreach (var status in statuses)
            //{
            //    Console.WriteLine(status.Text);
            //}
        }

        


    }
}