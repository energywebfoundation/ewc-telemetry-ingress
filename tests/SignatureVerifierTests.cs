using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using webapi;
using webapi.Controllers;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Linq;

namespace tests
{
    public class SignatureVerifierTests
    {
        [Theory]
        [InlineData(
            "BgIAAACkAABSU0ExAAQAAAEAAQCBj/4c2efRhDzwFlqwoobNjHIgrG7M5eI3wiUlZCK5y7nlWltSOk6plEFFz6Efp8EJxyQhqqC+QnIJlpmtUmtKzZAqGV4RDcB25h+klvyUGxmOXS0YLFte3k84526ldiOFZuCI1iGW3LDeKMdD+grOQ/CdFovoz/7o/DdEwEmIsg==",
            "foobar",
            "icgW//Ca226YqQKRdhophvBSU9sgf1V0O2wv6k1O7fWTBLKG/THCiJgeufOueg2YUgzzXZzHh+KEWH8MTnlrTUq0s1SI9OoMGh3WDJb3LkIF55K8RKYpp6W8KOCtU3hf6HTC+ui4+M+EYcX3ee34ZviNfbtYni4LWnEqnGAGHtc="
        )]
        [InlineData(
            "BgIAAACkAABSU0ExAAQAAAEAAQDFs8Wwxj0dqyUIGAMdqos2XT0gcYQxIFqBoZv9miYKcEi3G5DWadFYnYzpZ5+2IX8rF81L5xlxcEAv20u+5pcK30AGQnWGL4ZiJ5+7X+oXEY1S6nES/9mz/k5UF2e4n6/gBOv/VDR2UnuJyIrzWLgDQKPaGAYNi/EDIgAoGSMTmA==",
            "weather,location=us-midwest temperature=82 1465839830100400200",
            "Yxfd4y0IUGLOgOzE9KbfiA7Y+9V6WZP1NQ40CMAeJRRCrf7j1Fd1EXnFR78OgryW5tFbjP7PJ8BAAr3yh7UGgkEipiyESX39Ij6gcyQ2ymX+fQx97Wbgdfy698F6rlpYwfjnfaDPm2d3iVTdqcX5catNcjbT1WlDcsWyWIzleBQ=")]
        [InlineData(
            "BgIAAACkAABSU0ExAAQAAAEAAQBzU8EqTnpzCeywx4W0P5Szgv9nLv/zOEoAhfo7ymzye/0Gk+y8dZv+2g4FDoaiJOW8nMk2Yu8hMEQeDsS9dhZCulBpQQE1QAXYOiqp7hpEEbAmAX5IW37fuTSDZnxdSMsWN40KNyYhti+w7NGBT6e3EVoMcqSyHsFiTyfeoC1vxA==",
            "this-is-also-important",
            "absAUKYl78KAI3aA8FDWE2y2JATOCz7OUKG1hVhFNOyjSfwlGXhMA4oe3qou6JEnuKlsx+AqS5O+nz0oJ68FR7gLU8NPrWjVIWqFTyQMS0ntDRMEUl3oZXXD24fy+NaUOZ6o9OPxFASlEN/ueplXSgcedpXLfo0cfWQWM0GcTJ4=")]
        public void SignatureShouldVerify(string pubKey, string payload, string signature)
        {
            bool isValid = SignatureVerifier.IsSignatureValid(payload, signature, pubKey);
            Assert.True(isValid);
        }


