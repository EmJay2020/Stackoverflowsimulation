$(() => {

    $("#question").on('click', '.clk', function () {
        const t = $(this).data('id');
        const Id = {
            iD: t
        }
        $.post('/home/like', Id, function (p) {
            ReloadLikes();
        })
    })
    function ReloadLikes() {
        const ie = $("#question").data('id');
        const i = parseInt(ie);
        $('.ldiv').remove();
        

        $.get(`/home/getlikes?Id=${i}`, function (l) {
            if (l.didLike) {
                $("#question").append(`<div class="ldiv"><h4>You liked this</h4><h4>${l.likeCount}</h4></div>

`)
            }
            else if (!l.notIn && !l.didLike) {
                $("#question").append(`<div class="ldiv">
<button class="btn btn-primary clk" data-id="${i}">Like</button><h4>${l.likeCount}</h4></div>
`)
            }
            else if (l.notIn) {
                $("#question").append(`<div class="ldiv"><h4>You need to log in to like this</h4><h4>${l.likeCount}</h4></div>`)

            }
        })
    }
    ReloadLikes();
})