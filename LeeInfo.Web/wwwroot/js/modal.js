(function ($) {
    function Example() {
        var $this = this;

        function initilizeModel() {
            $("#modal-action").on('loaded.bs.modal', function (e) {
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }
        $this.init = function () {
            initilizeModel();
        }
    }
    $(function () {
        var self = new Example();
        self.init();
    })
}(jQuery))
