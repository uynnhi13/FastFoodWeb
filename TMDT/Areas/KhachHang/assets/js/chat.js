$(document).ready(() => {
    console.log("test in")
    $(".icon-chat").click(() => {
        $(".icon-chat").hide()
        $(".live-chat").show()
    })
    $(".btn-close-chat").click(() => {
        $(".live-chat").hide()
        $(".icon-chat").show()
    })

    $("#btnSendChat").click(() => {
        var text = $("#valSendChat").val()
        console.log(text)

        //Xoá trắng input sau khi enter
        $('#valSendChat').val('');
    })

    $('#valSendChat').keydown(function (event) {

        if (event.keyCode === 13) {

            var inputVal = $(this).val();

            // Code xử lý giá trị đầu vào ở đây

            $(".content-chat ul").append(
                `<li class="user-chat">${inputVal}</li>`
            )

            console.log(inputVal);

            //Xoá trắng input sau khi enter
            $(this).val(''); 

        }

    });
})