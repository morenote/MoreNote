﻿/**
 *  Thanks for open source!
 *  date：2022-09-03
 *  git : cb4d15273ce2fd914b2317d267d8e366e208cd41
 *  license:https://github.com/ldqk/Masuit.MyBlogs/blob/master/LICENSE 
 *  基于代码版本：b78c483a0dea0d00350bdf44bf788ceb51190e46
 */

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Collections.Pooled;

using EFCoreSecondLevelCacheInterceptor;

using Masuit.Tools.Models;
using Masuit.Tools.Systems;

using Microsoft.EntityFrameworkCore;

using Morenote.Logic.Infrastructure.Repository.Interface;
using Morenote.Models.Models.Entity;

using MoreNote.Logic.Database;
using MoreNote.Logic.Infrastructure.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Z.EntityFramework.Plus;

namespace MoreNote.Logic.Infrastructure.Repository.IMPL
{
	/// <summary>
	/// DAL基类
	/// </summary>
	/// <typeparam name="T">实体类型</typeparam>
	public abstract class BaseRepository<T> : Disposable, IBaseRepository<T> where T : BaseEntity
	{
		public virtual DataContext DataContext { get; set; }

		public MapperConfiguration MapperConfig { get; set; }

		/// <summary>
		/// 获取所有实体
		/// </summary>
		/// <returns>还未执行的SQL语句</returns>
		public virtual IQueryable<T> GetAll()
		{
			return DataContext.Set<T>();
		}

		/// <summary>
		/// 获取所有实体（不跟踪）
		/// </summary>
		/// <returns>还未执行的SQL语句</returns>
		public virtual IQueryable<T> GetAllNoTracking()
		{
			return DataContext.Set<T>().AsNoTracking();
		}

		/// <summary>
		/// 从二级缓存获取所有实体
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<T> GetAllFromCache()
		{
			return DataContext.Set<T>().AsNoTracking().Cacheable().AsEnumerable();
		}

		/// <summary>
		/// 从二级缓存获取所有实体
		/// </summary>
		/// <returns></returns>
		public Task<List<T>> GetAllFromCacheAsync()
		{
			return DataContext.Set<T>().AsNoTracking().Cacheable().ToListAsync();
		}

		/// <summary>
		/// 获取所有实体（不跟踪）
		/// </summary>
		/// <typeparam name="TDto">映射实体</typeparam>
		/// <returns>还未执行的SQL语句</returns>
		public virtual IQueryable<TDto> GetAll<TDto>() where TDto : class
		{
			return DataContext.Set<T>().AsNoTracking().ProjectTo<TDto>(MapperConfig);
		}

		/// <summary>
		/// 从二级缓存获取所有实体（不跟踪）
		/// </summary>
		/// <typeparam name="TDto">映射实体</typeparam>
		/// <returns></returns>
		public virtual PooledList<TDto> GetAllFromCache<TDto>() where TDto : class
		{
			return DataContext.Set<T>().ProjectTo<TDto>(MapperConfig).Cacheable().ToPooledList();
		}

		/// <summary>
		/// 从二级缓存获取所有实体（不跟踪）
		/// </summary>
		/// <typeparam name="TDto">映射实体</typeparam>
		/// <returns></returns>
		public Task<List<TDto>> GetAllFromCacheAsync<TDto>() where TDto : class
		{
			return DataContext.Set<T>().ProjectTo<TDto>(MapperConfig).Cacheable().ToListAsync();
		}

		/// <summary>
		/// 获取所有实体
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>还未执行的SQL语句</returns>
		public virtual IOrderedQueryable<T> GetAll<TS>(Expression<Func<T, TS>> orderby, bool isAsc = true)
		{
			return isAsc ? DataContext.Set<T>().OrderBy(orderby) : DataContext.Set<T>().OrderByDescending(orderby);
		}

		/// <summary>
		/// 获取所有实体（不跟踪）
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>还未执行的SQL语句</returns>
		public virtual IOrderedQueryable<T> GetAllNoTracking<TS>(Expression<Func<T, TS>> orderby, bool isAsc = true)
		{
			return isAsc ? DataContext.Set<T>().AsNoTracking().OrderBy(orderby) : DataContext.Set<T>().AsNoTracking().OrderByDescending(orderby);
		}

