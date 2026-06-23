// BSD 2-Clause License
//
// Copyright (c) 2026, Rapid Loop
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Neo.Configuration;
using Neo.Core;
using Neo.Core.Types.Converter;
using Neo.Platform.Hosting.Logging;
using System;
using System.ComponentModel;
using System.Net;

namespace Neo.Platform.Hosting
{
    public static class HostingExtensions
    {

        public static IHostBuilder UseNeoPlatformConfiguration(this IHostBuilder builder)
        {
            // Host Configuration
            builder.ConfigureHostConfiguration
            (
                static config =>
                {
                    var manger = new ConfigurationManager();

                    config.AddConfiguration(manger);
                    config.AddNeoPlatformConfiguration();
                }
            );

            // Application Configuration
            builder.ConfigureAppConfiguration
            (
                static (context, config) =>
                {
                    var environmentName = context.HostingEnvironment.EnvironmentName;

                    // Executable Directory (Config)
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("config.json", optional: false); // application global settings  (gets overwritten by environment)

                    // Working Directory (Config)
                    var workingDir = context.Configuration[HostDefaults.ContentRootKey];

                    // NOTE: JSON Files Overwrite Environment Variables
                    config.SetBasePath($"{workingDir}");
                    config.AddJsonFile($"config.{environmentName}.json", optional: true);   // application settings by environment
                    config.AddJsonFile($"protocol.json", optional: false);                  // protocol global settings (gets overwritten by environment)
                    config.AddJsonFile($"protocol.{environmentName}.json", optional: true); // protocol settings  by environment
                    config.AddJsonFile($"vm.json", optional: false);                        // vm global settings (gets overwritten by environment)
                    config.AddJsonFile($"vm.{environmentName}.json", optional: true);       // vm settings  by environment
                }
            );

            // Logging Configuration
            builder.ConfigureLogging
            (
                static (context, logging) =>
                {
                    var isWindows = OperatingSystem.IsWindows();

                    if (isWindows)
                        logging.AddFilter<EventLogLoggerProvider>
                        (
                            level => level >= LogLevel.Warning
                        );

                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logging.AddNeoPlatform();
#if DEBUG
                    logging.AddDebug();
#endif
                    logging.AddEventSourceLogger();

                    if (isWindows)
                        logging.AddEventLog();

                    logging.Configure
                    (
                        static options =>
                        {
                            options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                                | ActivityTrackingOptions.TraceId
                                | ActivityTrackingOptions.ParentId;
                        }
                    );
                }
            );

            builder.UseDefaultServiceProvider
            (
                static (context, options) =>
                {
                    var isDevelopment = context.HostingEnvironment.IsDevelopment();

                    options.ValidateScopes = isDevelopment;
                    options.ValidateOnBuild = isDevelopment;
                }
            );

            builder.AddNeoPlatformLifetime();
            builder.AddNeoPlatformOptions();

            builder.ConfigureServices
            (
                static (context, services) =>
                {
                    // Add default services
                }
            );

            // IConfiguration.Get<T>() Converting
            TypeDescriptor.AddAttributes(typeof(IPAddress), new TypeConverterAttribute(typeof(IPAddressTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(IPEndPoint), new TypeConverterAttribute(typeof(IPEndPointTypeConverter)));

            return builder;
        }

        public static IHostBuilder AddNeoPlatformOptions(this IHostBuilder builder) =>
            builder.ConfigureServices
            (
                static (context, services) =>
                {
                    var protocolSettingsSection = context.Configuration.GetSection(ProtocolSettingNames.SectionKey);
                    var protocolSettingsOptions = protocolSettingsSection.Get<ProtocolSettingsOptions>();
                    services.AddSingleton(protocolSettingsOptions?.ToObject() ?? ProtocolSettings.Default);
                }
            );

        public static IHostBuilder AddNeoPlatformLifetime(this IHostBuilder builder, Action<NeoPlatformLifetimeOptions>? configure = default) =>
            builder.ConfigureServices(
                services =>
                {
                    services.AddSingleton<IHostLifetime, NeoPlatformLifetime>();
                    if (configure is not null)
                        services.Configure(configure);
                });
    }
}
