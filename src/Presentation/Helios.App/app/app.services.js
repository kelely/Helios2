/*
  为了方便管理众多 services，我们创建一个 Helios.services， 这里面集合了所有的 services
*/

(function() {
    'use strict';

    var dependency = [
        'Helios.services.Navgation',
        'Helios.services.Btn',
        // ... 其他 services
    ];

    angular.module('Helios.services', dependency);
}());