		/// <summary>
		/// 从二级缓存获取所有实体
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns></returns>
		public virtual PooledList<T> GetAllFromCache<TS>(Expression<Func<T, TS>> orderby, bool isAsc = true)
		{
			return GetAllNoTracking(orderby, isAsc).Cacheable().ToPooledList();
		}

		/// <summary>
		/// 从二级缓存获取所有实体
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns></returns>
		public Task<List<T>> GetAllFromCacheAsync<TS>(Expression<Func<T, TS>> @orderby, bool isAsc = true)
		{
			return GetAllNoTracking(orderby, isAsc).Cacheable().ToListAsync();
		}

		/// <summary>
		/// 获取所有实体
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <typeparam name="TDto">映射实体</typeparam>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>还未执行的SQL语句</returns>
		public virtual IQueryable<TDto> GetAll<TS, TDto>(Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
		{
			return GetAllNoTracking(orderby, isAsc).ProjectTo<TDto>(MapperConfig);
		}

		/// <summary>
		/// 从二级缓存获取所有实体
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <typeparam name="TDto">映射实体</typeparam>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>还未执行的SQL语句</returns>
		public virtual PooledList<TDto> GetAllFromCache<TS, TDto>(Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
		{
			return GetAll(orderby, isAsc).ProjectTo<TDto>(MapperConfig).Cacheable().ToPooledList();
		}

		/// <summary>
		/// 从二级缓存获取所有实体
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <typeparam name="TDto">映射实体</typeparam>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns></returns>
		public Task<List<TDto>> GetAllFromCacheAsync<TS, TDto>(Expression<Func<T, TS>> @orderby, bool isAsc = true) where TDto : class
		{
			return GetAll(orderby, isAsc).ProjectTo<TDto>(MapperConfig).Cacheable().ToListAsync();
		}

		/// <summary>
		/// 基本查询方法，获取一个集合
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>还未执行的SQL语句</returns>
		public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> where)
		{
			return DataContext.Set<T>().Where(where);
		}

		/// <summary>
		/// 基本查询方法，获取一个集合
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>还未执行的SQL语句</returns>
		public virtual IOrderedQueryable<T> GetQuery<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
		{
			return isAsc ? DataContext.Set<T>().Where(where).OrderBy(orderby) : DataContext.Set<T>().Where(where).OrderByDescending(orderby);
		}

		/// <summary>
		/// 基本查询方法，获取一个集合，优先从二级缓存读取
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns></returns>
		public virtual PooledList<T> GetQueryFromCache(Expression<Func<T, bool>> where)
		{
			return DataContext.Set<T>().Where(where).AsNoTracking().Cacheable().ToPooledList();
		}

		/// <summary>
		/// 基本查询方法，获取一个集合，优先从二级缓存读取
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns></returns>
		public Task<List<T>> GetQueryFromCacheAsync(Expression<Func<T, bool>> @where)
		{
			return DataContext.Set<T>().Where(where).AsNoTracking().Cacheable().ToListAsync();
		}

		/// <summary>
		/// 基本查询方法，获取一个集合，优先从二级缓存读取
		/// </summary>
		/// <typeparam name="TS">排序字段</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序方式</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns></returns>
		public virtual PooledList<T> GetQueryFromCache<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
		{
			return GetQueryNoTracking(where, orderby, isAsc).Cacheable().ToPooledList();
		}

		/// <summary>
		/// 基本查询方法，获取一个集合，优先从二级缓存读取
		/// </summary>
		/// <typeparam name="TS">排序字段</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序方式</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns></returns>
		public Task<List<T>> GetQueryFromCacheAsync<TS>(Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true)
		{
			return GetQueryNoTracking(where, orderby, isAsc).Cacheable().ToListAsync();
		}

		/// <summary>
		/// 基本查询方法，获取一个被AutoMapper映射后的集合，优先从二级缓存读取
		/// </summary>
		/// <typeparam name="TS">排序字段</typeparam>
		/// <typeparam name="TDto">输出类型</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序方式</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns></returns>
		public Task<List<TDto>> GetQueryFromCacheAsync<TS, TDto>(Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true) where TDto : class
		{
			return GetQueryNoTracking(where, orderby, isAsc).ProjectTo<TDto>(MapperConfig).Cacheable().ToListAsync();
		}

		/// <summary>
		/// 基本查询方法，获取一个集合（不跟踪实体）
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>还未执行的SQL语句</returns>
		public virtual IQueryable<T> GetQueryNoTracking(Expression<Func<T, bool>> where)
		{
			return DataContext.Set<T>().Where(where).AsNoTracking();
		}

		/// <summary>
		/// 基本查询方法，获取一个集合（不跟踪实体）
		/// </summary>
		/// <typeparam name="TS">排序字段</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序方式</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>还未执行的SQL语句</returns>
		public virtual IOrderedQueryable<T> GetQueryNoTracking<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
		{
			return isAsc ? DataContext.Set<T>().Where(where).AsNoTracking().OrderBy(orderby) : DataContext.Set<T>().Where(where).AsNoTracking().OrderByDescending(orderby);
		}

		/// <summary>
		/// 基本查询方法，获取一个被AutoMapper映射后的集合（不跟踪实体）
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>还未执行的SQL语句</returns>
		public virtual IQueryable<TDto> GetQuery<TDto>(Expression<Func<T, bool>> where) where TDto : class
		{
			return DataContext.Set<T>().Where(where).AsNoTracking().ProjectTo<TDto>(MapperConfig);
		}

		/// <summary>
		/// 基本查询方法，获取一个被AutoMapper映射后的集合（不跟踪实体）
		/// </summary>
		/// <typeparam name="TS">排序字段</typeparam>
		/// <typeparam name="TDto">输出类型</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序方式</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>还未执行的SQL语句</returns>
		public virtual IQueryable<TDto> GetQuery<TS, TDto>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
		{
			return GetQueryNoTracking(where, orderby, isAsc).ProjectTo<TDto>(MapperConfig);
		}

		/// <summary>
		/// 基本查询方法，获取一个被AutoMapper映射后的集合，优先从二级缓存读取(不跟踪实体)
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>实体集合</returns>
		public virtual PooledList<TDto> GetQueryFromCache<TDto>(Expression<Func<T, bool>> where) where TDto : class
		{
			return DataContext.Set<T>().Where(where).ProjectTo<TDto>(MapperConfig).Cacheable().ToPooledList();
		}

		/// <summary>
		/// 基本查询方法，获取一个被AutoMapper映射后的集合，优先从二级缓存读取
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns></returns>
		public Task<List<TDto>> GetQueryFromCacheAsync<TDto>(Expression<Func<T, bool>> @where) where TDto : class
		{
			return DataContext.Set<T>().Where(where).ProjectTo<TDto>(MapperConfig).Cacheable().ToListAsync();
		}

		/// <summary>
		/// 基本查询方法，获取一个被AutoMapper映射后的集合，优先从二级缓存读取(不跟踪实体)
		/// </summary>
		/// <typeparam name="TS">排序字段</typeparam>
		/// <typeparam name="TDto">输出类型</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序方式</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns></returns>
		public virtual PooledList<TDto> GetQueryFromCache<TS, TDto>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
		{
			return GetQuery(where, orderby, isAsc).ProjectTo<TDto>(MapperConfig).Cacheable().ToPooledList();
		}

		/// <summary>
		/// 获取第一条数据
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>实体</returns>
		public virtual T Get(Expression<Func<T, bool>> where)
		{
			return EF.CompileQuery((DataContext ctx) => ctx.Set<T>().FirstOrDefault(where))(DataContext);
		}

		/// <summary>
		/// 获取第一条数据
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>实体</returns>
		public virtual T GetFromCache(Expression<Func<T, bool>> where)
		{
			return DataContext.Set<T>().Where(where).AsNoTracking().DeferredFirstOrDefault().Execute();
		}

		/// <summary>
		/// 获取第一条数据
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>实体</returns>
		public Task<T> GetFromCacheAsync(Expression<Func<T, bool>> @where)
		{
			return DataContext.Set<T>().Where(where).AsNoTracking().DeferredFirstOrDefault().ExecuteAsync();
		}

		/// <summary>
		/// 获取第一条数据
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>实体</returns>
		public virtual T Get<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
		{
			return isAsc ? EF.CompileQuery((DataContext ctx) => ctx.Set<T>().OrderBy(orderby).FirstOrDefault(where))(DataContext) : EF.CompileQuery((DataContext ctx) => ctx.Set<T>().OrderByDescending(orderby).FirstOrDefault(where))(DataContext);
		}

		/// <summary>
		/// 获取第一条数据
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>实体</returns>
		public virtual T GetFromCache<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
		{
			return isAsc ? DataContext.Set<T>().OrderBy(orderby).Where(where).AsNoTracking().DeferredFirstOrDefault().Execute() : DataContext.Set<T>().OrderByDescending(orderby).Where(where).AsNoTracking().DeferredFirstOrDefault().Execute();
		}

		/// <summary>
		/// 获取第一条数据
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>实体</returns>
		public Task<T> GetFromCacheAsync<TS>(Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true)
		{
			return isAsc ? DataContext.Set<T>().OrderBy(orderby).Where(where).AsNoTracking().DeferredFirstOrDefault().ExecuteAsync() : DataContext.Set<T>().OrderByDescending(orderby).Where(where).AsNoTracking().DeferredFirstOrDefault().ExecuteAsync();
		}

		/// <summary>
		/// 获取第一条被AutoMapper映射后的数据（不跟踪）
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <typeparam name="TDto">映射实体</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>映射实体</returns>
		public Task<TDto> GetAsync<TS, TDto>(Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true) where TDto : class
		{
			return isAsc ? DataContext.Set<T>().Where(where).OrderBy(orderby).AsNoTracking().ProjectTo<TDto>(MapperConfig).FirstOrDefaultAsync() : DataContext.Set<T>().Where(where).OrderByDescending(orderby).AsNoTracking().ProjectTo<TDto>(MapperConfig).FirstOrDefaultAsync();
		}

		/// <summary>
		/// 获取第一条被AutoMapper映射后的数据（不跟踪）
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <typeparam name="TDto">映射实体</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>映射实体</returns>
		public virtual TDto GetFromCache<TS, TDto>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
		{
			return isAsc ? DataContext.Set<T>().Where(where).OrderBy(orderby).ProjectTo<TDto>(MapperConfig).DeferredFirstOrDefault().Execute() : DataContext.Set<T>().Where(where).OrderByDescending(orderby).ProjectTo<TDto>(MapperConfig).DeferredFirstOrDefault().Execute();
		}

		/// <summary>
		/// 获取第一条被AutoMapper映射后的数据
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <typeparam name="TDto">映射实体</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>映射实体</returns>
		public Task<TDto> GetFromCacheAsync<TS, TDto>(Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true) where TDto : class
		{
			return isAsc ? DataContext.Set<T>().Where(where).OrderBy(orderby).ProjectTo<TDto>(MapperConfig).DeferredFirstOrDefault().ExecuteAsync() : DataContext.Set<T>().Where(where).OrderByDescending(orderby).ProjectTo<TDto>(MapperConfig).DeferredFirstOrDefault().ExecuteAsync();
		}

		/// <summary>
		/// 获取第一条数据
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>实体</returns>
		public virtual Task<T> GetAsync(Expression<Func<T, bool>> where)
		{
			return EF.CompileAsyncQuery((DataContext ctx) => ctx.Set<T>().FirstOrDefault(where))(DataContext);
		}

		/// <summary>
		/// 获取第一条数据
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>实体</returns>
		public virtual Task<T> GetAsync<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
		{
			return isAsc ? EF.CompileAsyncQuery((DataContext ctx) => ctx.Set<T>().OrderBy(orderby).FirstOrDefault(where))(DataContext) : EF.CompileAsyncQuery((DataContext ctx) => ctx.Set<T>().OrderByDescending(orderby).FirstOrDefault(where))(DataContext);
		}

		/// <summary>
		/// 获取第一条数据（不跟踪实体）
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>实体</returns>
		public virtual T GetNoTracking(Expression<Func<T, bool>> where)
		{
			return EF.CompileQuery((DataContext ctx) => ctx.Set<T>().AsNoTracking().FirstOrDefault(where))(DataContext);
		}

		/// <summary>
		/// 获取第一条数据（不跟踪实体）
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>实体</returns>
		public virtual T GetNoTracking<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
		{
			return isAsc ? EF.CompileQuery((DataContext ctx) => ctx.Set<T>().AsNoTracking().OrderBy(orderby).FirstOrDefault(where))(DataContext) : EF.CompileQuery((DataContext ctx) => ctx.Set<T>().AsNoTracking().OrderByDescending(orderby).FirstOrDefault(where))(DataContext);
		}

		/// <summary>
		/// 获取第一条被AutoMapper映射后的数据（不跟踪实体）
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>实体</returns>
		public virtual TDto Get<TDto>(Expression<Func<T, bool>> where) where TDto : class
		{
			return DataContext.Set<T>().Where(where).AsNoTracking().ProjectTo<TDto>(MapperConfig).FirstOrDefault();
		}

		/// <summary>
		/// 获取第一条被AutoMapper映射后的数据
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>实体</returns>
		public virtual TDto GetFromCache<TDto>(Expression<Func<T, bool>> where) where TDto : class
		{
			return DataContext.Set<T>().Where(where).ProjectTo<TDto>(MapperConfig).DeferredFirstOrDefault().Execute();
		}

		/// <summary>
		/// 获取第一条被AutoMapper映射后的数据
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>实体</returns>
		public Task<TDto> GetFromCacheAsync<TDto>(Expression<Func<T, bool>> @where) where TDto : class
		{
			return DataContext.Set<T>().Where(where).ProjectTo<TDto>(MapperConfig).DeferredFirstOrDefault().ExecuteAsync();
		}

		/// <summary>
		/// 获取第一条被AutoMapper映射后的数据（不跟踪实体）
		/// </summary>
		/// <typeparam name="TDto">映射实体</typeparam>
		/// <typeparam name="TS">排序</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>实体</returns>
		public virtual TDto Get<TS, TDto>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
		{
			return isAsc ? DataContext.Set<T>().Where(where).OrderBy(orderby).AsNoTracking().ProjectTo<TDto>(MapperConfig).FirstOrDefault() : DataContext.Set<T>().Where(where).OrderByDescending(orderby).AsNoTracking().ProjectTo<TDto>(MapperConfig).FirstOrDefault();
		}

		/// <summary>
		/// 获取第一条数据（异步，不跟踪实体）
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>实体</returns>
		public virtual Task<T> GetNoTrackingAsync(Expression<Func<T, bool>> where)
		{
			return EF.CompileAsyncQuery((DataContext ctx) => ctx.Set<T>().AsNoTracking().FirstOrDefault(@where))(DataContext);
		}

		/// <summary>
		/// 获取第一条数据（异步，不跟踪实体）
		/// </summary>
		/// <typeparam name="TS">排序</typeparam>
		/// <param name="where">查询条件</param>
		/// <param name="orderby">排序字段</param>
		/// <param name="isAsc">是否升序</param>
		/// <returns>实体</returns>
		public virtual Task<T> GetNoTrackingAsync<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
		{
			return isAsc ? EF.CompileAsyncQuery((DataContext ctx) => ctx.Set<T>().OrderBy(orderby).AsNoTracking().FirstOrDefault(where))(DataContext) : EF.CompileAsyncQuery((DataContext ctx) => ctx.Set<T>().OrderByDescending(orderby).AsNoTracking().FirstOrDefault(where))(DataContext);
		}

		/// <summary>
		/// 根据ID找实体
		/// </summary>
		/// <param name="id">实体id</param>
		/// <returns>实体</returns>
		public virtual T GetById(int id)
		{
			return EF.CompileQuery((DataContext ctx, int xid) => ctx.Set<T>().FirstOrDefault(t => t.Id == xid))(DataContext, id);
		}

		/// <summary>
		/// 根据ID找实体(异步)
		/// </summary>
		/// <param name="id">实体id</param>
		/// <returns>实体</returns>
		public virtual Task<T> GetByIdAsync(int id)
		{
			return EF.CompileAsyncQuery((DataContext ctx, int xid) => ctx.Set<T>().FirstOrDefault(t => t.Id == xid))(DataContext, id);
		}

		/// <summary>
		/// 高效分页查询方法
		/// </summary>
		/// <typeparam name="TS"></typeparam>
		/// <param name="pageIndex">第几页</param>
		/// <param name="pageSize">每页大小</param>
		/// <param name="where">where Lambda条件表达式</param>
		/// <param name="orderby">orderby Lambda条件表达式</param>
		/// <param name="isAsc">升序降序</param>
		/// <returns></returns>
		public virtual PagedList<T> GetPages<TS>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc)
		{
			return isAsc ? DataContext.Set<T>().Where(where).NotCacheable().OrderBy(orderby).ToPagedList(pageIndex, pageSize) : DataContext.Set<T>().Where(where).NotCacheable().OrderByDescending(orderby).ToPagedList(pageIndex, pageSize);
		}

