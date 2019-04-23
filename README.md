# MakeMyDayProject
Simple social network (like Twitter) created in ASP.NET MVC 5 with Redis and Neo4j as databases. Redis is used as cache and Neo4j is used to store users, posts, messages, comments, pictures (pictures are saved as path on server to them), hashtags.

> For all suggestions please feel free to contact me :)

## Main page
- Basically like on twitter user is able to upload posts with text, pictures, tags and hashtags. All posts are ordered by post time and every new post is dynamically added on top. On main page there are all new uploaded posts. Latest 10 new posts are in redis db for faster main page loading.

![Screenshot_6](https://user-images.githubusercontent.com/36667491/56597949-ce0e1b00-65f3-11e9-93e0-95ff72abbcfb.png)

## Posts
- Every user can like, dislike and comment posts. Only creator of posts can delete post, this also refers to comments.

![Screenshot_9](https://user-images.githubusercontent.com/36667491/56598438-c8650500-65f4-11e9-86fd-732d6653c9cc.png)

## Profile page
- On profile page of user is shown all its uploaded posts. Also you can follow and message that user.

![Screenshot_12](https://user-images.githubusercontent.com/36667491/56598783-7cff2680-65f5-11e9-9e2d-2ec718893274.png)

## Messages
- In navbar there is notification how many new messages user has. I've used redis sorted set for that. Where key is user that sends message and score is how many messages it sended.

![Screenshot_15](https://user-images.githubusercontent.com/36667491/56599010-eb43e900-65f5-11e9-825b-3ecfa1a71ea4.png)

- Messages are displayed as simple chat box. And everythime new message is sent, it's dinamicaly added in chat box.

![Screenshot_16](https://user-images.githubusercontent.com/36667491/56599148-483f9f00-65f6-11e9-8fbf-40dd277abf34.png)

## Hashtags
- Every post can have hashtags, and because of that it's possible to show all posts for some specific hashtag. It's possible to click on hashtag in post and it will open new page with all posts with that hashtag.

![Screenshot_18](https://user-images.githubusercontent.com/36667491/56599550-1418ae00-65f7-11e9-9e9c-748179de1e6d.png)
