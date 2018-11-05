//宣告主程式
var frameApp =
    angular.module("frameApp", []).
        controller("frameController",function ($scope, $http) {
            //宣告URL
            var urlOptions = {
                readframeList: '/Monitor/ReadframeList'
            };
            //資料
            var updataData = function() {    
                $http.post(urlOptions.readframeList)
                    .then(function (response) {
                        //console.log(response.data);
                        $scope.Solar = response.data.Solar;
                        $scope.GirdPower = response.data.GirdPower;
                        $scope.Load = response.data.Load;
                        $scope.BatteyMode = response.data.BatteyMode;
                        $scope.BatteySOC = response.data.BatteySOC;
                        $scope.BatteyPower = response.data.BatteyPower;
                        $scope.GeneratorPower = response.data.GeneratorPower;
                        $scope.Direction = response.data.Direction;
                    });
            };

            //定時更新資料
            updataData();
            setInterval(function() { $scope.$apply(updataData); }, 1000);   

            ////定時更新時間
            //var timers = function() {$scope.now = new Date();};
            //timers();
            //setInterval(function() { $scope.$apply(timers); }, 1000);   
    });






