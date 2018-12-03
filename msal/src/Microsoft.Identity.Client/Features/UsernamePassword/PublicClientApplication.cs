﻿//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using Microsoft.Identity.Client.Internal.Requests;
using Microsoft.Identity.Core;
using Microsoft.Identity.Core.Instance;
using Microsoft.Identity.Core.Telemetry;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Core.WsTrust;
using System;

namespace Microsoft.Identity.Client
{
#if !ANDROID_BUILDTIME && !iOS_BUILDTIME && !WINDOWS_APP_BUILDTIME

    public sealed partial class PublicClientApplication : ClientApplicationBase
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <summary>
        /// Non-interactive request to acquire a security token from the authority, via Username/Password Authentication.
        /// Available only on .net desktop and .net core. See https://aka.ms/msal-net-up for details.
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="username">Identifier of the user application requests token on behalf.
        /// Generally in UserPrincipalName (UPN) format, e.g. john.doe@contoso.com</param>
        /// <param name="securePassword">User password.</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        public async Task<AuthenticationResult> AcquireTokenByUsernamePasswordAsync(IEnumerable<string> scopes, string username, System.Security.SecureString securePassword)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
#if ANDROID || iOS || WINDOWS_APP
            GuardMobilePlatforms();
            return null;
#else
            UsernamePasswordInput usernamePasswordInput = new UsernamePasswordInput(username, securePassword);
            return await AcquireTokenByUsernamePasswordAsync(scopes, usernamePasswordInput).ConfigureAwait(false);
#endif
        }

        private async Task<AuthenticationResult> AcquireTokenByUsernamePasswordAsync(IEnumerable<string> scopes, UsernamePasswordInput usernamePasswordInput)
        {
            GuardMobilePlatforms();

            Authority authority = Core.Instance.Authority.CreateAuthority(ServiceBundle, Authority, ValidateAuthority);
            var requestParams = CreateRequestParameters(authority, scopes, null, UserTokenCache);
            var handler = new UsernamePasswordRequest(
                ServiceBundle,
                requestParams,
                ApiEvent.ApiIds.AcquireTokenWithScopeUser,
                usernamePasswordInput);

            return await handler.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        private static void GuardMobilePlatforms()
        {
#if ANDROID || iOS || WINDOWS_APP
            throw new PlatformNotSupportedException("The Username / Password flow is not supported on Xamarin.Android, Xamarin.iOS and UWP. " +
               "For more details see https://aka.ms/msal-net-up");
#endif
        }
    }
#endif
}
