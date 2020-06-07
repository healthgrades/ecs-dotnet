// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

#if NETSTANDARD
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif
using System;
using System.Runtime.CompilerServices;
using Serilog.Events;
using System.Net;

namespace Elastic.CommonSchema.Serilog
{
	public interface IEcsTextFormatterConfiguration
	{
		bool MapCurrentThread { get; set; }
		Func<Base, LogEvent, Base> MapCustom { get; set; }
		bool MapExceptions { get; set; }
		IHttpAdapter MapHttpAdapter { get; set; }
	}

	public class EcsTextFormatterConfiguration : IEcsTextFormatterConfiguration
	{
		bool IEcsTextFormatterConfiguration.MapExceptions { get; set; } = true;
		bool IEcsTextFormatterConfiguration.MapCurrentThread { get; set; } = true;

		IHttpAdapter IEcsTextFormatterConfiguration.MapHttpAdapter { get; set; }

		Func<Base, LogEvent, Base> IEcsTextFormatterConfiguration.MapCustom { get; set; } = (b, e) => b;

#if NETSTANDARD
        public EcsTextFormatterConfiguration MapHttpContext(IHttpContextAccessor contextAccessor) => Assign(this, contextAccessor, (o, v) => o.MapHttpAdapter
 = new HttpAdapter(v));
		public EcsTextFormatterConfiguration MapHttpContext(IHttpContextAccessor contextAccessor, IPAddress ipAddressOverride) => Assign(this, contextAccessor, ipAddressOverride, (o, v, q) => o.MapHttpAdapter
 = new HttpAdapter(v, q));
#else
		public EcsTextFormatterConfiguration MapHttpContext(HttpContext httpContext) =>
			Assign(this, httpContext, (o, v) => o.MapHttpAdapter = new HttpAdapter(v));
#endif
		public EcsTextFormatterConfiguration MapExceptions(bool value) => Assign(this, value, (o, v) => o.MapExceptions = v);

		public EcsTextFormatterConfiguration MapCurrentThread(bool value) => Assign(this, value, (o, v) => o.MapCurrentThread = v);

		public EcsTextFormatterConfiguration MapCustom(Func<Base, LogEvent, Base> value) => Assign(this, value, (o, v) => o.MapCustom = v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static EcsTextFormatterConfiguration Assign<TValue>(
			EcsTextFormatterConfiguration self, TValue value, Action<IEcsTextFormatterConfiguration, TValue> assign
		)
		{
			assign(self, value);
			return self;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static EcsTextFormatterConfiguration Assign<TValue,TValue2>(
			EcsTextFormatterConfiguration self, TValue value, TValue2 value2, Action<IEcsTextFormatterConfiguration, TValue, TValue2> assign
		)
		{
			assign(self, value, value2);
			return self;
		}
	}
}
