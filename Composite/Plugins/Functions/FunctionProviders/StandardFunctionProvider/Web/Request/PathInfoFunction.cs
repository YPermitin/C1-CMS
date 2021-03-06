﻿using System;
using System.Collections.Generic;
using Composite.Core.Extensions;
using Composite.Core.Routing.Pages;
using Composite.Functions;
using Composite.Plugins.Functions.FunctionProviders.StandardFunctionProvider.Foundation;

namespace Composite.Plugins.Functions.FunctionProviders.StandardFunctionProvider.Web.Request
{
    internal sealed class PathInfoFunction : StandardFunctionBase
    {
        public PathInfoFunction(EntityTokenFactory entityTokenFactory)
            : base("PathInfo", "Composite.Web.Request", typeof(string), entityTokenFactory)
        {
        }

        protected override IEnumerable<StandardFunctionParameterProfile> StandardFunctionParameterProfiles
        {
            get
            {
                WidgetFunctionProvider segmentDropDown = StandardWidgetFunctions.DropDownList(
                    typeof(PathInfoFunction), "SegmentSelectorOptionsFull", "Key", "Value", false, true);

                yield return new StandardFunctionParameterProfile(
                    "Segment", typeof(int), true, new ConstantValueProvider("-1"), segmentDropDown);

                yield return new StandardFunctionParameterProfile(
                    "AutoApprove", typeof(bool), false, new ConstantValueProvider(true), StandardWidgetFunctions.CheckBoxWidget);

                yield return new StandardFunctionParameterProfile(
                    "FallbackValue", typeof(string), false, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);
            }
        }


        public override object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            int segment = (int)parameters.GetParameter("Segment");
            bool autoApprove = (bool)parameters.GetParameter("AutoApprove");

            string result = GetPathInfoSegment(segment);

            if (autoApprove && !result.IsNullOrEmpty())
            {
                C1PageRoute.RegisterPathInfoUsage();
            }

            return result ?? parameters.GetParameter<string>("FallbackValue");
        }

        public static IEnumerable<KeyValuePair<int, string>> SegmentSelectorOptions()
        {
            return new[]
                       {
                           new KeyValuePair<int, string>(0, "0 "), // Additional space is intentional 
                           new KeyValuePair<int, string>(1, "1"),
                           new KeyValuePair<int, string>(2, "2"),
                           new KeyValuePair<int, string>(3, "3"),
                           new KeyValuePair<int, string>(4, "4"),
                           new KeyValuePair<int, string>(5, "5")
                       };
        }

        public static IEnumerable<KeyValuePair<int, string>> SegmentSelectorOptionsFull()
        {
            yield return new KeyValuePair<int, string>(-1, "-1");
            foreach (var option in SegmentSelectorOptions())
            {
                yield return option;
            }
        }

        internal static string GetPathInfoSegment(int segment)
        {
            string pathInfo = C1PageRoute.GetPathInfo();
            if (segment == -1)
            {
                return pathInfo;
            }

            string[] segments = (pathInfo ?? string.Empty).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length > segment)
            {
                return segments[segment];
            }

            return null;
        }
    }
}