var box=[];
var dict={};
var Slot;
var SlotM;
var FirstLine;

function getBoard(){
    var pom = Slot;
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

function getParamBoard() {
    firebase.database().ref('Class_ID/member/email/example/board/').once('value').then(function(snapshot) {
        Slot = snapshot.val()["Slot"];
        SlotM = snapshot.val()["SlotM"];
        FirstLine = snapshot.val()["FirstLine"];
    });
}
function getResultBoard() {
    firebase.database().ref('Class_ID/member/email/example/result/').once('value').then(function(snapshot) {
        snapshot.forEach(function(child) {
            dict[child.key] = child.val();
        });
        draw(dict);
    });
}
function rewriteParamBoard() {
    var boardRef = firebase.database().ref('Class_ID/member/email/example/board/');
    boardRef.on('value', function(data) {
        Slot = data.val()["Slot"];
        SlotM = data.val()["SlotM"];
        FirstLine = data.val()["FirstLine"];
    });
}
function rewriteValueInBoard() {
    var resultRef = firebase.database().ref('Class_ID/member/email/example/result/');
    resultRef.on('child_changed', function(data) {
        dict[data.key] = data.val();
        draw(dict);
    });
}
$( document ).ready(function() {
    getParamBoard();
    getResultBoard();
    rewriteParamBoard();
    rewriteValueInBoard();
});