/**
 * Created by Lingo on 05/02/2017.
 */

function scrollNav() {

    (function () {
        'use strict';

        /*====================================
         Main Navigation Stick to Top when Scroll
         ======================================*/
        function sticky_relocate() {
            try {
            var window_top = $(window).scrollTop();
            var div_top = $('#sticky-anchor').offset().top;

                if (window_top > div_top) {
                    $('#tf-menu').addClass('stick');
                } else {
                    $('#tf-menu').removeClass('stick');
                }
            }catch(err) {
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

function flashMsg($type,$msg) {
    $( "<div class='flash "+$type+"' >"+$msg+"</div>" ).insertAfter( "header" );
}




// Game//
var box=[];
var dict={};
var Slot;
var SlotM;
var FirstLine;

function getBoard(){
    var pom = Slot-1;
    $("<div class=boxy ></div>").appendTo('.game');
    for(var i=0; i<FirstLine;i++) {
        $("<div style='display: flex;justify-content: center;' class=boxPanel" + i + " id=panel" + i + " ></div>").appendTo('.boxy');
        for(var j=0; j<FirstLine-i;j++){
            $("<div class=box id=Slot_"+ pom + ">").appendTo('.boxPanel'+i).droppable({
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
            pom--;
        }
    }
    $("<div style='display: flex;justify-content: center;' class=boxPanelM id=panelM ></div>").appendTo('.boxy');
    $("<div style='display: flex;flex-wrap: wrap;' class=boxPanM id=pan ></div>").appendTo('.boxPanelM');
    if(SlotM >10){
        $('#pan').css("width", "490px");
    }
    for(var i=0; i<SlotM;i++){
        $("<div class=box id=SlotM_"+ i + ">").appendTo('.boxPanM').droppable({
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
    }

}

function draw(values){
    $('.game').empty();
    getBoard();
    for(key in values) {
        if(dict[key] != "null"){
            $("<img style='width: 60px;height: 60px; left: -1px;top: -1px;' id=img" + dict[key] + " src='../media/img/Number/" + dict[key] + ".jpg' alt='" + dict[key] + "'>").attr('id', 'card'+ key).appendTo("#" + key).draggable({
                containment: '.boxy',
                cursor: 'move',
                opacity: 0.5,
                revert: "invalid",
                refreshPositions: true
            });
        }
    }
}
function PopupAlert(text) {
    if (confirm("Naozaj chcete odstranit " + text) == true) {
        return true;
    } else {
        return false;
    }
}

function removeItem(ref) {
    // Now we can get back to that item we just pushed via .child().
    ref.remove(function(error) {
        alert(error ? "Uh oh!" : "Success!");
    });
}
function getURLParameter(name) {
    return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [null, ''])[1].replace(/\+/g, '%20')) || null;
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for(var i = 0; i <ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function initFirebase() {
    var config = {
        apiKey: "AIzaSyDA8nnfmBPzxZ0pNxjxVObPt_pXOmg_lgM",
        authDomain: "sumrectangle.firebaseapp.com",
        databaseURL: "https://sumrectangle.firebaseio.com",
        storageBucket: "sumrectangle.appspot.com",
        messagingSenderId: "80041684698"
    };
    firebase.initializeApp(config);
}
$( document ).ready(function() {
    scrollNav();
    initFirebase();
});