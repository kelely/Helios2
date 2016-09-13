/*
  Helios Web APP 启动文件
  页面程序从这里启动并完成初始化，包括配置和路由等
*/
'use strict';

var helios = angular.module('helios', [
    // 'ngAnimate',
    // 'ngCookies',
    // 'ngResource',
    // 'ngSanitize',
    // 'ngTouch',
    'ui.router',
    // 'ui.bootstrap',
    'oc.lazyLoad',
    // 'helios.customers',
    // 'helios.customers.service',
    // 'helios.weixin',
    // 'helios.weixin.service',
    // 'helios.utils.service',
], function() {
    console.log('程序启动时触发');
    // TODO: 可以在这里做登录检测?
})

// .run(['$rootScope', '$state', '$stateParams', function($rootScope, $state, $stateParams) {

//     // It's very handy to add references to $state and $stateParams to the $rootScope
//     // so that you can access them from any scope within your applications.For example,
//     // <li ng-class="{ active: $state.includes('contacts.list') }"> will set the <li>
//     // to active whenever 'contacts.list' or one of its decendents is active.
//     $rootScope.$state = $state;
//     $rootScope.$stateParams = $stateParams;
// }])

/**
 * ui-router 配置
 */
.config(['$stateProvider', '$locationProvider', '$urlRouterProvider',
    function($stateProvider, $locationProvider, $urlRouterProvider) {

        /////////////////////////////////
        // 重定向以及默认(otherwise)地址 //
        /////////////////////////////////

        // 使用 $urlRouterProvider 配置 url 重定向(when) 和失败的地址 (otherwise).
        $urlRouterProvider.otherwise("/");

        // `when` 方法用来指示把第一个参数匹配的地址跳转到第二个参数匹配的地址
        // 这里我们简单的配置一些跳转规则
        //.when('/c?id', '/contacts/:id')
        //.when('/user/:id', '/contacts/:id')

        $locationProvider.html5Mode(false).hashPrefix("!");

        /////////////////
        //   路由配置   //
        /////////////////

        // 使用 $stateProvider 配置路由.
        $stateProvider

        ////////////
        //  首页  //
        ////////////
            .state('dashboard', {
            url: "/",
            views: {
                'lazyLoadView': {
                    templateUrl: 'app/components/dashboard/dashboard.view.html',
                    controller: "dashboardCtrl",
                }
            },
            data: { pageTitle: '首页' },
            resolve: {
                deps: ['$ocLazyLoad', function($ocLazyLoad) {
                    return $ocLazyLoad.load([{
                        name: 'helios',
                        //insertBefore: '#ng_load_plugins_before', // load the above css files before a LINK element with this ID. Dynamic CSS files must be loaded between core and theme css files
                        files: [
                            // '/s.meiyuner.com/assets/m/manage/css/supplement.css',
                            // 'http://s.meiyuner.com/assets/m/global/plugins/moment.min.js',
                            'app/components/dashboard/dashboard.ctrl.js?v=' + (new Date()).getTime(),
                        ]
                    }]);
                }]
            }
        });
    }
])


.controller('AppController', ['$scope', function($scope) {
    $scope.$on('$viewContentLoaded', function() {

        // 此段代码每次加载页面都会执行??
        //$log.debug('AppController.viewContentLoaded');
    });

    console.debug('AppController.ctor');
}])

.controller('HeaderController', ['$scope', function($scope) {
    $scope.$on('$includeContentLoaded', function() {
        //Layout.initHeader(); // init header
        $.AdminLTE.layout.activate();
        console.debug('HeaderController.$includeContentLoaded');
    });
}])

.controller('SidebarController', ['$scope', function($scope) {
    $scope.$on('$includeContentLoaded', function() {
        //Layout.initSidebar(); // init sidebar
        $.AdminLTE.layout.activate();
        console.debug('SidebarController.$includeContentLoaded');
    });
}])

.controller('FooterController', ['$scope', function($scope) {
    $scope.$on('$includeContentLoaded', function() {
        //Layout.initFooter(); // init footer
        $.AdminLTE.layout.activate();
        console.debug('FooterController.$includeContentLoaded');
    });
}]);;


helios.directive('ngSpinnerLoader', ['$rootScope',
    function($rootScope) {
        return {
            link: function(scope, element, attrs) {
                // by defult hide the spinner bar
                element.addClass('hide'); // hide spinner bar by default
                // display the spinner bar whenever the route changes(the content part started loading)
                $rootScope.$on('$routeChangeStart', function() {
                    element.removeClass('hide'); // show spinner bar
                });
                // hide the spinner bar on rounte change success(after the content loaded)
                $rootScope.$on('$routeChangeSuccess', function() {
                    setTimeout(function() {
                        element.addClass('hide'); // hide spinner bar
                    }, 500);
                    $("html, body").animate({
                        scrollTop: 0
                    }, 500);
                });
            }
        };
    }
]);