		/// <summary>
		/// 高效分页查询方法
		/// </summary>
		/// <typeparam name="TS"></typeparam>
		/// <param name="pageIndex">第几页</param>
		/// <param name="pageSize">每页大小</param>
		/// <param name="where">where Lambda条件表达式</param>
		/// <param name="orderby">orderby Lambda条件表达式</param>
		/// <param name="isAsc">升序降序</param>
		/// <returns></returns>
		public virtual Task<PagedList<T>> GetPagesAsync<TS>(int pageIndex, int pageSize, Expression<Func<T, bool>> @where, Expression<Func<T, TS>> orderby, bool isAsc)
		{
			return isAsc ? DataContext.Set<T>().Where(where).NotCacheable().OrderBy(orderby).ToPagedListAsync(pageIndex, pageSize) : DataContext.Set<T>().Where(where).NotCacheable().OrderByDescending(orderby).ToPagedListAsync(pageIndex, pageSize);
		}

		/// <summary>
		/// 高效分页查询方法，优先从二级缓存读取
		/// </summary>
		/// <typeparam name="TS"></typeparam>
		/// <param name="pageIndex">第几页</param>
		/// <param name="pageSize">每页大小</param>
		/// <param name="where">where Lambda条件表达式</param>
		/// <param name="orderby">orderby Lambda条件表达式</param>
		/// <param name="isAsc">升序降序</param>
		/// <returns></returns>
		public virtual PagedList<T> GetPagesFromCache<TS>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc)
		{
			var temp = DataContext.Set<T>().Where(where).AsNoTracking();
			return isAsc ? temp.OrderBy(orderby).ToCachedPagedList(pageIndex, pageSize) : temp.OrderByDescending(orderby).ToCachedPagedList(pageIndex, pageSize);
		}

