/*!
  RequireJs 配置文件，此文件配置了模块的依赖，解决了AngularJs自动延迟的问题，所以整个系统也从这里进行启动
*/

'use strict';

require.config({

    // 强制关闭缓存,TODO: 在开发时很好用，但是生产时增量更新怎么办? 加版本号吗?
    urlArgs: "v=" + (new Date()).getTime(),

    // 程序根目录
    baseUrl: 'app/',

    //配置angular的路径
    paths: {
        'jquery': '//cdn.bootcss.com/jquery/2.2.3/jquery.min',
        'bootstrap': '//cdn.bootcss.com/bootstrap/3.3.6/js/bootstrap.min',

        'angular': '//cdn.bootcss.com/angular.js/1.5.8/angular.min',
        // "angular-route": "path/to/angular-route",

        // AdminLTE 模板的app.js文件
        '$app': '../asserts/js/app.min',
    },

    //这个配置是你在引入依赖的时候的包名
    shim: {
        'angular': {
            exports: 'angular'
        },
        'bootstrap': {
            deps: ['jquery']
        },
        '$app': {
            deps: ['jquery', 'bootstrap']
        },
        'app.main': {
            deps: ['angular', '$app']
        }
        // "angular-route": {
        //     exports: "angular-route"
        // }
    }
});


// 加载 app.main.js 并运行
require(['app.main'], function() {
    // 使用RequireJs时，必须在此处手动启动 angular，不需要在 <html> 标记增加 ng-app 标记
    angular.bootstrap(document, ['helios']);
});