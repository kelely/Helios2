/*
  Helios Web APP 启动文件
  页面程序从这里启动并完成初始化，包括配置和路由等
*/

define(function() {
    'use strict';

    var dependency = [
        // 'ngRoute',
        // 'Helios.directives',
        // 'Helios.services',
        // ... 其他各种引用
    ];

    // 主程序 / <html>
    window.Helios = angular.module('helios', dependency, function() {
        console.log('程序启动时触发');
    });


    // 主控制器 / <body>
    window.Helios.controller('AppController', ['$scope', '$log', function($scope, $log) {
        $scope.$on('$viewContentLoaded', function() {
            $log.debug('AppController.$viewContentLoaded');
        });

        $scope.hello = 'world';
        $log.debug('controller 实例化');
    }]);
}());