		/// <summary>
		/// 高效分页查询方法，优先从二级缓存读取
		/// </summary>
		/// <typeparam name="TS"></typeparam>
		/// <param name="pageIndex">第几页</param>
		/// <param name="pageSize">每页大小</param>
		/// <param name="where">where Lambda条件表达式</param>
		/// <param name="orderby">orderby Lambda条件表达式</param>
		/// <param name="isAsc">升序降序</param>
		/// <returns></returns>
		public virtual Task<PagedList<T>> GetPagesFromCacheAsync<TS>(int pageIndex, int pageSize, Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc)
		{
			var temp = DataContext.Set<T>().Where(where).AsNoTracking();
			return isAsc ? temp.OrderBy(orderby).ToCachedPagedListAsync(pageIndex, pageSize) : temp.OrderByDescending(orderby).ToCachedPagedListAsync(pageIndex, pageSize);
		}

		/// <summary>
		/// 高效分页查询方法，优先从二级缓存读取，取出被AutoMapper映射后的数据集合
		/// </summary>
		/// <typeparam name="TS"></typeparam>
		/// <typeparam name="TDto"></typeparam>
		/// <param name="pageIndex">第几页</param>
		/// <param name="pageSize">每页大小</param>
		/// <param name="where">where Lambda条件表达式</param>
		/// <param name="orderby">orderby Lambda条件表达式</param>
		/// <param name="isAsc">升序降序</param>
		/// <returns></returns>
		public Task<PagedList<TDto>> GetPagesFromCacheAsync<TS, TDto>(int pageIndex, int pageSize, Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc) where TDto : class
		{
			var temp = DataContext.Set<T>().Where(where).AsNoTracking();
			return isAsc ? temp.OrderBy(orderby).ToCachedPagedListAsync<T, TDto>(pageIndex, pageSize, MapperConfig) : temp.OrderByDescending(orderby).ToCachedPagedListAsync<T, TDto>(pageIndex, pageSize, MapperConfig);
		}

