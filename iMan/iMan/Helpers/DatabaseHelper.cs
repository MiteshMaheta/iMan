﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using iMan.Data;
using SQLite;

namespace iMan.Helpers
{
    public class DatabaseHelper
    {
        public async Task ExecuteQuery(string query)
        {
            if (App.Connection != null)
            {
                try
                {
                    await App.Connection.ExecuteScalarAsync<bool>(query);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<List<Info<string>>> GetScriptsLoaded(string value = "script")
        {
            if (App.Connection != null)
            {
                try
                {
                    List<Info<string>> info = await App.Connection.QueryAsync<Info<string>>($"select * from info where value = '{value}'");
                    return info;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return new List<Info<string>>();
        }

        public async Task SaveInfo<T>(string key, T value) where T : class
        {
            if (App.Connection != null)
            {
                try
                {
                    string stringVal;
                    if (typeof(T) != typeof(string))
                        stringVal = JsonConvert.SerializeObject(value).ToString();
                    else
                        stringVal = value.ToString();
                    await App.Connection.ExecuteAsync("insert into info values (?,?)", key, stringVal);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public async Task<T> GetInfo<T>(string key) where T : class
        {
            if (App.Connection != null)
            {
                List<Info<T>> info = await App.Connection.QueryAsync<Info<T>>($"select * from info where key == '{key}'");
                if (info != null && info.Count > 0)
                    return info.FirstOrDefault().value;
            }
            return null;
        }

        public async Task<int> GetTableCount()
        {
            if (App.Connection != null)
            {
                return await App.Connection.ExecuteScalarAsync<int>("select count(*) from sqlite_master");
            }
            else
                return 0;
        }

        public async Task<List<Product>> GetAllProducts(string category="",int start=0)
        {
            if (App.Connection != null)
            {
                string query = $"select * from product";
                if (!string.IsNullOrEmpty(category))
                {
                    query += $" where category='{category}'";
                    query += $" order by id desc limit {start},10";
                }
                try
                {
                    List<Product> res = await App.Connection.QueryAsync<Product>(query);
                    //await App.Connection.CloseAsync();
                    return res;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return new List<Product>();
        }
        
        public async Task<int> SaveProduct(Product product)
        {
            try
            {
                if (App.Connection != null)
                {
                    int res = 0;
                    if (string.IsNullOrEmpty(product.Id))
                        res = await App.Connection.InsertAsync(product);
                    else
                        res = await App.Connection.UpdateAsync(product);
                    //await App.Connection.CloseAsync();
                    return res;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return -1;
        }

        public async Task<int> DeleteProduct(int id)
        {
            if (App.Connection != null)
            {
                string query = $"delete from product where id = {id}";
                await App.Connection.ExecuteAsync(query);
                string itemQuery = $"delete from itemsUsed where productId= {id}";
                return await App.Connection.ExecuteAsync(itemQuery);
            }
            return await Task.FromResult(0);
        }

        public async Task<int> SaveItem(Item item)
        {
            try
            {
                if (App.Connection != null)
                {
                    int add = 0;
                    if (string.IsNullOrEmpty(item.Id))
                    {
                        add = await App.Connection.InsertAsync(item);
                    }
                    else
                    {
                        Item prevItem = await App.Connection.GetAsync<Item>(item.Id);
                        add = await App.Connection.UpdateAsync(item);
                        if (prevItem.Rate != item.Rate)
                        {
                            string updateItemUsed = $"update itemsUsed set price = {item.Rate}, total = quantity * {item.Rate} where itemId == {item.Id}";
                            int res = await App.Connection.ExecuteAsync(updateItemUsed);
                            if(res>0)
                            {
                                List<ItemUsed> ids = await App.Connection.QueryAsync<ItemUsed>($"select productId from itemsused where itemId == {item.Id}");
                                if (ids != null && ids.Count > 0)
                                {
                                    foreach (string id in ids.Select(e=>e.ProductId))
                                    {
                                        string productUpdate = $"update product set costPrice =  (select sum(total) from itemsUsed where productId == {id})," +
                                            $" sellingPrice = (select sum(total) from itemsUsed where productId =={id})*100/(100 - profitPercent) where id = {id}";
                                        await App.Connection.ExecuteAsync(productUpdate);
                                    }
                                }
                            }

                        }
                    }
                   // await App.Connection.CloseAsync();
                    return add;
                }
            }
            catch (Exception ex)
            {

            }
            return -1;
        }

        public async Task<List<Item>> GetAllItems( string category, bool isAscending = true)
        {
            if (App.Connection != null)
            {
                string query = $"select * from item where categoryId = '{category}'";
                if (!isAscending)
                {
                    query += " order by id desc ";
                }
                try
                {
                    List<Item> items = await App.Connection.QueryAsync<Item>(query);
                    //await App.Connection.CloseAsync();
                    return items;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return null;
        }

        public async Task<int> DeleteItem(int itemId)
        {
            if (App.Connection != null)
            {
                string query = $"delete from itemsUsed where itemId = {itemId}";
                await App.Connection.ExecuteAsync(query);
                string itemQuery = $"delete from item where id= {itemId}";
                return await App.Connection.ExecuteAsync(itemQuery);
            }
            return await Task.FromResult(0);
        }


        public async Task<int> SaveCategory(Category category)
        {
            try
            {
                if (App.Connection != null)
                {
                    int add = await App.Connection.InsertAsync(category);
                    //await App.Connection.CloseAsync();
                    return add;
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return -1;
        }

        public async Task<List<Category>> GetAllCategory(String categoryId ="",bool isAscending = true)
        {
            if (App.Connection != null)
            {
                string query = "select * from category";
                if (!string.IsNullOrEmpty(categoryId))
                {
                    query += $" where id='{categoryId}' ";
                }
                if (!isAscending)
                {
                    query +=" order by id desc ";
                }
                try
                {
                    List<Category> categories = await App.Connection.QueryAsync<Category>(query);
                    //await App.Connection.CloseAsync();
                    return categories;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return null;
        }

        public async Task<int> DeleteCategory(int categoryId)
        {
            if (App.Connection != null)
            {
                List<Product> productList = await GetAllProducts(categoryId.ToString());

                List<string> productIds = new List<string>();
                foreach (var product in productList)
                {
                    Xamarin.Forms.DependencyService.Get<IFileHelper>().DeleteFile(product.OriginalImgSource);
                    Xamarin.Forms.DependencyService.Get<IFileHelper>().DeleteFile(product.CompressImgSource);
                    productIds.Add(product.Id);
                }
                
                string query = $"delete from itemsUsed where productId in ({String.Join(",",productIds)})";
                await App.Connection.ExecuteAsync(query);
                string itemQuery = $"delete from item where categoryId= {categoryId}";
                await App.Connection.ExecuteAsync(itemQuery);

                string productQuery = $"delete from product where category= {categoryId}";
                await App.Connection.ExecuteAsync(productQuery);

                string categoryQuery = $"delete from category where id= {categoryId}";
                return await App.Connection.ExecuteAsync(categoryQuery);
            }
            return await Task.FromResult(0);
        }

        public async Task<int> SaveParty(Party party)
        {
            try
            {
                if (App.Connection != null)
                {
                    int add = await App.Connection.InsertAsync(party);
                    //await App.Connection.CloseAsync();
                    return add;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return -1;
        }

        public async Task<List<Party>> GetAllParty()
        {
            if (App.Connection != null)
            {
                string query = "select * from party";
                try
                {
                    List<Party> parties = await App.Connection.QueryAsync<Party>(query);
                    return parties;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return null;
        }

        public async Task<int> SaveItemUsed(ItemUsed itemUsed)
        {
            try
            {
                if (App.Connection != null)
                {
                    int res = 0;
                    if (string.IsNullOrEmpty(itemUsed.Id))
                        res = await App.Connection.InsertAsync(itemUsed);
                    else
                        res = await App.Connection.UpdateAsync(itemUsed);
                    //await App.Connection.CloseAsync();
                    return res;
                }
            }
            catch (Exception ex)
            {

            }
            return -1;
        }

        public async Task<List<ItemUsed>> GetAllItemUsed(string productId = "")
        {
            if (App.Connection != null)
            {
                string query = $"select * from itemsUsed";
                if (!string.IsNullOrEmpty(productId))
                    query += $" where productId='{productId}'";
                try
                {
                    List<ItemUsed> res = await App.Connection.QueryAsync<ItemUsed>(query);
                    //await App.Connection.CloseAsync();
                    return res;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return null;
        }

        

        
    }
}
