﻿using System;
using System.Linq;
using NUnit.Framework;

namespace WebAPI.OutputCache.MongoDb.Tests
{
    [TestFixture]
    public class AddTests : MongoDbApiOutputCacheTestsBase
    {
        private UserFixture _user;

        [SetUp]
        public void SetUp()
        {
            MongoCollection.RemoveAll();

            _user = new UserFixture { Name = "John", DateOfBirth = new DateTime(1980, 01, 23) };
        }

        [Test]
        public void adds_item_to_cache()
        {
            MongoDbApiOutputCache.Add(_user.Id.ToString(), _user, DateTime.Now.AddSeconds(60));

            var item = MongoCollection.FindAllAs<CachedItem>().FirstOrDefault();

            Assert.That(item, Is.Not.Null);
        }

        [Test]
        public void added_item_stored_with_supplied_key()
        {
            var key = _user.Id.ToString();
            MongoDbApiOutputCache.Add(key, _user, DateTime.Now.AddSeconds(60));

            var item = MongoCollection.FindAllAs<CachedItem>().FirstOrDefault();

            Assert.That(item.Key, Is.EqualTo(key));
        }

        [Test]
        public void added_item_stored_with_expiry()
        {
            var expiration = DateTime.Now.AddSeconds(60);

            MongoDbApiOutputCache.Add(_user.Id.ToString(), _user, expiration);

            var item = MongoCollection.FindOneAs<CachedItem>();

            Assert.That(DateTimeOffset.Compare(item.Expiration, expiration), Is.EqualTo(0));
        }
    }
}