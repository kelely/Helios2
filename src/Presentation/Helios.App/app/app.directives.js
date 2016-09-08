/*
  为了方便管理众多directives，我们创建一个 Helios.directives， 这里面集合了所有的 directives
*/

(function() {
    'use strict';

    var dependency = [
        'Helios.directives.Navgation',
        'Helios.directives.Btn',
        // ... 其他 directives
    ];

    angular.module('Helios.directives', dependency);
}());