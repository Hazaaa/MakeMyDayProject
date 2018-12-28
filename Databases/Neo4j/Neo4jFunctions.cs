﻿using Databases.Neo4j.DomainModel;
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

            // Merge helps that node is unique when creating node
            var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Comment { text: { text } }) return n",
                paramsDictionary, CypherResultMode.Set);

            Comment comment = ((IRawGraphClient)client).ExecuteGetCypherResults<Comment>(query).ToList().FirstOrDefault();

            if (comment == null)
                return false;
            else
                return true;
        }

        // Creates relationship between post and user that created post
        public void CreatePostedRelationship(Posted posted)
        {
            // Create user node if isn't created
            // CreateUserNode(posted.user);

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
            if(liked != null)
            {
                var query = client.Cypher
                    .Match("(post:Post)", "(user:User)")
                    .Where((Post post) => post.id == liked.post.id)
                    .AndWhere((User user) => user.username == liked.user.username)
                    .Create("(user)-[r:LIKED {time: '" + liked.time + "'}]->(post)");

                query.ExecuteWithoutResults();
            }
            else
            {
                var query = client.Cypher
                    .Match("(post:Post)", "(user:User)")
                    .Where((Post post) => post.id == liked.post.id)
                    .AndWhere((User user) => user.username == disliked.user.username)
                    .Create("(user)-[r:DISLIKED {time: '" + liked.time + "'}]->(post)");

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
        public void CreateCommentRelationships(HasComment hasComment, Commented commented)
        {
            // Firts create comment node
            CreateCommentNode(hasComment.comment);

            var query = client.Cypher
                .Match("(user:User)", "(post:User)", "(comment:Comment)")
                .Where((User user) => user.username == commented.commentator.username)
                .AndWhere((Post post) => post.id == hasComment.post.id)
                .AndWhere((Comment comment) => hasComment.comment.id == comment.id)
                .Create("(post)-[r:HAS_COMMENT {time: '" + hasComment.time + "'}]->(comment)<-[ro:COMMENTED {time: '" + commented.time + "'}]-(user)");

            query.ExecuteWithoutResults();

        }
        #endregion


        #region Get functions

        // Return all users
        public List<User> GetAllUsers()
        {
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:User) and exists(n.id) return n",
                                                            new Dictionary<string, object>(), CypherResultMode.Set);

            List<User> users = ((IRawGraphClient)client).ExecuteGetCypherResults<User>(query).ToList();

            if (users.Count != 0)
                return users;
            else
                return null;
        }

        // Return all posts and their data
        public IEnumerable<Post> GetAllPosts()
        {
            // Getting all posts with all its comments, hashtags, number of likes, dislikes, picture url and time its created
            var queryPosts = client.Cypher
                .Match("(user:User)-[r:POSTED]->(p:Post)")
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

            return queryPosts;
        }

        // Returns user by username (without more data)
        public User GetUser(string username)
        {
            var queryUser = client.Cypher
                .Match("(user:User)", "(post:Post)", "(user2:User)")
                .Where((User user) => user.username == username)
                .OptionalMatch("(user)-[:HAS_PROFILEPICTURE]->(picture:Picture)")
                .With("user, picture, size((user)-[:POSTED]->(post)) as numOfPosts, size((user)-[:FOLLOW]->(user2)) as numOfFollowed, size((user2)-[:FOLLOW]->(user)) as numOfFollowers")
                .Return((user, numOfPosts, numOfFollowed, numOfFollowers, picture) => new User
                {
                    aspid = user.As<User>().aspid,
                    username = user.As<User>().username,
                    numberoffollowed = numOfFollowed.As<int>(),
                    numberoffollowers = numOfFollowers.As<int>(),
                    numberofposts = numOfPosts.As<int>(),
                    profilepictureurl = picture.As<Picture>().url
                })
                .Results;

            return queryUser.FirstOrDefault();
        }

        // Returns post by id
        public Post GetPost(int id)
        {
            return null;
        }

        // Returns all posts of user
        public List<Post> GetUserPosts(string username)
        {

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("username", username);

            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) match (n)-[r:POSTED]->(m) where exists(n.username) and n.username =~ {username} return m",
                                                            queryDict, CypherResultMode.Set);

            List<Post> posts = ((IRawGraphClient)client).ExecuteGetCypherResults<Post>(query).ToList();

            if (posts.Count != 0)
                return posts;
            else
                return null;
        }

        #endregion

        #region Help functions

        // Returns last id from all nodes of specific type (not using)
        public int GetMaxNodeId(object o)
        {
            string type = o.ToString().Split('.').Last();

            var numberOfNodesQuery = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:" + type + ") return count(*)",
                                                            new Dictionary<string, object>(), CypherResultMode.Set);

            int numberOfPosts = ((IRawGraphClient)client).ExecuteGetCypherResults<int>(numberOfNodesQuery).ToList().FirstOrDefault();

            if (numberOfPosts == 0)
                return 0;
            else
            {
                var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:" + type + ") and exists(n.id) return max(n.id)",
                                                                            new Dictionary<string, object>(), CypherResultMode.Set);

                int maxId = ((IRawGraphClient)client).ExecuteGetCypherResults<int>(query).ToList().FirstOrDefault();

                return maxId;
            }
        }

        #endregion
    }
}
