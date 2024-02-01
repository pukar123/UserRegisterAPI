﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using HiredFirst.Domain.Repos;

namespace HiredFirstInfrastructure.Repositories.Implementations
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly IMongoCollection<TEntity> _collection;

        public BaseRepository()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("TestMongo");
            string collectionName = GetCollectionName();
            _collection = database.GetCollection<TEntity>(collectionName);
        }
        private string GetCollectionName()
        {
            var collectionNameAttribute = typeof(TEntity)
                .GetTypeInfo()
                .GetCustomAttributes(typeof(CollectionNameAttribute), true)
                .FirstOrDefault() as CollectionNameAttribute;

            if (collectionNameAttribute != null)
            {
                return collectionNameAttribute.Name;
            }

            // Default to entity name if CollectionName attribute is not found
            return typeof(TEntity).Name.ToLower() + "s"; // Example: "users" for TEntity = User
        }

        public IEnumerable<TEntity> GetAll()
        {
            return  _collection.Find(_ => true).ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter)
        {
            return  _collection.Find(filter).ToList();
        }

        public TEntity GetById(string id)
        {
            return  _collection.Find(Builders<TEntity>.Filter.Eq("_id", Guid.Parse(id))).FirstOrDefault();
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task<bool> UpdateAsync(string id, TEntity entity)
        {
            var result = await _collection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", Guid.Parse(id)), entity);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", Guid.Parse(id)));
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