        [Theory]
        [InlineData(
            "BgIAAACkAABSU0ExAAQAAAEAAQCBj/4c2efRhDzwFlqwoobNjHIgrG7M5eI3wiUlZCK5y7nlWltSOk6plEFFz6Efp8EJxyQhqqC+QnIJlpmtUmtKzZAqGV4RDcB25h+klvyUGxmOXS0YLFte3k84526ldiOFZuCI1iGW3LDeKMdD+grOQ/CdFovoz/7o/DdEwEmIsg==",
            "foobarbaz",
            "icgW//Ca226YqQKRdhophvBSU9sgf1V0O2wv6k1O7fWTBLKG/THCiJgeufOueg2YUgzzXZzHh+KEWH8MTnlrTUq0s1SI9OoMGh3WDJb3LkIF55K8RKYpp6W8KOCtU3hf6HTC+ui4+M+EYcX3ee34ZviNfbtYni4LWnEqnGAGHtc="
        )]
        [InlineData(
            "BgIAAACkAABSU0ExAAQAAAEAAQDFs8Wwxj0dqyUIGAMdqos2XT0gcYQxIFqBoZv9miYKcEi3G5DWadFYnYzpZ5+2IX8rF81L5xlxcEAv20u+5pcK30AGQnWGL4ZiJ5+7X+oXEY1S6nES/9mz/k5UF2e4n6/gBOv/VDR2UnuJyIrzWLgDQKPaGAYNi/EDIgAoGSMTmA==",
            "weather,location=us-east temperature=82 1465839830100400200",
            "Yxfd4y0IUGLOgOzE9KbfiA7Y+9V6WZP1NQ40CMAeJRRCrf7j1Fd1EXnFR78OgryW5tFbjP7PJ8BAAr3yh7UGgkEipiyESX39Ij6gcyQ2ymX+fQx97Wbgdfy698F6rlpYwfjnfaDPm2d3iVTdqcX5catNcjbT1WlDcsWyWIzleBQ=")]
        [InlineData(
            "BgIAAACkAABSU0ExAAQAAAEAAQBzU8EqTnpzCeywx4W0P5Szgv9nLv/zOEoAhfo7ymzye/0Gk+y8dZv+2g4FDoaiJOW8nMk2Yu8hMEQeDsS9dhZCulBpQQE1QAXYOiqp7hpEEbAmAX5IW37fuTSDZnxdSMsWN40KNyYhti+w7NGBT6e3EVoMcqSyHsFiTyfeoC1vxA==",
            "this-is-not-important",
            "absAUKYl78KAI3aA8FDWE2y2JATOCz7OUKG1hVhFNOyjSfwlGXhMA4oe3qou6JEnuKlsx+AqS5O+nz0oJ68FR7gLU8NPrWjVIWqFTyQMS0ntDRMEUl3oZXXD24fy+NaUOZ6o9OPxFASlEN/ueplXSgcedpXLfo0cfWQWM0GcTJ4=")]
        public void SignatureShouldNotVerify(string pubKey, string payload, string signature)
        {
            bool isValid = SignatureVerifier.IsSignatureValid(payload, signature, pubKey);
            Assert.True(!isValid);
        }

        [Fact]
        public void PublicKeyImportWithPrivateKeyShouldFail()
        {
            bool isValid = SignatureVerifier.IsSignatureValid("this-is-not-important",
             "absAUKYl78KAI3aA8FDWE2y2JATOCz7OUKG1hVhFNOyjSfwlGXhMA4oe3qou6JEnuKlsx+AqS5O+nz0oJ68FR7gLU8NPrWjVIWqFTyQMS0ntDRMEUl3oZXXD24fy+NaUOZ6o9OPxFASlEN/ueplXSgcedpXLfo0cfWQWM0GcTJ4=",
             //next we will give public private exported csp
             "BwIAAACkAABSU0EyAAQAAAEAAQBXZXt7QOileknWzBH2Sg+Yk4INDTbKA5XUUfUe23zUmr6eM1USCNHX3lidZfjk5Emuui1m8k0KnghxcJfOau8iPRpLg/lubMNojpLGe2MXn5GsyjgEpVdE+Cf0pLBAYHcBuBYHj99muMsJrJW1/InbKFa24JuVnBr+MybPuMXqtc9Ehyz/oomfsO6eYguHP4sqrvB595AFTtkKE7TcGmbt1dUWkSTxT0vfkvbVj0C/H8d7XlshIHlG1m2BEnFNi+T5CM2o0MH54c8DXKRhEujS+xeuk+u9POwL2/XLIvUcfMhL6Pt3o+Dk7HRkT3SPhRa/yJeZ1JEHpoVkYJOVfcXLfwijulTXJughtRpgd+0CBk4JSLYj0fkY+QyRWtuHQfAXTscNGGyW8uIMvHfPdNNOwuiKC4yZLfHuw+hQ1FDtTgH+iJ3nW5fwcIKudN0wusnRE3RIDo8QZky8AvAPb9Z/KbZHhpYXriWyfBKAfP9izhZaIcrRT6Fu+N9E+7oWkQY82jRwv237qtKOuqKl/WGcQE42vfBHWxmeKnBSzKZq8o+92oc06o2PYXqNqt38JOXn34W64nccPvINJJMIQ2UFoSydNf9D6iyxde86RDgSbNMxjqwEY3MiQmRs+QTG/8gwm5aiZG9Kr7K+3Vs65yr0NCwBmZ0DqMowmMbaeTrP3JYS/Ngr22p5vqtsYSTTGn/tU9mBL2asfWO4dxvkNXHCDNmendlCwvYKQZJONTS+GgxDfeC5i/lThMP1ua64H3I="
             );
            Assert.True(!isValid);

        }
    }
}