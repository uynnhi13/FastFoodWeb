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
})
