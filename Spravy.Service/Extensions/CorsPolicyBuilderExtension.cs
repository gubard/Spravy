using Microsoft.AspNetCore.Cors.Infrastructure;
using Spravy.Service.Helpers;

namespace Spravy.Service.Extensions;

public static class CorsPolicyBuilderExtension
{
    public static CorsPolicyBuilder AddGrpcHeaders(this CorsPolicyBuilder policyBuilder)
    {
        policyBuilder.WithExposedHeaders(PolicyNames.GrpcStatusHeader, PolicyNames.GrpcMessageHeader,
            PolicyNames.GrpcEncodingHeader, PolicyNames.GrpcAcceptEncodingHeader);

        return policyBuilder;
    }
}