		/// <summary>
		/// 高效分页查询方法（不跟踪实体）
		/// </summary>
		/// <typeparam name="TS"></typeparam>
		/// <param name="pageIndex">第几页</param>
		/// <param name="pageSize">每页大小</param>
		/// <param name="where">where Lambda条件表达式</param>
		/// <param name="orderby">orderby Lambda条件表达式</param>
		/// <param name="isAsc">升序降序</param>
		/// <returns></returns>
		public virtual PagedList<T> GetPagesNoTracking<TS>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
		{
			return isAsc ? DataContext.Set<T>().Where(where).AsNoTracking().NotCacheable().OrderBy(orderby).ToPagedList(pageIndex, pageSize) : DataContext.Set<T>().Where(where).AsNoTracking().NotCacheable().OrderByDescending(orderby).ToPagedList(pageIndex, pageSize);
		}

		/// <summary>
		/// 高效分页查询方法，取出被AutoMapper映射后的数据集合（不跟踪实体）
		/// </summary>
		/// <typeparam name="TS"></typeparam>
		/// <typeparam name="TDto"></typeparam>
		/// <param name="pageIndex">第几页</param>
		/// <param name="pageSize">每页大小</param>
		/// <param name="where">where Lambda条件表达式</param>
		/// <param name="orderby">orderby Lambda条件表达式</param>
		/// <param name="isAsc">升序降序</param>
		/// <returns></returns>
		public virtual PagedList<TDto> GetPages<TS, TDto>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
		{
			return isAsc ? DataContext.Set<T>().Where(where).AsNoTracking().OrderBy(orderby).ToPagedList<T, TDto>(pageIndex, pageSize, MapperConfig) : DataContext.Set<T>().Where(where).AsNoTracking().OrderByDescending(orderby).ToPagedList<T, TDto>(pageIndex, pageSize, MapperConfig);
		}

