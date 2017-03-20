/**
 * Created by Lingo on 05/02/2017.
 */
// Hello.
//
// This is The Scripts used for ___________ Theme
//
//

function main() {

    (function () {
        'use strict';

        /*====================================
         Main Navigation Stick to Top when Scroll
         ======================================*/
        function sticky_relocate() {
            var window_top = $(window).scrollTop();
            var div_top = $('#sticky-anchor').offset().top;
            if (window_top > div_top) {
                $('#tf-menu').addClass('stick');
            } else {
                $('#tf-menu').removeClass('stick');
            }
        }

        $(function () {
            $(window).scroll(sticky_relocate);
            sticky_relocate();
        });


        $(function() {
            $('a[href*="#"]:not([href="#"])').click(function() {
                if (location.pathname.replace(/^\//,'') == this.pathname.replace(/^\//,'') && location.hostname == this.hostname) {
                    var target = $(this.hash);
                    target = target.length ? target : $('[name=' + this.hash.slice(1) +']');
                    if (target.length) {
                        $('html,body').animate({
                            scrollTop: target.offset().top - 70
                        }, 1000);
                        return false;
                    }
                }
            });
        });




    }());
}
// Game//
function parseInput(val) {
    var value = val;
    value = value.split("|");
    var value_array=[];
    for (var i=0; i<value.length;i++){
        value_array.push(value[i].split(","));
        //value_array.push(parseInt(value[i]));
    }
    return value_array;
}
var box=[];
function draw(val){
    var value_array=parseInput(val);

    $('.game').empty();
    box=[]
    $("<div class=boxy ></div>").appendTo('.game');
    for (var i= 0; i<value_array.length;i++) {
        $("<div style='display: flex;justify-content: center;' class=boxPanel" + i + " id=panel" + i + " ></div>").appendTo('.boxy');
        for (var j = 0; j < value_array[i].length; j++) {
            console.log(value_array[i][j]);
            box.push('box'+ i + j);
            $("<div class=box id=box"+ i + j + "></div>").appendTo('.boxPanel'+i).droppable({
                drop: function (event, ui) {
                    if ($(this).children().length == 0) {
                        $('#' + $('#' + ui.draggable.attr('id')).parent().attr('id') + '>#' + ui.draggable.attr('id')).appendTo($('#' + $(this).attr('id')));
                        ui.draggable.position({of: $(this), my: '0 left 0 top ', at: '0 left 0 top'});
                        ui.draggable.draggable({disabled: false});
                    }
                    else if ($(this).children().length > 0) {
                        $('#' + $(this).attr('id') + '>#' + $(this).children().attr('id')).appendTo('#' + $('#' + ui.draggable.attr('id')).parent().attr('id'));
                        $('#' + $('#' + ui.draggable.attr('id')).parent().attr('id') + '>#' + ui.draggable.attr('id')).appendTo($('#' + $(this).attr('id')));
                        ui.draggable.position({of: $(this), my: '0 left 0 top ', at: '0 left 0 top'});
                        ui.draggable.draggable({disabled: false});
                    }
                }
            });
            if(value_array[i][j] != "Null"){
                $("<img style='width: 60px;height: 60px; left: -1px;top: -1px;' id=img" + j + " src='../media/img/Number/" + value_array[i][j] + ".jpg' alt='" + value_array[i][j] + "'>").attr('id', 'card'+ i + j).appendTo("#box" + i+ j).draggable({
                    containment: '.boxy',
                    cursor: 'move',
                    opacity: 0.5,
                    revert: "invalid",
                    refreshPositions: true
                });
            }
        }
    }
}

// Initialize Firebase
var config = {
    apiKey: "AIzaSyDA8nnfmBPzxZ0pNxjxVObPt_pXOmg_lgM",
    authDomain: "sumrectangle.firebaseapp.com",
    databaseURL: "https://sumrectangle.firebaseio.com",
    storageBucket: "sumrectangle.appspot.com",
    messagingSenderId: "80041684698"
};
firebase.initializeApp(config);
function writeUserData(userId, name, email) {
    firebase.database().ref('users/' + userId).set({
        username: name,
        email: email,
    });
}
var commentsRef = firebase.database().ref('Class_ID/member/email/example/');
commentsRef.on('child_changed', function(data) {
    draw(data.val());
    //setCommentValues(postElement, data.key, data.val().text, data.val().author);
});

firebase.database().ref('Class_ID/member/email/example/').once('value').then(function(snapshot) {
    var result = snapshot.val().result;
    $("<div class=boxy ></div>").appendTo('.game');
    draw(result);
});


/*commentsRef.once('value', function(snapshot) {
    snapshot.forEach(function(childSnapshot) {
        var childKey = childSnapshot.key;
        var childData = childSnapshot.val();
        console.log(childData,childKey);
        draw(childData);
    });
});*/

$( document ).ready(function() {
    console.log( "ready!" );
    main();
    //draw();
    //writeUserData("XXXX","Daniel","D@D.sk");
});