﻿namespace CloudFlare.NET.ZoneClientSpec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Machine.Specifications;
    using Ploeh.AutoFixture;

    [Subject(typeof(CloudFlareClient))]
    public class When_getting_all_zones : RequestContext
    {
        static IReadOnlyList<Zone> _expected;
        static IReadOnlyList<Zone> _actual;
        static Uri _expectedRequestUri;

        Establish context = () =>
        {
            var response = _fixture.Create<CloudFlareResponse<IReadOnlyList<Zone>>>();
            _expected = response.Result;
            _handler.SetResponseContent(response);
            _expectedRequestUri = new Uri(CloudFlareConstants.BaseUri, "zones");
        };

        Because of = () => _actual = _sut.GetZonesAsync(_auth).Await().AsTask.Result;

        Behaves_like<AuthenticatedRequestBehaviour> authenticated_request_behaviour;

        It should_make_a_GET_request = () => _handler.Request.Method.ShouldEqual(HttpMethod.Get);

        It should_request_the_zones_endpoint = () => _handler.Request.RequestUri.ShouldEqual(_expectedRequestUri);

        It should_return_the_expected_zones = () =>
            _actual.Select(z => z.AsLikeness().CreateProxy()).SequenceEqual(_expected).ShouldBeTrue();
    }

    [Subject(typeof(CloudFlareClient))]
    public class When_getting_a_zone : RequestContext
    {
        static Zone _expected;
        static Zone _actual;
        static Uri _expectedRequestUri;

        Establish context = () =>
        {
            var response = _fixture.Create<CloudFlareResponse<Zone>>();
            _expected = response.Result;
            _handler.SetResponseContent(response);
            _expectedRequestUri = new Uri(CloudFlareConstants.BaseUri, $"zones/{_expected.Id}");
        };

        Because of = () => _actual = _sut.GetZoneAsync(_expected.Id, _auth).Await();

        Behaves_like<AuthenticatedRequestBehaviour> authenticated_request_behaviour;

        It should_make_a_GET_request = () => _handler.Request.Method.ShouldEqual(HttpMethod.Get);

        It should_request_the_zone_endpoint = () => _handler.Request.RequestUri.ShouldEqual(_expectedRequestUri);

        It should_return_the_expected_zones = () => _actual.AsLikeness().ShouldEqual(_expected);
    }
}