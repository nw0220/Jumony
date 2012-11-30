﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Ivony.Html
{

  /// <summary>
  /// CSS属性选择器
  /// </summary>
  /// <remarks>
  /// 此类型实例是线程安全的
  /// </remarks>
  public sealed class CssAttributeSelector
  {

    private readonly string name;
    private readonly string comparison;
    private readonly string value;


    private delegate bool ValueMatcher( string exp, string value );

    private static readonly Dictionary<string, ValueMatcher> matchers = new Dictionary<string, ValueMatcher>()
      {
        { "^=", ( exp, value ) => value != null && value.StartsWith( exp, StringComparison.Ordinal ) },
        { "$=", ( exp, value ) => value != null && value.EndsWith( exp, StringComparison.Ordinal ) },
        { "*=", ( exp, value ) => value != null && value.Contains( exp ) },
        { "~=", ( exp, value ) => value != null && Regulars.whiteSpaceSeparatorRegex.Split( value ).Contains( exp,StringComparer.Ordinal ) },
        { "!=", ( exp, value ) => value != exp },
        { "=",  ( exp, value ) => value == exp }
      };




    public CssAttributeSelector( string name, string comparison, string value )
    {
      this.name = name;
      this.comparison = comparison;
      this.value = value;
    }


    /// <summary>
    /// 检查元素是否符合选择条件
    /// </summary>
    /// <param name="element">要检查的元素</param>
    /// <returns>是否符合条件</returns>
    public bool IsEligible( IHtmlElement element )
    {

      string _value = null;

      var attribute = element.Attribute( name );

      if ( comparison == null )//如果没有运算符，那么表示判断属性是否存在
        return attribute != null;

      if ( attribute != null )
        _value = attribute.AttributeValue;

      return matchers[comparison]( value, _value );
    }


    /// <summary>
    /// 返回表示当前选择器的表达式
    /// </summary>
    /// <returns>表示当前选择器的表达式</returns>
    public override string ToString()
    {
      if ( comparison != null )
        return string.Format( CultureInfo.InvariantCulture, "[{0}{1}'{2}']", name, comparison, value.Replace( "'", "\\'" ).Replace( "\"", "\\\"" ) );
      else
        return string.Format( CultureInfo.InvariantCulture, "[{0}]", name );
    }

  }
}
