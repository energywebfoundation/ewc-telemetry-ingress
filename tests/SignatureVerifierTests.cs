using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using webapi;
using webapi.Controllers;
using Xunit;

namespace tests
{

    public class InfluxIngressTests
    {
        [Fact]
        public async void ValidMetricsShouldRecordAsync()
        {
            
            var dummyInflux = new DummyInflux();
            var keystore = new MockKeystore();
            keystore.AddKey("node-1", "BgIAAACkAABSU0ExAAQAAAEAAQBdUkRrF0SA3a+QtGv6y97DFa79Z/IDHtCHehoj/LADUJxXsI1k6GBqdyE7MkF9uX2j8FbAMlxpmIKrMcRTWj9wZ5gIhbntiCF61IFsQJ5af23WsTg82u9A7mepxSXrfgfu6Bzq1nB+pUGeWlATaLiOT+wm5uCYjYH8MiTMfDLu4g==");
         
            
            IngressController tc = new IngressController(keystore,dummyInflux);
            ActionResult webResponse = await tc.PostInfluxTelemetry(new InfluxTelemetry
            {
                NodeId = "node-1",
                Signature = "GT+8qiTNx2X2jtE0YQOBH6EE6Pu+a6DUFMK//LU+wiIwp/OPvaO7h2SDlU40/MAt83R4ZzVT2IBrl37phKUhbiBN0sMmvgxGJdJAOkAjKtgtacqUUxuVGim4PE6pAIAEIRoETQMe7ZlsALcoyA1p5M8Y1481bM1ykNcKQ23QPuM=",
                Payload = new List<string>
                {
                    "weather,location=us-midwest temperature=82 1465839830100400200",
                    "weather,location=us-east temperature=75 1465839830100400200"
                }
            });

            Assert.IsType<AcceptedResult>(webResponse);
            Assert.Equal(2,dummyInflux.LastInsertCount);
        }
        
        [Fact]
        public async void InvalidSignatureShouldNotRecordAsync()
        {
            
            var dummyInflux = new DummyInflux();
            var keystore = new MockKeystore();
            keystore.AddKey("node-1", "BgIAAACkAABSU0ExAAQAAAEAAQBdUkRrF0SA3a+QtGv6y97DFa79Z/IDHtCHehoj/LADUJxXsI1k6GBqdyE7MkF9uX2j8FbAMlxpmIKrMcRTWj9wZ5gIhbntiCF61IFsQJ5af23WsTg82u9A7mepxSXrfgfu6Bzq1nB+pUGeWlATaLiOT+wm5uCYjYH8MiTMfDLu4g==");
         
            
            IngressController tc = new IngressController(keystore,dummyInflux);
            ActionResult webResponse = await tc.PostInfluxTelemetry(new InfluxTelemetry
            {
                NodeId = "node-1",
                Signature = "GT+8qiTNx2X2jtE0YQOBH6EE6Pu+a6DUFMK//LU+wiIwp/OPvaO7h2SDlU40/MAt83R4ZzVT2IBrl37phKUhbiBN0sMmvgxGJdJAOkAjKtgtacqUUxuVGim4PE6pAIAEIRoETQMe7ZlsALcoyA1p5M8Y1481bM1ykNcKQ23QPuM=",
                Payload = new List<string>
                {
                    "weather,location=us-midwest temperature=82 1465839830100400200"
                }
            });

            Assert.IsType<ForbidResult>(webResponse);
            Assert.Equal(0,dummyInflux.LastInsertCount);
        }
        
        [Fact]
        public async void NullTelemetryShouldNotRecord()
        {
            
            var dummyInflux = new DummyInflux();
            var keystore = new MockKeystore();
            keystore.AddKey("node-1", "BgIAAACkAABSU0ExAAQAAAEAAQBdUkRrF0SA3a+QtGv6y97DFa79Z/IDHtCHehoj/LADUJxXsI1k6GBqdyE7MkF9uX2j8FbAMlxpmIKrMcRTWj9wZ5gIhbntiCF61IFsQJ5af23WsTg82u9A7mepxSXrfgfu6Bzq1nB+pUGeWlATaLiOT+wm5uCYjYH8MiTMfDLu4g==");
         
            
            IngressController tc = new IngressController(keystore,dummyInflux);
            ActionResult webResponse = await tc.PostInfluxTelemetry(new InfluxTelemetry
            {
                NodeId = null,
                Signature = null,
                Payload = null
            });

            Assert.IsType<BadRequestResult>(webResponse);
            Assert.Equal(0,dummyInflux.LastInsertCount);
        }
        
        [Fact]
        public async void EmptyTelemetryShouldNotRecord()
        {
            
            var dummyInflux = new DummyInflux();
            var keystore = new MockKeystore();
            keystore.AddKey("node-1", "BgIAAACkAABSU0ExAAQAAAEAAQBdUkRrF0SA3a+QtGv6y97DFa79Z/IDHtCHehoj/LADUJxXsI1k6GBqdyE7MkF9uX2j8FbAMlxpmIKrMcRTWj9wZ5gIhbntiCF61IFsQJ5af23WsTg82u9A7mepxSXrfgfu6Bzq1nB+pUGeWlATaLiOT+wm5uCYjYH8MiTMfDLu4g==");
         
            
            IngressController tc = new IngressController(keystore,dummyInflux);
            ActionResult webResponse = await tc.PostInfluxTelemetry(new InfluxTelemetry
            {
                NodeId = "",
                Signature = "",
                Payload = new List<string>()
            });

            Assert.IsType<BadRequestResult>(webResponse);
            Assert.Equal(0,dummyInflux.LastInsertCount);
        }
    }
    
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
    }
}