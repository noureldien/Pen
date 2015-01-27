// ## Some important Links
// Premission Link
// http://www.facebook.com/login.php?api_key=a8eb4e099c802155c49ff82d58aa747e
// &connect_display=popup&v=1.0&next=http://www.facebook.com/connect/login_success
// .html&cancel_url=http://www.facebook.com/connect/login_failure.html&fbconnect=
// true&return_session=true&req_perms=read_stream,publish_stream,offline_access

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook;

namespace Pen.Tools
{
    /// <summary>
    /// This class enables posting on Facebook walls using Facebook API.
    /// </summary>
    class FacebookAPI
    {
        /// <summary>
        /// Post on te Facebook wall to a user using Pen Facebook App.
        /// </summary>
        /// <param name="post">Text of the wall post.</param>
        /// <param name="userName">Username to post on his Facebook wall.</param>
        /// <param name="rating">The rate of the user (student).</param>
        public static void PostOnWall(string post, string userName, int rating)
        {           
            // Here are the steps
            // 1. Create media item (eg. image)
            // 2. create mediaList and add media item to it
            // 3. Create attachment and add mediaList to it
            // 4. Create actionlink item
            // 5. create actionlink list and add actionlink item to it
            // 6. create a FbSession of kind DesktopSession
            // 7. create a API and add(postText, attachement, actionList) and session to it

            string appLink = "http://apps.Facebook.Rest.com/touchpen/";
            string appKey = "a8eb4e099c802155c49ff82d58aa747e";

            // 1,2.
            Facebook.Rest.attachment_media_image media = new Facebook.Rest.attachment_media_image();
            media.type = Facebook.Rest.attachment_media_type.image;
            media.src = "http://i528.photobucket.com/albums/dd329/onemanengine/star" + rating.ToString() + ".png";
            media.href = "http://i528.photobucket.com/albums/dd329/onemanengine/star" + rating.ToString() + ".png";
            List<Facebook.Rest.attachment_media> mediaList = new List<Facebook.Rest.attachment_media>();
            mediaList.Add(media);
                             
            // 3.       
            Facebook.Rest.attachment attachment = new Facebook.Rest.attachment();
            attachment.name =  "Hoooray, " + userName +  " got rated !" ;
            attachment.caption = "pen-app.com";
            attachment.description = GetAttachementDescription(rating, userName);
            attachment.href = appLink;
            attachment.media = mediaList;

            // 4,5
            Facebook.Schema.action_link actionLink = new Facebook.Schema.action_link();
            actionLink.text = "action link text";
            actionLink.href = appLink;
            List<Facebook.Schema.action_link> actionList = new List<Facebook.Schema.action_link>();
            actionList.Add(actionLink);

            // 6.
            Facebook.Session.FacebookSession session;
            session = new Facebook.Session.DesktopSession(appKey, null, null, true, new List<Facebook.Schema.Enums.ExtendedPermissions>() { Facebook.Schema.Enums.ExtendedPermissions.read_stream, Facebook.Schema.Enums.ExtendedPermissions.publish_stream });
            session.Login();
            //session = new Facebook.Session.DesktopSession(appKey, true);

            // 7.
            Facebook.Rest.Api api = new Facebook.Rest.Api(session);            
            api.Stream.Publish(post, attachment, actionList, null, 0);
        }

        /// <summary>
        /// Take user name and it's rating and return a suitable Attachement Description
        /// according to the given rating.
        /// </summary>       
        private static string GetAttachementDescription(int rating, string userName)
        {
            string attachmentDesc = "The teacher rated you with ";
            switch (rating)
            {
                case 0:
                case 1:
                    attachmentDesc += rating + " star in the lesson. We all expect better form you, " + userName + ".";
                    break;

                case 2:
                    attachmentDesc += rating + " stars in the lesson. It's a good start, " + userName + ".";
                    break;

                case 3:
                    attachmentDesc += rating + " stars in the lesson. You are doing well, " + userName + ".";
                    break;

                case 4:
                    attachmentDesc += rating + " stars in the lesson. That's awesome, " + userName + ".";
                    break;

                case 5:
                    attachmentDesc += rating + " stars in the lesson. " + userName + ", you are the man.";
                    break;

                default:
                    break;
            }

            return attachmentDesc;
        }        
    }
}
