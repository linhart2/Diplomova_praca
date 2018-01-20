/**
 * Created by Lingo on 26/04/2017.
 */
var box = 0
var priklad = {};
var prikladJson;
var prikladyJson = {};
var prikladyJsonPc = 1;

function drawNuberForExam(){
    //Vyplni modal content Cislami od 1 do 100;
    var num = 1;
    for(var i = 0; i< 10; i++){
        $("<div class=row"+i+"></div>").appendTo(".modal-body");
        for(var j = 0; j<10; j++){
            $("<a class=numbers id="+num+" onclick=appendNumberIntoBox("+ num + ") data-dismiss=modal><img style='width: 60px;height: 60px; left: 0px;top: 0px;' src=../media/img/Number/"+num+".jpg alt=Cislo_"+num+"></a>").appendTo(".row"+i);
            num++;
        }
    }
}

function appendNumberIntoBox(num){
    // prida cisla do boxov v trojuholniku
    $("#"+box).empty();
    $("<img style='width: 59px;height: 59px; left: -1px;top: -1px;' src=../media/img/Number/"+num+".jpg alt=Cislo_"+num+"></a>").appendTo("#"+box);
    priklad[box] = num;
    prikladJson = JSON.stringify(priklad)
    $("#inputJson").val(prikladJson);
}

function getBoxId(a){
    //uchovava cislo otvoreneho boxu
    box = a['id'];
}

function selectTemplate(){
    // zobrazenie moznych templateov
    $("#Butt_Add_Exam").remove();
    $("#Save_Exams").remove();
    if (! $(".SelectTemplate")[0]){
        $("<div class=SelectTemplate id=SelectTemplate></div>").appendTo(".SecondParameters");
        $("<button type=button onclick=createTask(1)>Sablona1</button>").appendTo(".SelectTemplate");
        $("<button type=button onclick=createTask(2)>Sablona2</button>").appendTo(".SelectTemplate");
        $("<button type=button onclick=createTask(3)>Sablona3</button>").appendTo(".SelectTemplate");
    }
}
function createTask(level){
    //vygeneruje prazdny trojuholnik pola zvoleneho templatu
    priklad = {};
    $("#2_param").empty();
    if (! $(".CreateTask")[0]){
        $("<div class=CreateTask></div>").appendTo(".SecondParameters");
        switch(level) {
            case 1:
                var pom = 3;
                var FirstLine = 2;
                break;
            case 2:
                var pom = 6;
                var FirstLine = 3;
                break;
            case 3:
                var pom = 10;
                var FirstLine = 4;
                break;
            default:
                var pom = 10;
                var FirstLine = 4;
        }
        $("<div class=board id=priklad></div>").appendTo('.CreateTask');
        $("<div class=boxy ></div>").appendTo('.board');
        for(var i=0; i<FirstLine;i++) {
            $("<div style='display: flex;justify-content: center;' class=boxPanel" + i + " id=panel" + i + " ></div>").appendTo('.boxy');
            for(var j=0; j<FirstLine-i;j++){
                $("<a class=box id=Slot_"+ pom + " class=btn btn-info btn-lg  data-toggle=modal data-target=#myModal onclick=getBoxId(Slot_"+ pom + ")></a>").appendTo('.boxPanel'+i);
                priklad["Slot_"+ pom ] = null;
                pom--;
            }
        }
        $("<div class=vstup><input id=inputJson type=text style='width:400px;'></div>").appendTo(".board");
        $("#inputJson").val(JSON.stringify(priklad)).focusout(function(){
            prikladJson = $("#inputJson").val();
            priklad = JSON.parse(prikladJson);
            for(pr in priklad){
                if(priklad[pr] !== null){
                    $("#"+pr).empty();
                    $("<img style='width: 59px;height: 59px; left: -1px;top: -1px;' src=../media/img/Number/"+priklad[pr]+".jpg alt=Cislo_"+priklad[pr]+">").appendTo("#"+pr);
                }
            }

        });
        $("<button type=button onclick=submitTask()>Potvrd priklad</button>").appendTo('.CreateTask');
        $("<button type=button onclick=removeTask()>Zrus priklad</button>").appendTo('.CreateTask');

    }
}
function submitTask(){
    if (controlTask()){
        $("#2_param").empty();
        prikladyJson[prikladyJsonPc.toString()] = priklad;
        $("<li id=priklad"+prikladyJsonPc+"><a onclick=''>"+prikladJson+"</a><a class='close' onclick=deleteTask("+prikladyJsonPc+")><i class='fa fa-window-close-o' aria-hidden=true></i></a></li>").appendTo(".exams_array ul");
        $("<button type=button onclick=selectTemplate() id=Butt_Add_Exam>Pridaj priklad</button>").appendTo(".BasicParameters");
        $("<button type=button id=Save_Exams onclick=saveExam()>Uloz ulohu</button>").appendTo(".BasicParameters");
        prikladyJsonPc++;
    }
}
function controlTask() {
    var pom = true;
    for(var pr in priklad) {
        if ( priklad[pr] === null ) {
            $("#"+pr).css('background-color', 'red');
            pom = false;
        }
    }
    if ( pom ) {
        return true;
    }
    return false;
}
function deleteTask(id) {
    if( PopupAlert("priklad ?") ) {
    $("#priklad"+id).remove();
    delete prikladyJson[id];
    }
}

function removeTask(){
    $("#2_param").empty();
    $("<button type=button onclick=selectTemplate() id=Butt_Add_Exam>Pridaj priklad</button>").appendTo(".BasicParameters");
    $("<button type=button id=Save_Exams onclick=saveExam()>Uloz ulohu</button>").appendTo(".BasicParameters");
}

function saveExam() {
    var examsName = $("#ExamsName").val();
    if (examsName.length >= 4 ){
        var newPostKey = firebase.database().ref().child('posts').push().key;
        prikladyJson["examsName"] = examsName;
        firebase.database().ref('exams/'+getCookie("teacherID")+"/"+newPostKey+"/").set(prikladyJson);
        prikladyJson = {};
        priklad = {};
        prikladyJsonPc = 1;
        $(".exams_array ul").empty();
        $("#ExamsName").val("");
        controlAppendExam(getCookie("teacherID"));
    }else{
        flashMsg("error","Nazov ulohy je priliz kratky");
    }
}
function controlAppendExam($name) {
    firebase.database().ref('exams/'+$name).once('value', function(snapshot){
        if (snapshot.exists()) {
            window.location.replace("../../www/application/showexams/");
            flashMsg("success","Cvicenie bolo ulozene do DB");
        } else {
            flashMsg("error","Cvicenie neulozilo do DB");
        }
    });
}

$(document).ready(function(){
    drawNuberForExam();
});