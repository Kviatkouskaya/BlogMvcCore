﻿using BlogMvcCore.DomainModel;
using BlogMvcCore.Services;
using BlogMvcCore.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BlogMvcCore.Controllers
{
    public class UserController : Controller
    {
        private readonly Authentication authentication;
        private readonly UserService userService;
        private readonly PostService postService;
        private readonly CommentService commentService;
        public UserController(Authentication authentication,
                              UserService userService,
                              PostService postService,
                              CommentService commentService)
        {
            this.authentication = authentication;
            this.userService = userService;
            this.postService = postService;
            this.commentService = commentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckRegister(string first, string second, string login,
                                           string password, string repPassword)
        {
            var registrationCheck = authentication.CheckUserRegistration(first, second, login, password, repPassword);
            return registrationCheck ? RedirectToAction("SignIn") : RedirectToAction("Register");
        }

        public IActionResult SignIn()
        {
            return View();
        }

        public new IActionResult SignOut()
        {
            SessionHelper.SetUserAsJson(HttpContext.Session, "user", null);
            authentication.SignOut();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CheckIn(string login, string password)
        {
            var user = authentication.CheckIn(login, password);
            SessionHelper.SetUserAsJson(HttpContext.Session, "user", user);
            return user == null ? RedirectToAction("SignIn") : RedirectToAction("UserPage");
        }

        public IActionResult VisitUserPage(string login)
        {
            return login == SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user").Login ?
                            RedirectToAction("UserPage") : View(userService.VisitUserPage(login));
        }

        public IActionResult UserPage()
        {
            var userLogin = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user").Login;
            return View(userService.VisitUserPage(userLogin));
        }

        public IActionResult ShowUsersList()
        {
            return View(userService.ReturnUsers());
        }

        public IActionResult ViewRecentAddedPostList()
        {
            return View("ViewRecentAddedPostList", postService.ReturnPostList());
        }

        [HttpPost]
        public IActionResult AddPost(string title, string postText, string ownerLogin)
        {
            if (authentication.CheckStringParams(title, postText, ownerLogin))
                postService.AddPost(title, postText, ownerLogin);

            return RedirectToAction("UserPage");
        }

        public IActionResult ViewPostAndComments(long postID)
        {
            return View(postService.GetPostWithComments(postID));
        }

        [HttpPost]
        public IActionResult AddComment(string commentText, long postID, long parentID)
        {
            User user = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user");
            if (authentication.CheckStringParams(commentText))
                commentService.AddComment(commentText, postID, parentID, user);

            return RedirectToAction("ViewPostAndComments", new { postID });
        }
    }
}