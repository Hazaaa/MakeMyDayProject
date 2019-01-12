using Databases.Neo4j.DomainModel;
using Databases.Neo4j.DomainModel.Relationships;
using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Neo4j
{
    public class Neo4jFunctions
    {

        private GraphClient client;

        public Neo4jFunctions()
        {
            client = new GraphClient(new Uri("http://localhost:7474/db/data"));

            client.Connect();
        }

        #region Create functions

        // Creates user node
        public bool CreateUserNode(User newUser)
        {
            // Parameters for query need to be in dictionary
            Dictionary<string, object> paramsDictionary = new Dictionary<string, object>();
            paramsDictionary.Add("aspid", newUser.aspid);
            paramsDictionary.Add("username", newUser.username);

            // Merge helps that node is unique when creating node
            var query = new Neo4jClient.Cypher.CypherQuery("MERGE (n:User { aspid: { aspid }, username: { username }}) return n",
                paramsDictionary, CypherResultMode.Set);

            User user = ((IRawGraphClient)client).ExecuteGetCypherResults<User>(query).ToList().FirstOrDefault();

            if (user == null)
                return false;
            else
                return true;
        }

        // Creates post node via CyptherQuey
        public bool CreatePostNode(Post newPost)
        {
            // Parameters for query need to be in dictionary
            Dictionary<string, object> paramsDictionary = new Dictionary<string, object>();

            paramsDictionary.Add("id", newPost.id);
            paramsDictionary.Add("content", newPost.content);

            var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Post { id: { id }, content: { content }}) return n",
                paramsDictionary, CypherResultMode.Set);

            Post post = ((IRawGraphClient)client).ExecuteGetCypherResults<Post>(query).ToList().FirstOrDefault();

            if (post == null)
                return false;
            else
                return true;
        }

        // Creates picture node
        public bool CreatePictureNode(Picture newPicture)
        {

            // Parameters for query need to be in dictionary
            Dictionary<string, object> paramsDictionary = new Dictionary<string, object>();
            paramsDictionary.Add("url", newPicture.url);

            // Merge helps that node is unique when creating node
            var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Picture { url: { url }}) return n",
                paramsDictionary, CypherResultMode.Set);

            Picture picture = ((IRawGraphClient)client).ExecuteGetCypherResults<Picture>(query).ToList().FirstOrDefault();

            if (picture == null)
                return false;
            else
                return true;

        }

        // Creates hashtag node
        public bool CreateHashtagNode(Hashtag newHashtag)
        {
            // Parameters for query need to be in dictionary
            Dictionary<string, object> paramsDictionary = new Dictionary<string, object>();
            paramsDictionary.Add("text", newHashtag.text);

            // Merge helps that node is unique when creating node
            var query = new Neo4jClient.Cypher.CypherQuery("MERGE (n:Hashtag { text: { text } }) return n",
                paramsDictionary, CypherResultMode.Set);

            Hashtag hashtag = ((IRawGraphClient)client).ExecuteGetCypherResults<Hashtag>(query).ToList().FirstOrDefault();

            if (hashtag == null)
                return false;
            else
                return true;
        }

        // Creates comment node
        public bool CreateCommentNode(Comment newComment)
        {
            // Parameters for query need to be in dictionary
            Dictionary<string, object> paramsDictionary = new Dictionary<string, object>();
            paramsDictionary.Add("text", newComment.text);
            paramsDictionary.Add("id", newComment.id);

            // Merge helps that node is unique when creating node
            var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Comment { id: { id } , text: { text } }) return n",
                paramsDictionary, CypherResultMode.Set);

            Comment comment = ((IRawGraphClient)client).ExecuteGetCypherResults<Comment>(query).ToList().FirstOrDefault();

            if (comment == null)
                return false;
            else
                return true;
        }

        // Creates message node
        public bool CreateMessageNode(Message newMessage)
        {
            // Parameters for query need to be in dictionary
            Dictionary<string, object> paramsDictionary = new Dictionary<string, object>();
            paramsDictionary.Add("text", newMessage.text);
            paramsDictionary.Add("id", newMessage.id);

            // Merge helps that node is unique when creating node
            var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Message { id: { id } , text: { text } }) return n",
                paramsDictionary, CypherResultMode.Set);

            Message message = ((IRawGraphClient)client).ExecuteGetCypherResults<Message>(query).ToList().FirstOrDefault();

            if (message == null)
                return false;
            else
                return true;
        }

        // Creates relationship between post and user that created post
        public void CreatePostedRelationship(Posted posted)
        { 
            // Create post node has to be unique
            CreatePostNode(posted.post);

            var query = client.Cypher
                .Match("(u:User)", "(p:Post)")
                .Where((User u) => u.username == posted.user.username)
                .AndWhere((Post p) => p.id == posted.post.id)
                .Create("(u)-[r:POSTED {time: '" + posted.time + "'}]->(p)");

            query.ExecuteWithoutResults();
        }

        // Creates relationship between post and picture or user and picture
        public void CreatePictureRelationship(HasProfilePicture userPicture = null, HasPicture postPicture = null)
        {
            if(postPicture != null)
            {
                // Create picture node
                CreatePictureNode(postPicture.picture);

                var query = client.Cypher
                    .Match("(post:Post)", "(picture:Picture)")
                    .Where((Post post) => post.id == postPicture.post.id)
                    .AndWhere((Picture picture) => picture.url == postPicture.picture.url)
                    .Create("(post)-[r:HAS_PICTURE {time: '" + postPicture.time + "'}]->(picture)");

                query.ExecuteWithoutResults();
            }

            if(userPicture != null)
            {
                // Create user first because this part will run when user register
                CreateUserNode(userPicture.user);

                // Create picture node
                CreatePictureNode(userPicture.profilePicture);

                var query = client.Cypher
                    .Match("(user:User)", "(picture:Picture)")
                    .Where((User user) => user.aspid == userPicture.user.aspid)
                    .AndWhere((Picture picture) => picture.url == userPicture.profilePicture.url)
                    .Create("(user)-[r:HAS_PROFILEPICTURE {time: '" + userPicture.time + "'}]->(picture)");

                query.ExecuteWithoutResults();
            }
        }

        // Creates relationship between post and hashtag
        public void CreateHashtagRelationship(HasHashtag hasHashtag)
        {
            // Creates hashtag node if doesn't exists or does nothing if already exists
            CreateHashtagNode(hasHashtag.hashtag);

            var query = client.Cypher
                .Match("(post:Post)", "(hashtag:Hashtag)")
                .Where((Post post) => post.id == hasHashtag.post.id)
                .AndWhere((Hashtag hashtag) => hashtag.text == hasHashtag.hashtag.text)
                .Create("(post)-[r:HAS_HASHTAG {time: '" + hasHashtag.time + "'}]->(hashtag)");

            query.ExecuteWithoutResults();
        }

        // Creates relationship between post and tag and between user and tag
        public void CreateTaggedRelationship(Tagged tagged)
        {
            var query = client.Cypher
            .Match("(post:Post)", "(user:User)")
            .Where((Post post) => post.id == tagged.post.id)
            .AndWhere((User user) => user.username == tagged.tagged.username)
            .Create("(post)-[r:TAGGED {time: '" + tagged.time + "'}]->(user)");

            query.ExecuteWithoutResults();
        }

        // Creates relationship between post and user that says if user liked or disliked post
        public void CreateLikedDislikedRelationship(Liked liked = null, Disliked disliked = null)
        {
            // These queries will create Like or Dislike relationship between user and post 
            // But if there is already like or dislike relationship between same user and post
            // It will be deleted and new one will be created (if disliked and there is like relationship, like will be deleted and only disliked will exist)
            // Also because of merge there will always be one like or one dislike relationship 
            if(liked != null)
            {
                var query = client.Cypher
                    .Match("(post:Post)", "(user:User)")
                    .OptionalMatch("(user)-[d:DISLIKED]->(post)")
                    .With("post,user,d")
                    .Where((Post post) => post.id == liked.post.id)
                    .AndWhere((User user) => user.username == liked.user.username)
                    .Merge("(user)-[l:LIKED]->(post)")
                    .OnCreate()
                    .Set("l.time = {time}")
                    .WithParam("time", liked.time)
                    .DetachDelete("d");

                query.ExecuteWithoutResults();
            }
            else
            {
                var query = client.Cypher
                    .Match("(post:Post)", "(user:User)")
                    .OptionalMatch("(user)-[l:LIKED]->(post)")
                    .With("post,user,l")
                    .Where((Post post) => post.id == disliked.post.id)
                    .AndWhere((User user) => user.username == disliked.user.username)
                    .Merge("(user)-[d:DISLIKED]->(post)")
                    .OnCreate()
                    .Set("d.time = {time}")
                    .WithParam("time", disliked.time)
                    .DetachDelete("l");

                query.ExecuteWithoutResults();
            }
            
        }

        // Creates realationship between user and user
        public void CreateFollowRelationship(Follow follow)
        {
            var query = client.Cypher
                .Match("(follower:User)", "(followed:User)")
                .Where((User follower) => follower.username == follow.follower.username)
                .AndWhere((User followed) => followed.username == follow.followed.username)
                .Create("(follower)-[r:FOLLOW {time: '" + follow.time + "'}]->(followed)");

            query.ExecuteWithoutResults();
        }

        // Creates realationship between post and comment and user and comment
        public bool CreateCommentRelationships(HasComment hasComment, Commented commented)
        {
            // Firts create comment node
            CreateCommentNode(hasComment.comment);

            // Parameters for query need to be in dictionary
            Dictionary<string, object> paramsDictionary = new Dictionary<string, object>();
            paramsDictionary.Add("text", hasComment.comment.text);
            paramsDictionary.Add("commentid", hasComment.comment.id);
            paramsDictionary.Add("postid", hasComment.post.id);
            paramsDictionary.Add("username", commented.commentator.username);
            paramsDictionary.Add("time", commented.time);

            var query = new Neo4jClient.Cypher.CypherQuery("match (p:Post), (u:User), (c:Comment) where p.id = {postid} and u.username = {username} and c.id = {commentid}" +
                " create (p)-[r:HAS_COMMENT {time: {time}}]->(c)<-[ro:COMMENTED {time: {time}}]-(u) return c",
                paramsDictionary, CypherResultMode.Set);

            Comment comment = ((IRawGraphClient)client).ExecuteGetCypherResults<Comment>(query).ToList().FirstOrDefault();

            if (comment != null)
                return true;
            else
                return false;
        }

        // Creates relationship between user that sends message and message node and also between user that recieve message and message node
        public bool CreateSendRecieveRelationships(Send send, Recieve recieve)
        {
            // First create message node
            CreateMessageNode(send.message);

            // Parameters for query need to be in dictionary
            Dictionary<string, object> paramsDictionary = new Dictionary<string, object>();
            paramsDictionary.Add("senderusername", send.sender.username);
            paramsDictionary.Add("recieverusername", recieve.reciever.username);
            paramsDictionary.Add("messageid", send.message.id);
            paramsDictionary.Add("time", send.time);

            var query = new Neo4jClient.Cypher.CypherQuery("match (sender:User), (message:Message), (reciever:User) " +
                "where sender.username = {senderusername} and reciever.username = {recieverusername} and message.id = {messageid}" +
                " create (sender)-[r:SEND {time: {time}}]->(message)-[ro:RECIEVE {time: {time}}]->(reciever) return message",
                paramsDictionary, CypherResultMode.Set);

            Message message = ((IRawGraphClient)client).ExecuteGetCypherResults<Message>(query).ToList().FirstOrDefault();

            if (message != null)
                return true;
            else
                return false;
        }

        #endregion


        #region Get functions

        // Return all users
        public IEnumerable<User> GetAllUsers()
        {
            var queryUsers = client.Cypher
                .Match("(user:User)")
                .Return<User>("user")
                .Results;

            return queryUsers;
        }

        // Returns user by username with simple data
        public User GetUser(string username)
        {
            var queryUser = client.Cypher
                .Match("(user:User)")
                .Where((User user) => user.username == username)
                .OptionalMatch("(user)-[posted:POSTED]->(post:Post)")
                .OptionalMatch("(user)-[:HAS_PROFILEPICTURE]->(picture:Picture)")
                .OptionalMatch("(user)-[followed:FOLLOW]->(user2:User)")
                .OptionalMatch("(user3:User)-[follower:FOLLOW]->(user)")
                .With("user, picture, user2.username as userFollow, user3.username as userFollower, count(posted) as numOfPosts")
                .ReturnDistinct((user, userFollow, userFollower, numOfPosts, picture) => new User
                {
                    aspid = user.As<User>().aspid,
                    username = user.As<User>().username,
                    followed = userFollow.CollectAsDistinct<string>(),
                    followers = userFollower.CollectAsDistinct<string>(),
                    numberofposts = numOfPosts.As<int>(),
                    profilepictureurl = picture.As<Picture>().url
                })
                .Results;

            return queryUser.FirstOrDefault();
        }

        // Return all posts and their data and if username is passed it will return all posts of that user
        public IEnumerable<Post> GetAllPosts(int skip, string username = null)
        {
            if(username != null)
            {
                // Getting all posts of user with all its comments, hashtags, number of likes, dislikes, picture url and time its created
                var queryPosts = client.Cypher
                    .Match("(user:User)-[r:POSTED]->(p:Post)")
                    .Where((User user) => user.username == username)
                    .OptionalMatch("(p)-[:HAS_PICTURE]->(picture:Picture)")
                    .OptionalMatch("(p)-[:HAS_HASHTAG]->(hashtag:Hashtag)")
                    .OptionalMatch("(p)-[:HAS_COMMENT]->(comment:Comment)")
                    .OptionalMatch("(user)-[:HAS_PROFILEPICTURE]->(profile:Picture)")
                    .OptionalMatch("(user3:User)-[like:LIKED]->(p)")
                    .OptionalMatch("(user4:User)-[dislike:DISLIKED]->(p)")
                    .With("p, user, r, profile, picture, hashtag, comment, count(user3) as likes, count(user4) as dislikes")
                    .ReturnDistinct((p, r, user, profile, picture, likes, dislikes, hashtag, comment) => new Post
                    {
                        id = p.As<Post>().id,
                        timeCreated = r.As<Posted>().time,
                        creator = user.As<User>(),
                        creatorPict = profile.As<Picture>().url,
                        content = p.As<Post>().content,
                        pictureurl = picture.As<Picture>().url,
                        dislikes = dislikes.As<int>(),
                        likes = likes.As<int>(),
                        hashtags = hashtag.CollectAs<Hashtag>(),
                        comments = comment.CollectAs<Comment>()
                    })
                    .OrderBy("r.time DESC")
                    .Skip(skip)
                    .Limit(10)
                    .Results;

                return queryPosts;
            }
            else
            {
                // Getting all posts with all its comments, hashtags, number of likes, dislikes, picture url and time its created
                var queryPosts = client.Cypher
                    .Match("(user:User)-[r:POSTED]->(p:Post)")
                    .OptionalMatch("(p)-[:HAS_PICTURE]->(picture:Picture)")
                    .OptionalMatch("(p)-[:HAS_HASHTAG]->(hashtag:Hashtag)")
                    .OptionalMatch("(p)-[:HAS_COMMENT]->(comment:Comment)")
                    .OptionalMatch("(user3:User)-[like:LIKED]->(p)")
                    .OptionalMatch("(user4:User)-[dislike:DISLIKED]->(p)")
                    .OptionalMatch("(p)-[:TAGGED]->(user2:User)")
                    .With("p, user, r, picture, hashtag, user2.username as taggedusers, comment, count(user3) as likes, count(user4) as dislikes")
                    .ReturnDistinct((p, r, user, picture, likes, dislikes, hashtag, comment, taggedusers) => new Post
                    {
                        id = p.As<Post>().id,
                        timeCreated = r.As<Posted>().time,
                        creator = user.As<User>(),
                        content = p.As<Post>().content,
                        pictureurl = picture.As<Picture>().url,
                        dislikes = dislikes.As<int>(),
                        likes = likes.As<int>(),
                        hashtags = hashtag.CollectAs<Hashtag>(),
                        comments = comment.CollectAs<Comment>(),
                        taggedusers = taggedusers.CollectAs<string>()
                    })
                    .OrderBy("r.time DESC")
                    .Skip(skip)
                    .Limit(10)
                    .Results;

                return queryPosts;
            }
        }

        // Returns post by id
        public Post GetPost(string id)
        {
            // Getting all posts with all its comments, hashtags, number of likes, dislikes, picture url and time its created
            var queryPosts = client.Cypher
                .Match("(user:User)-[r:POSTED]->(p:Post)")
                .Where((Post p) => p.id.ToString() == id)
                .OptionalMatch("(p)-[:HAS_PICTURE]->(picture:Picture)")
                .OptionalMatch("(p)-[:HAS_HASHTAG]->(hashtag:Hashtag)")
                .OptionalMatch("(p)-[:HAS_COMMENT]->(comment:Comment)")
                .With("p, user, r, picture, hashtag, comment, size((user)-[:LIKED]->(p)) as likes, size((user)-[:DISLIKED]->(p)) as dislikes")
                .ReturnDistinct((p, r, user, picture, likes, dislikes, hashtag, comment) => new Post
                {
                    id = p.As<Post>().id,
                    timeCreated = r.As<Posted>().time,
                    creator = user.As<User>(),
                    content = p.As<Post>().content,
                    pictureurl = picture.As<Picture>().url,
                    dislikes = dislikes.As<int>(),
                    likes = likes.As<int>(),
                    hashtags = hashtag.CollectAs<Hashtag>(),
                    comments = comment.CollectAs<Comment>()
                })
                .Results;

            return queryPosts.FirstOrDefault();
        }

        // Returns all comments of post
        public IEnumerable<Comment> GetPostComments(string id)
        {
            var query = client.Cypher
                .Match("(post:Post)-[hascomment:HAS_COMMENT]->(comment:Comment)")
                .Where((Post post) => post.id == new Guid(id))
                .OptionalMatch("(user:User)-[:COMMENTED]->(comment)")
                .With("comment, user, hascomment")
                .ReturnDistinct((comment, user, hascomment) => new Comment
                {
                    id = comment.As<Comment>().id,
                    creator = user.As<User>(),
                    text = comment.As<Comment>().text,
                    time = hascomment.As<HasComment>().time
                })
                .OrderBy("hascomment.time ASC")
                .Results;

            return query;
        }

        // Returns all hashtags of user or 10 other hashtags
        public IEnumerable<Hashtag> GetHashtags(string username = null)
        {
            if(username != null)
            {
                var query = client.Cypher
                .Match("(user:User)-[posted:POSTED]->(post:Post)-[hashashtag:HAS_HASHTAG]->(hashtag:Hashtag)")
                .Where((User user) => user.username == username)
                .Return<Hashtag>("hashtag")
                .Results;

                return query;
            }
            else
            {
                var query = client.Cypher
                .Match("(hashtag:Hashtag)")
                .Return<Hashtag>("hashtag")
                .Limit(10)
                .Results;

                return query;
            }
        }

        // Returns all posts that contains hashtag
        public IEnumerable<Post> GetPostsWithHashtag(string hashtagToFind)
        {
            // Getting all posts with hashtag in it
            var queryPosts = client.Cypher
                .Match("(user:User)-[r:POSTED]->(p:Post)")
                .Where((Post p) => p.content.Contains(hashtagToFind))
                .OptionalMatch("(p)-[:HAS_PICTURE]->(picture:Picture)")
                .OptionalMatch("(p)-[:HAS_HASHTAG]->(hashtag:Hashtag)")
                .OptionalMatch("(p)-[:HAS_COMMENT]->(comment:Comment)")
                .OptionalMatch("(user)-[like:LIKED]->(p)")
                .OptionalMatch("(user)-[dislike:DISLIKED]->(p)")
                .OptionalMatch("(user)-[:HAS_PROFILEPICTURE]->(profile:Picture)")
                .OptionalMatch("(p)-[:TAGGED]->(user2:User)")
                .With("p, user, r, picture, profile, hashtag, user2.username as taggedusers, comment, count(like) as likes, count(dislike) as dislikes")
                .ReturnDistinct((p, r, user, profile, picture, likes, dislikes, hashtag, comment, taggedusers) => new Post
                {
                    id = p.As<Post>().id,
                    timeCreated = r.As<Posted>().time,
                    creator = user.As<User>(),
                    creatorPict = profile.As<Picture>().url,
                    content = p.As<Post>().content,
                    pictureurl = picture.As<Picture>().url,
                    dislikes = dislikes.As<int>(),
                    likes = likes.As<int>(),
                    hashtags = hashtag.CollectAs<Hashtag>(),
                    comments = comment.CollectAs<Comment>(),
                    taggedusers = taggedusers.CollectAs<string>()
                })
                .OrderBy("r.time DESC")
                .Results;

            return queryPosts;
        }

        // Returns top 10 most liked posts
        public IEnumerable<Post> GetTopRatedPosts()
        {
            // Getting top rated posts with all its comments, hashtags, number of likes, dislikes, picture url and time its created
            var queryPosts = client.Cypher
                .Match("(user:User)-[r:POSTED]->(p:Post)")
                .OptionalMatch("(p)-[:HAS_PICTURE]->(picture:Picture)")
                .OptionalMatch("(p)-[:HAS_HASHTAG]->(hashtag:Hashtag)")
                .OptionalMatch("(p)-[:HAS_COMMENT]->(comment:Comment)")
                .OptionalMatch("(user)-[:HAS_PROFILEPICTURE]->(profile:Picture)")
                .OptionalMatch("(user3:User)-[like:LIKED]->(p)")
                .OptionalMatch("(user4:User)-[dislike:DISLIKED]->(p)")
                .OptionalMatch("(p)-[:TAGGED]->(user2:User)")
                .With("p, user, r, picture, hashtag, profile, user2.username as taggedusers, comment, count(user3) as likes, count(user4) as dislikes")
                .ReturnDistinct((p, r, user, picture, profile, likes, dislikes, hashtag, comment, taggedusers) => new Post
                {
                    id = p.As<Post>().id,
                    timeCreated = r.As<Posted>().time,
                    creator = user.As<User>(),
                    creatorPict = profile.As<Picture>().url,
                    content = p.As<Post>().content,
                    pictureurl = picture.As<Picture>().url,
                    dislikes = dislikes.As<int>(),
                    likes = likes.As<int>(),
                    hashtags = hashtag.CollectAs<Hashtag>(),
                    comments = comment.CollectAs<Comment>(),
                    taggedusers = taggedusers.CollectAs<string>()
                })
                .OrderBy("likes DESC")
                .Limit(10)
                .Results;

            return queryPosts;
        }

        // Returns all messages between two users
        public IEnumerable<Message> GetAllMessagesBetweenUsers(string senderUsername, string recieverUsername)
        {
            var query = client.Cypher
                .Match("(sender:User)-[send:SEND]->(message:Message)-[recieve:RECIEVE]->(reciever:User)")
                .Where((User sender) => sender.username == senderUsername || sender.username == recieverUsername)
                .AndWhere((User reciever) => reciever.username == recieverUsername || reciever.username == senderUsername)
                .OptionalMatch("(sender)-[:HAS_PROFILEPICTURE]->(picture:Picture)")
                .With("sender, message, reciever, send.time as timeSended, picture.url as senderPic")
                .Return((message, sender, reciever, timeSended, senderPic) => new Message
                {
                    id = message.As<Message>().id,
                    text = message.As<Message>().text,
                    reciever = reciever.As<User>(),
                    sender = sender.As<User>(),
                    senderPic = senderPic.As<string>(),
                    timesent = timeSended.As<string>()
                })
                .OrderBy("timeSended ASC")
                .Results;

            return query;
        }

        // Returns profilepicture url for user
        public Picture GetUserProfilePicture(string username)
        {
            var query = client.Cypher
                .Match("(user:User)-[:HAS_PROFILEPICTURE]->(picture:Picture)")
                .Where((User user) => user.username == username)
                .Return<Picture>("picture")
                .Results;

            return query.FirstOrDefault();
        }

        #endregion


        #region Update functions

        // Updating comment
        public void UpdateComment(string id, string text)
        {
            var query = client.Cypher
                .Match("(comment:Comment)")
                .Where((Comment comment) => comment.id == new Guid(id))
                .Set("comment.text = {text}")
                .WithParam("text", text);

            query.ExecuteWithoutResults();
        }

        // Updating profile picture of user
        public void UpdateProfilePicture(string username, string filePath)
        {
            var query = client.Cypher
                .Match("(user:User)-[h:HAS_PROFILEPICTURE]->(picture:Picture)")
                .Where((User user) => user.username == username)
                .Set("picture.url = {url}")
                .WithParam("url", filePath);

            query.ExecuteWithoutResults();
        }

        #endregion


        #region Delete functions

        // Deleting comment
        public void DeleteComment(string id)
        {
            var query = client.Cypher
                .Match("(comment:Comment)")
                .Where((Comment comment) => comment.id == new Guid(id))
                .DetachDelete("comment");

            query.ExecuteWithoutResults();
        }

        // Deleting post
        public void DeletePost(string id)
        {
            var query = client.Cypher
                .Match("(post:Post)")
                .Where((Post post) => post.id == new Guid(id))
                .OptionalMatch("(post)-[r:HAS_PICTURE]->(picture:Picture)")
                .OptionalMatch("(post)-[c:HAS_COMMENT]->(comment:Comment)")
                .DetachDelete("post, picture, comment");

            query.ExecuteWithoutResults();
        }

        // Deleting follow relationship
        public void DeleteFollowRelationship(string whofollow, string tounfollow)
        {
            var query = client.Cypher
                .Match("(follower:User)-[follow:FOLLOW]->(followed:User)")
                .Where((User follower) => follower.username == whofollow)
                .AndWhere((User followed) => followed.username == tounfollow)
                .Delete("follow");

            query.ExecuteWithoutResults();
        }

        #endregion
    }
}
