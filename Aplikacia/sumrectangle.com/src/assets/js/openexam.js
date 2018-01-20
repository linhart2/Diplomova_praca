function getValues(){
    var resultRef = firebase.database().ref('exams/'+getCookie("teacherID")+"/"+getURLParameter('pid')+"/"+getURLParameter('name')+"/");
    resultRef.on('value', function(data) {
        createTask(data.numChildren());
        $("h1.examname").empty().append("Cislo ulohy "+data.key);
        data.forEach(function(childData) {
            $("<img style='width: 59px;height: 59px; left: -1px;top: -1px;' src=../media/img/Number/"+childData.val()+".jpg alt=Cislo_"+childData.val()+">").appendTo('#'+childData.key);
        });
    });
}

function createTask(level){
    //vygeneruje prazdny trojuholnik pola zvoleneho templatu
    priklad = {};
    $("#2_param").empty();
    if (! $(".CreateTask")[0]){
        $("<div class=CreateTask></div>").appendTo(".SecondParameters");
        switch(level) {
            case 3:
                var pom = 3;
                var FirstLine = 2;
                break;
            case 6:
                var pom = 6;
                var FirstLine = 3;
                break;
            case 10:
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
                $("<a class=box id=Slot_"+ pom + " class=btn btn-info btn-lg></a>").appendTo('.boxPanel'+i);
                priklad["Slot_"+ pom ] = null;
                pom--;
            }
        }
    }
}

$( document ).ready(function() {
    getValues();
});