		/// <summary>
		/// 高效分页查询方法，取出被AutoMapper映射后的数据集合
		/// </summary>
		/// <typeparam name="TS"></typeparam>
		/// <typeparam name="TDto"></typeparam>
		/// <param name="pageIndex">第几页</param>
		/// <param name="pageSize">每页大小</param>
		/// <param name="where">where Lambda条件表达式</param>
		/// <param name="orderby">orderby Lambda条件表达式</param>
		/// <param name="isAsc">升序降序</param>
		/// <returns></returns>
		public Task<PagedList<TDto>> GetPagesAsync<TS, TDto>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc) where TDto : class
		{
			return isAsc ? DataContext.Set<T>().Where(where).AsNoTracking().OrderBy(orderby).ToPagedListAsync<T, TDto>(pageIndex, pageSize, MapperConfig) : DataContext.Set<T>().Where(where).AsNoTracking().OrderByDescending(orderby).ToPagedListAsync<T, TDto>(pageIndex, pageSize, MapperConfig);
		}

		/// <summary>
		/// 高效分页查询方法，取出被AutoMapper映射后的数据集合，优先从缓存读取（不跟踪实体）
		/// </summary>
		/// <typeparam name="TS"></typeparam>
		/// <typeparam name="TDto"></typeparam>
		/// <param name="pageIndex">第几页</param>
		/// <param name="pageSize">每页大小</param>
		/// <param name="where">where Lambda条件表达式</param>
		/// <param name="orderby">orderby Lambda条件表达式</param>
		/// <param name="isAsc">升序降序</param>
		/// <returns></returns>
		public virtual PagedList<TDto> GetPagesFromCache<TS, TDto>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
		{
			var temp = DataContext.Set<T>().Where(where).AsNoTracking();
			return isAsc ? temp.OrderBy(orderby).ToCachedPagedList<T, TDto>(pageIndex, pageSize, MapperConfig) : temp.OrderByDescending(orderby).ToCachedPagedList<T, TDto>(pageIndex, pageSize, MapperConfig);
		}

		/// <summary>
		/// 根据ID删除实体
		/// </summary>
		/// <param name="id">实体id</param>
		/// <returns>删除成功</returns>
		public virtual bool DeleteById(int id)
		{
			return DataContext.Set<T>().Where(t => t.Id == id).Delete() > 0;
		}

		/// <summary>
		/// 根据ID删除实体
		/// </summary>
		/// <param name="id">实体id</param>
		/// <returns>删除成功</returns>
		public virtual Task<int> DeleteByIdAsync(int id)
		{
			return DataContext.Set<T>().Where(t => t.Id == id).DeleteAsync();
		}

		/// <summary>
		/// 删除实体
		/// </summary>
		/// <param name="t">需要删除的实体</param>
		/// <returns>删除成功</returns>
		public virtual bool DeleteEntity(T t)
		{
			DataContext.Entry(t).State = EntityState.Unchanged;
			DataContext.Entry(t).State = EntityState.Deleted;
			DataContext.Remove(t);
			return true;
		}

		/// <summary>
		/// 根据条件删除实体
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>删除成功</returns>
		public virtual int DeleteEntity(Expression<Func<T, bool>> where)
		{
			return DataContext.Set<T>().Where(where).Delete();
		}

		/// <summary>
		/// 根据条件删除实体（异步）
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>删除成功</returns>
		public virtual Task<int> DeleteEntityAsync(Expression<Func<T, bool>> where)
		{
			return DataContext.Set<T>().Where(where).DeleteAsync();
		}

		/// <summary>
		/// 添加实体
		/// </summary>
		/// <param name="t">需要添加的实体</param>
		/// <returns>添加成功</returns>
		public T AddEntity(T t)
		{
			DataContext.Add(t);
			return t;
		}

		///// <summary>
		///// 添加或更新实体
		///// </summary>
		///// <param name="key">更新键规则</param>
		///// <param name="t">需要保存的实体</param>
		///// <returns>保存成功</returns>
		//public T AddOrUpdate<TKey>(Expression<Func<T, TKey>> key, T t)
		//{
		//    DataContext.Set<T>().AddOrUpdate(key, t);
		//    return t;
		//}

		///// <summary>
		///// 添加或更新实体
		///// </summary>
		///// <param name="key">更新键规则</param>
		///// <param name="entities">需要保存的实体</param>
		///// <returns>保存成功</returns>
		//public void AddOrUpdate<TKey>(Expression<Func<T, TKey>> key, params T[] entities)
		//{
		//    DataContext.Set<T>().AddOrUpdate(key, entities);
		//}

		/// <summary>
		/// 添加或更新实体
		/// </summary>
		/// <param name="key">更新键规则</param>
		/// <param name="entities">需要保存的实体</param>
		/// <returns>保存成功</returns>
		//public void AddOrUpdate<TKey>(Expression<Func<T, TKey>> key, IEnumerable<T> entities)
		//{
		//    DataContext.Set<T>().AddOrUpdate(key, entities);
		//}

		/// <summary>
		/// 统一保存数据
		/// </summary>
		/// <returns>受影响的行数</returns>
		public virtual int SaveChanges()
		{
			return DataContext.SaveChanges();
		}

		/// <summary>
		/// 统一保存数据（异步）
		/// </summary>
		/// <returns>受影响的行数</returns>
		public virtual Task<int> SaveChangesAsync()
		{
			return DataContext.SaveChangesAsync();
		}

		/// <summary>
		/// 判断实体是否在数据库中存在
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>是否存在</returns>
		public virtual bool Any(Expression<Func<T, bool>> where)
		{
			return EF.CompileQuery((DataContext ctx) => ctx.Set<T>().Any(where))(DataContext);
		}

		/// <summary>
		/// 符合条件的个数
		/// </summary>
		/// <param name="where">查询条件</param>
		/// <returns>是否存在</returns>
		public virtual int Count(Expression<Func<T, bool>> where)
		{
			return EF.CompileQuery((DataContext ctx) => ctx.Set<T>().Count(where))(DataContext);
		}

		/// <summary>
		/// 删除多个实体
		/// </summary>
		/// <param name="list">实体集合</param>
		/// <returns>删除成功</returns>
		public virtual bool DeleteEntities(IEnumerable<T> list)
		{
			DataContext.RemoveRange(list);
			return true;
		}

		/// <summary>
		/// 添加多个实体
		/// </summary>
		/// <param name="list">实体集合</param>
		/// <returns>添加成功</returns>
		public virtual IEnumerable<T> AddEntities(IList<T> list)
		{
			DataContext.AddRange(list);
			return list;
		}

		public override void Dispose(bool disposing)
		{
			//DataContext?.Dispose();
			//DataContext = null;
		}

		public T AddOrUpdate<TKey>(Expression<Func<T, TKey>> key, T t)
		{
			throw new NotImplementedException();
		}

		public void AddOrUpdate<TKey>(Expression<Func<T, TKey>> key, params T[] entities)
		{
			throw new NotImplementedException();
		}

		public void AddOrUpdate<TKey>(Expression<Func<T, TKey>> key, IEnumerable<T> entities)
		{
			throw new NotImplementedException();
		}

		public T this[int id] => GetById(id);
	}
}
