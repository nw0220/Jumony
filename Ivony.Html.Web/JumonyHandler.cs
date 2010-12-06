﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.IO;
using Ivony.Fluent;

namespace Ivony.Html.Web
{
  public abstract class JumonyHandler : IHttpHandler, IRequiresSessionState
  {

    public bool IsReusable
    {
      get { return false; }
    }



    protected MapInfo MapperResult
    {
      get;
      private set;
    }


    /// <summary>
    /// 派生类重写此方法处理文档
    /// </summary>
    protected abstract void ProcessDocument();


    void IHttpHandler.ProcessRequest( HttpContext context )
    {
      Context = context;

      MapperResult = Context.GetMapperResult();

      if ( MapperResult == null )
      {
        Trace.Warn( "Core", "origin url is not found." );

        var builder = new UriBuilder( Request.Url );
        var path = builder.Path;

        if ( !path.EndsWith( ".ashx" ) )
          throw new InvalidOperationException();

        builder.Path = path.Remove( path.Length - 5 );

        Trace.Warn( "Core", "redirect to template vitual path." );
        Response.Redirect( builder.Uri.AbsoluteUri );
      }


      OnPreLoadDocument();

      Trace.Write( "Core", "Begin Load Document" );
      var document = MapperResult.LoadTemplate();
      Trace.Write( "Core", "End Load Document" );

      OnPostLoadDocument();



      OnPreProcessDocument();

      Trace.Write( "Core", "Begin Process Document" );
      ProcessDocument();
      Trace.Write( "Core", "End Process Document" );

      OnPostProcessDocument();

      AddGeneratorMetaData();

      OnPreRender();

      Trace.Write( "Core", "Begin Render Document" );
      RenderDocument();
      Trace.Write( "Core", "End Render Document" );

      OnPostReander();
    }




    private void AddGeneratorMetaData()
    {
      var factory = Document.GetNodeFactory();
      if ( factory != null )
      {
        var meta = factory.CreateElement( "meta" );
        meta.SetAttribute( "name" ).Value( "generator" );
        meta.SetAttribute( "content" ).Value( "Jumony" );

        var header = Document.Find( "html head" ).FirstOrDefault();

        if ( header != null )
          meta.InsertTo( header, 0 );

      }
    }


    protected virtual void RenderDocument()
    {
      Response.Write( Document.InnerHtml( false ) );
    }


    /// <summary>
    /// 获取正在处理的页面文档
    /// </summary>
    public IHtmlDocument Document
    {
      get;
      private set;
    }


    /// <summary>
    /// 在文档范围类使用选择器查找符合要求的元素
    /// </summary>
    /// <param name="selector">CSS选择器</param>
    /// <returns>符合选择器要求的元素</returns>
    protected IEnumerable<IHtmlElement> Find( string selector )
    {
      return Document.Find( selector );
    }

    /// <summary>
    /// 在文档范围类使用选择器查找符合要求的元素
    /// </summary>
    /// <param name="selectors">多个CSS选择器，结果会合并</param>
    /// <returns>符合选择器要求的元素</returns>
    protected IEnumerable<IHtmlElement> Find( params string[] selectors )
    {
      return Document.Find( selectors );
    }



    /// <summary>
    /// 获取页面模板（即HTML静态页）
    /// </summary>
    /// <returns></returns>
    protected virtual IHtmlDocument LoadTemplate()
    {


      var document = MapperResult.LoadTemplate();


      return document;
    }


    /// <summary>
    /// 获取与该页关联的 HttpContext 对象。
    /// </summary>
    public HttpContext Context
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取请求的页的 HttpRequest 对象
    /// </summary>
    protected HttpRequest Request
    {
      get { return Context.Request; }
    }


    /// <summary>
    /// 获取与该 Page 对象关联的 HttpResponse 对象。该对象使您得以将 HTTP 响应数据发送到客户端，并包含有关该响应的信息
    /// </summary>
    protected HttpResponse Response
    {
      get { return Context.Response; }
    }


    /// <summary>
    /// 获取 Server 对象，它是 HttpServerUtility 类的实例
    /// </summary>
    protected HttpServerUtility Server
    {
      get { return Context.Server; }
    }


    /// <summary>
    /// 为当前 Web 请求获取 HttpApplicationState 对象
    /// </summary>
    protected HttpApplicationState Application
    {
      get { return Context.Application; }
    }


    /// <summary>
    /// 为当前 Web 请求获取 TraceContext 对象
    /// </summary>
    protected TraceContext Trace
    {
      get { return Context.Trace; }
    }


    /// <summary>
    /// 获取与该页驻留的应用程序关联的 Cache 对象
    /// </summary>
    protected System.Web.Caching.Cache Cache
    {
      get { return Context.Cache; }
    }


    /// <summary>
    /// 返回与 Web 服务器上的指定虚拟路径相对应的物理文件路径
    /// </summary>
    /// <param name="virtualPath">Web 服务器的虚拟路径</param>
    /// <returns>与 path 相对应的物理文件路径</returns>
    public string MapPath( string virtualPath )
    {
      return Server.MapPath( virtualPath );
    }



    public event EventHandler PreLoadDocument;
    public event EventHandler PostLoadDocument;

    protected virtual void OnPreLoadDocument() { if ( PreLoadDocument != null ) PreLoadDocument( this, EventArgs.Empty ); }
    protected virtual void OnPostLoadDocument() { if ( PostLoadDocument != null ) PostLoadDocument( this, EventArgs.Empty ); }


    public event EventHandler PreParseDocument;
    public event EventHandler PostParseDocument;

    protected virtual void OnPreParseDocument() { if ( PreParseDocument != null ) PreParseDocument( this, EventArgs.Empty ); }
    protected virtual void OnPostParseDocument() { if ( PostParseDocument != null ) PostParseDocument( this, EventArgs.Empty ); }


    public event EventHandler PreProcessDocument;
    public event EventHandler PostProcessDocument;

    protected virtual void OnPreProcessDocument() { if ( PreProcessDocument != null ) PreProcessDocument( this, EventArgs.Empty ); }
    protected virtual void OnPostProcessDocument() { if ( PostProcessDocument != null ) PostProcessDocument( this, EventArgs.Empty ); }


    public event EventHandler PreRender;
    public event EventHandler PostReander;

    protected virtual void OnPreRender() { if ( PreRender != null ) PreRender( this, EventArgs.Empty ); }
    protected virtual void OnPostReander() { if ( PostReander != null ) PostReander( this, EventArgs.Empty ); }





    #region IDisposable 成员

    public void Dispose()
    {

    }

    #endregion

  }
}
