˛
lC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\UserId.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public		 
sealed		 
record		 
UserId		 
{

 
public 

Guid 
Value 
{ 
get 
; 
} 
public 

UserId 
( 
Guid 
value 
) 
{ 
if 

( 
value 
== 
Guid 
. 
Empty 
)  
{ 	
throw 
new 
ArgumentException '
(' (
$str( F
,F G
nameofH N
(N O
valueO T
)T U
)U V
;V W
} 	
Value 
= 
value 
; 
} 
public## 

static## 
implicit## 
operator## #
UserId##$ *
(##* +
Guid##+ /
value##0 5
)##5 6
=>##7 9
new##: =
(##= >
value##> C
)##C D
;##D E
public)) 

static)) 
implicit)) 
operator)) #
Guid))$ (
())( )
UserId))) /
userId))0 6
)))6 7
=>))8 :
userId)); A
.))A B
Value))B G
;))G H
public11 

static11 
UserId11 

FromString11 #
(11# $
string11$ *
value11+ 0
)110 1
{22 
if33 

(33 
string33 
.33 
IsNullOrWhiteSpace33 %
(33% &
value33& +
)33+ ,
)33, -
{44 	
throw55 
new55 
ArgumentException55 '
(55' (
$str55( P
,55P Q
nameof55R X
(55X Y
value55Y ^
)55^ _
)55_ `
;55` a
}66 	
if88 

(88 
!88 
Guid88 
.88 
TryParse88 
(88 
value88  
,88  !
out88" %
var88& )
guid88* .
)88. /
)88/ 0
{99 	
throw:: 
new:: 
ArgumentException:: '
(::' (
$"::( *
$str::* ?
{::? @
value::@ E
}::E F
"::F G
,::G H
nameof::I O
(::O P
value::P U
)::U V
)::V W
;::W X
};; 	
return== 
new== 
UserId== 
(== 
guid== 
)== 
;==  
}>> 
publicCC 

overrideCC 
stringCC 
ToStringCC #
(CC# $
)CC$ %
=>CC& (
ValueCC) .
.CC. /
ToStringCC/ 7
(CC7 8
)CC8 9
;CC9 :
}DD ç
yC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\TipAllocationResult.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public		 
record		 
TipAllocationResult		 !
(		! "
Guid

 
	SessionId

	 
,

 
Money 	
TotalTipAmount
 
, 
IReadOnlyList 
< 
ServerTipAllocation %
>% &
Allocations' 2
,2 3
bool 
IsValid	 
= 
true 
, 
string 

?
 
ValidationMessage 
= 
null  $
) 
{ 
public 

static 
TipAllocationResult %
Success& -
(- .
Guid 
	sessionId 
, 
Money 
totalTipAmount 
, 
IReadOnlyList 
< 
ServerTipAllocation )
>) *
allocations+ 6
)6 7
=>8 :
new 
( 
	sessionId 
, 
totalTipAmount %
,% &
allocations' 2
)2 3
;3 4
public 

static 
TipAllocationResult %
ValidationError& 5
(5 6
Guid 
	sessionId 
, 
Money 
totalTipAmount 
, 
string 
validationMessage  
)  !
=>" $
new 
( 
	sessionId 
, 
totalTipAmount %
,% &
Array' ,
., -
Empty- 2
<2 3
ServerTipAllocation3 F
>F G
(G H
)H I
,I J
falseK P
,P Q
validationMessageR c
)c d
;d e
} 
public!! 
record!! 
ServerTipAllocation!! !
(!!! "
Guid"" 
ServerId""	 
,"" 
string## 


ServerName## 
,## 
decimal$$  
AllocationPercentage$$  
,$$  !
Money%% 	
AllocatedAmount%%
 
,%% 
bool&& 
	IsPrimary&&	 
)'' 
;'' û
mC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\TaxRate.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public 
record 
TaxRate 
{		 
public

 

decimal

 
Rate

 
{

 
get

 
;

 
init

 #
;

# $
}

% &
public 

string 
Name 
{ 
get 
; 
init "
;" #
}$ %
=& '
string( .
.. /
Empty/ 4
;4 5
public 

bool 

IsCompound 
{ 
get  
;  !
init" &
;& '
}( )
private 
TaxRate 
( 
) 
{ 
} 
public 

TaxRate 
( 
decimal 
rate 
,  
string! '
name( ,
,, -
bool. 2

isCompound3 =
=> ?
false@ E
)E F
{ 
if 

( 
rate 
< 
$num 
|| 
rate 
> 
$num  
)  !
{ 	
throw 
new 

Exceptions  
.  !*
BusinessRuleViolationException! ?
(? @
$str@ p
)p q
;q r
} 	
if 

( 
string 
. 
IsNullOrWhiteSpace %
(% &
name& *
)* +
)+ ,
{ 	
throw 
new 
ArgumentException '
(' (
$str( P
,P Q
nameofR X
(X Y
nameY ]
)] ^
)^ _
;_ `
} 	
Rate 
= 
rate 
; 
Name 
= 
name 
; 

IsCompound   
=   

isCompound   
;    
}!! 
public&& 

Money&& 
CalculateTax&& 
(&& 
Money&& #

baseAmount&&$ .
)&&. /
{'' 
return(( 

baseAmount(( 
*(( 
Rate((  
;((  !
})) 
public.. 

Money.. 
CalculateTax.. 
(.. 
Money.. #

baseAmount..$ .
,... /
Money..0 5
previousTaxes..6 C
)..C D
{// 
if00 

(00 

IsCompound00 
)00 
{11 	
return22 
(22 

baseAmount22 
+22  
previousTaxes22! .
)22. /
*220 1
Rate222 6
;226 7
}33 	
return44 

baseAmount44 
*44 
Rate44  
;44  !
}55 
public77 

static77 
TaxRate77 
Zero77 
(77 
)77  
=>77! #
new77$ '
TaxRate77( /
(77/ 0
$num770 2
,772 3
$str774 <
)77< =
;77= >
}88 Ë#
nC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\TaxGroup.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public		 
record		 
TaxGroup		 
{

 
public 

string 
Name 
{ 
get 
; 
init "
;" #
}$ %
=& '
string( .
.. /
Empty/ 4
;4 5
public 

IReadOnlyList 
< 
TaxRate  
>  !
TaxRates" *
{+ ,
get- 0
;0 1
init2 6
;6 7
}8 9
=: ;
new< ?
List@ D
<D E
TaxRateE L
>L M
(M N
)N O
;O P
private 
TaxGroup 
( 
) 
{ 
} 
public 

TaxGroup 
( 
string 
name 
,  
IEnumerable! ,
<, -
TaxRate- 4
>4 5
taxRates6 >
)> ?
{ 
if 

( 
string 
. 
IsNullOrWhiteSpace %
(% &
name& *
)* +
)+ ,
{ 	
throw 
new 
ArgumentException '
(' (
$str( Q
,Q R
nameofS Y
(Y Z
nameZ ^
)^ _
)_ `
;` a
} 	
if 

( 
taxRates 
== 
null 
) 
{ 	
throw 
new !
ArgumentNullException +
(+ ,
nameof, 2
(2 3
taxRates3 ;
); <
)< =
;= >
} 	
var 
	ratesList 
= 
taxRates  
.  !
ToList! '
(' (
)( )
;) *
if 

( 
! 
	ratesList 
. 
Any 
( 
) 
) 
{   	
throw!! 
new!! 

Exceptions!!  
.!!  !*
BusinessRuleViolationException!!! ?
(!!? @
$str!!@ o
)!!o p
;!!p q
}"" 	
Name$$ 
=$$ 
name$$ 
;$$ 
TaxRates%% 
=%% 
	ratesList%% 
.%% 

AsReadOnly%% '
(%%' (
)%%( )
;%%) *
}&& 
public++ 

Money++ 
CalculateTotalTax++ "
(++" #
Money++# (

baseAmount++) 3
)++3 4
{,, 
Money-- 
totalTax-- 
=-- 
Money-- 
.-- 
Zero-- #
(--# $
)--$ %
;--% &
Money.. 
currentBase.. 
=.. 

baseAmount.. &
;..& '
foreach00 
(00 
var00 
rate00 
in00 
TaxRates00 %
)00% &
{11 	
Money22 
	taxAmount22 
=22 
rate22 "
.22" #
CalculateTax22# /
(22/ 0
currentBase220 ;
,22; <
totalTax22= E
)22E F
;22F G
totalTax33 
+=33 
	taxAmount33 !
;33! "
if66 
(66 
rate66 
.66 

IsCompound66 
)66  
{77 
currentBase88 
=88 

baseAmount88 (
+88) *
totalTax88+ 3
;883 4
}99 
}:: 	
return<< 
totalTax<< 
;<< 
}== 
publicCC 

decimalCC 
CombinedRateCC 
=>CC  "
TaxRatesCC# +
.CC+ ,
SumCC, /
(CC/ 0
rCC0 1
=>CC2 4
rCC5 6
.CC6 7
RateCC7 ;
)CC; <
;CC< =
publicEE 

staticEE 
TaxGroupEE 
NoTaxEE  
(EE  !
)EE! "
=>EE# %
newEE& )
TaxGroupEE* 2
(EE2 3
$strEE3 ;
,EE; <
newEE= @
[EE@ A
]EEA B
{EEC D
TaxRateEEE L
.EEL M
ZeroEEM Q
(EEQ R
)EER S
}EET U
)EEU V
;EEV W
}FF ß<
zC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\TableSplitAllocation.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public

 
record

  
TableSplitAllocation

 "
(

" #
IReadOnlyDictionary 
< 
Guid 
,  
SplitTableAllocation 2
>2 3
TableAllocations4 D
) 
{ 
public 

static  
TableSplitAllocation &
Create' -
(- .
IDictionary. 9
<9 :
Guid: >
,> ? 
SplitTableAllocation@ T
>T U
allocationsV a
)a b
{ 
if 

( 
allocations 
== 
null 
||  "
!# $
allocations$ /
./ 0
Any0 3
(3 4
)4 5
)5 6
{ 	
throw 
new 
ArgumentException '
(' (
$str( T
,T U
nameofV \
(\ ]
allocations] h
)h i
)i j
;j k
} 	
var 
totalPercentage 
= 
allocations )
.) *
Values* 0
.0 1
Sum1 4
(4 5
a5 6
=>7 9
a: ;
.; <
ChargePercentage< L
)L M
;M N
if 

( 
Math 
. 
Abs 
( 
totalPercentage $
-% &
$num' +
)+ ,
>- .
$num/ 4
)4 5
{ 	
throw 
new 
ArgumentException '
(' (
$"( *
$str* ^
{^ _
totalPercentage_ n
}n o
$stro p
"p q
)q r
;r s
}   	
if## 

(## 
allocations## 
.## 
Values## 
.## 
Any## "
(##" #
a### $
=>##% '
a##( )
.##) *
ChargePercentage##* :
<=##; =
$num##> ?
)##? @
)##@ A
{$$ 	
throw%% 
new%% 
ArgumentException%% '
(%%' (
$str%%( [
)%%[ \
;%%\ ]
}&& 	
if)) 

()) 
allocations)) 
.)) 
Keys)) 
.)) 
Any))  
())  !
id))! #
=>))$ &
id))' )
==))* ,
Guid))- 1
.))1 2
Empty))2 7
)))7 8
)))8 9
{** 	
throw++ 
new++ 
ArgumentException++ '
(++' (
$str++( X
)++X Y
;++Y Z
},, 	
return.. 
new..  
TableSplitAllocation.. '
(..' (
allocations..( 3
...3 4
ToDictionary..4 @
(..@ A
kvp..A D
=>..E G
kvp..H K
...K L
Key..L O
,..O P
kvp..Q T
=>..U W
kvp..X [
...[ \
Value..\ a
)..a b
)..b c
;..c d
}// 
public44 

int44 

TableCount44 
=>44 
TableAllocations44 -
.44- .
Count44. 3
;443 4
public99 

decimal99 !
TotalChargePercentage99 (
=>99) +
TableAllocations99, <
.99< =
Values99= C
.99C D
Sum99D G
(99G H
a99H I
=>99J L
a99M N
.99N O
ChargePercentage99O _
)99_ `
;99` a
public?? 

bool?? 
IsValid?? 
(?? 
)?? 
{@@ 
returnAA 
TableAllocationsAA 
.AA  
AnyAA  #
(AA# $
)AA$ %
&&AA& (
MathBB 
.BB 
AbsBB 
(BB !
TotalChargePercentageBB -
-BB. /
$numBB0 4
)BB4 5
<=BB6 8
$numBB9 >
&&BB? A
TableAllocationsCC 
.CC  
ValuesCC  &
.CC& '
AllCC' *
(CC* +
aCC+ ,
=>CC- /
aCC0 1
.CC1 2
ChargePercentageCC2 B
>CCC D
$numCCE F
)CCF G
&&CCH J
TableAllocationsDD 
.DD  
KeysDD  $
.DD$ %
AllDD% (
(DD( )
idDD) +
=>DD, .
idDD/ 1
!=DD2 4
GuidDD5 9
.DD9 :
EmptyDD: ?
)DD? @
;DD@ A
}EE 
}FF 
publicKK 
recordKK  
SplitTableAllocationKK "
(KK" #
GuidLL 
TableIdLL	 
,LL 
decimalMM 
ChargePercentageMM 
,MM 
intNN 

GuestCountNN 
,NN 
IReadOnlyListOO 
<OO 
GuidOO 
>OO 
?OO 
EquipmentIdsOO %
=OO& '
nullOO( ,
,OO, -
IReadOnlyListPP 
<PP 
GuidPP 
>PP 
?PP 
	ServerIdsPP "
=PP# $
nullPP% )
)QQ 
{RR 
public]] 

static]]  
SplitTableAllocation]] &
Create]]' -
(]]- .
Guid^^ 
tableId^^ 
,^^ 
decimal__ 
chargePercentage__  
,__  !
int`` 

guestCount`` 
,`` 
IEnumerableaa 
<aa 
Guidaa 
>aa 
?aa 
equipmentIdsaa '
=aa( )
nullaa* .
,aa. /
IEnumerablebb 
<bb 
Guidbb 
>bb 
?bb 
	serverIdsbb $
=bb% &
nullbb' +
)bb+ ,
{cc 
ifdd 

(dd 
tableIddd 
==dd 
Guiddd 
.dd 
Emptydd !
)dd! "
{ee 	
throwff 
newff 
ArgumentExceptionff '
(ff' (
$strff( C
,ffC D
nameofffE K
(ffK L
tableIdffL S
)ffS T
)ffT U
;ffU V
}gg 	
ifii 

(ii 
chargePercentageii 
<=ii 
$numii  !
||ii" $
chargePercentageii% 5
>ii6 7
$numii8 ;
)ii; <
{jj 	
throwkk 
newkk 
ArgumentExceptionkk '
(kk' (
$strkk( V
,kkV W
nameofkkX ^
(kk^ _
chargePercentagekk_ o
)kko p
)kkp q
;kkq r
}ll 	
ifnn 

(nn 

guestCountnn 
<=nn 
$numnn 
)nn 
{oo 	
throwpp 
newpp 
ArgumentExceptionpp '
(pp' (
$strpp( P
,ppP Q
nameofppR X
(ppX Y

guestCountppY c
)ppc d
)ppd e
;ppe f
}qq 	
returnss 
newss  
SplitTableAllocationss '
(ss' (
tableIdtt 
,tt 
chargePercentageuu 
,uu 

guestCountvv 
,vv 
equipmentIdsww 
?ww 
.ww 
ToListww  
(ww  !
)ww! "
,ww" #
	serverIdsxx 
?xx 
.xx 
ToListxx 
(xx 
)xx 
)yy 	
;yy	 

}zz 
}{{ Ç
zC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\TableOperationResult.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public		 
record		  
TableOperationResult		 "
(		" #
bool

 
IsSuccessful

	 
,

 
string 

?
 
ErrorMessage 
= 
null 
,  
TableOperationData 
? 
Data 
= 
null #
) 
{ 
public 

static  
TableOperationResult &
Success' .
(. /
TableOperationData/ A
?A B
dataC G
=H I
nullJ N
)N O
=>P R
new 
( 
true 
, 
null 
, 
data 
) 
; 
public 

static  
TableOperationResult &
NotFound' /
(/ 0
string0 6

entityType7 A
=B C
$strD K
)K L
=>M O
new 
( 
false 
, 
$" 
{ 

entityType  
}  !
$str! +
"+ ,
), -
;- .
public 

static  
TableOperationResult &
InvalidOperation' 7
(7 8
string8 >
message? F
)F G
=>H J
new 
( 
false 
, 
message 
) 
; 
public 

static  
TableOperationResult &
ValidationError' 6
(6 7
string7 =
message> E
)E F
=>G I
new 
( 
false 
, 
message 
) 
; 
public 

static  
TableOperationResult &
Unauthorized' 3
(3 4
string4 :
message; B
=C D
$strE ]
)] ^
=>_ a
new 
( 
false 
, 
message 
) 
; 
} 
public"" 
record"" 
TableOperationData""  
(""  !
Guid## 
OperationId##	 
,## 
TableOperationType$$ 
OperationType$$ $
,$$$ %
IReadOnlyList%% 
<%% 
Guid%% 
>%% 
TableIds%%  
,%%  !
Guid&& 
?&& 	
ResultingSessionId&&
 
,&& 
IReadOnlyList'' 
<'' 
Guid'' 
>'' 
?'' 
ResultingSessionIds'' ,
,'', -
Money(( 	
TotalChargesBefore((
 
,(( 
Money)) 	
TotalChargesAfter))
 
,)) 
DateTime** 
OperationTimestamp** 
,**  
Guid++ 
StaffId++	 
,++ 
string,, 

Reason,, 
)-- 
;-- 
public22 
enum22 
TableOperationType22 
{33 
Merge44 	
,44	 

Split55 	
,55	 

Transfer66 
}77 …j
~C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\TableOperationAuditEntry.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public		 
record		 $
TableOperationAuditEntry		 &
(		& '
Guid

 
Id

	 
,

 
Guid 
TableId	 
, 
TableOperationType 
OperationType $
,$ %
DateTime 
	Timestamp 
, 
Guid 
StaffId	 
, 
string 

	StaffName 
, 
string 

Reason 
, #
TableOperationAuditData 
BeforeState '
,' (#
TableOperationAuditData 

AfterState &
,& '
IReadOnlyDictionary 
< 
string 
, 
object  &
>& '
AdditionalData( 6
) 
{ 
public## 

static## $
TableOperationAuditEntry## *
Create##+ 1
(##1 2
Guid$$ 
tableId$$ 
,$$ 
TableOperationType%% 
operationType%% (
,%%( )
Guid&& 
staffId&& 
,&& 
string'' 
	staffName'' 
,'' 
string(( 
reason(( 
,(( #
TableOperationAuditData)) 
beforeState))  +
,))+ ,#
TableOperationAuditData** 

afterState**  *
,*** +
IDictionary++ 
<++ 
string++ 
,++ 
object++ "
>++" #
?++# $
additionalData++% 3
=++4 5
null++6 :
)++: ;
{,, 
if-- 

(-- 
tableId-- 
==-- 
Guid-- 
.-- 
Empty-- !
)--! "
{.. 	
throw// 
new// 
ArgumentException// '
(//' (
$str//( C
,//C D
nameof//E K
(//K L
tableId//L S
)//S T
)//T U
;//U V
}00 	
if22 

(22 
staffId22 
==22 
Guid22 
.22 
Empty22 !
)22! "
{33 	
throw44 
new44 
ArgumentException44 '
(44' (
$str44( C
,44C D
nameof44E K
(44K L
staffId44L S
)44S T
)44T U
;44U V
}55 	
if77 

(77 
string77 
.77 
IsNullOrWhiteSpace77 %
(77% &
	staffName77& /
)77/ 0
)770 1
{88 	
throw99 
new99 
ArgumentException99 '
(99' (
$str99( E
,99E F
nameof99G M
(99M N
	staffName99N W
)99W X
)99X Y
;99Y Z
}:: 	
if<< 

(<< 
string<< 
.<< 
IsNullOrWhiteSpace<< %
(<<% &
reason<<& ,
)<<, -
)<<- .
{== 	
throw>> 
new>> 
ArgumentException>> '
(>>' (
$str>>( A
,>>A B
nameof>>C I
(>>I J
reason>>J P
)>>P Q
)>>Q R
;>>R S
}?? 	
ifAA 

(AA 
beforeStateAA 
==AA 
nullAA 
)AA  
{BB 	
throwCC 
newCC !
ArgumentNullExceptionCC +
(CC+ ,
nameofCC, 2
(CC2 3
beforeStateCC3 >
)CC> ?
)CC? @
;CC@ A
}DD 	
ifFF 

(FF 

afterStateFF 
==FF 
nullFF 
)FF 
{GG 	
throwHH 
newHH !
ArgumentNullExceptionHH +
(HH+ ,
nameofHH, 2
(HH2 3

afterStateHH3 =
)HH= >
)HH> ?
;HH? @
}II 	
returnKK 
newKK $
TableOperationAuditEntryKK +
(KK+ ,
GuidLL 
.LL 
NewGuidLL 
(LL 
)LL 
,LL 
tableIdMM 
,MM 
operationTypeNN 
,NN 
DateTimeOO 
.OO 
UtcNowOO 
,OO 
staffIdPP 
,PP 
	staffNameQQ 
,QQ 
reasonRR 
,RR 
beforeStateSS 
,SS 

afterStateTT 
,TT 
additionalDataUU 
?UU 
.UU 
ToDictionaryUU (
(UU( )
kvpUU) ,
=>UU- /
kvpUU0 3
.UU3 4
KeyUU4 7
,UU7 8
kvpUU9 <
=>UU= ?
kvpUU@ C
.UUC D
ValueUUD I
)UUI J
??UUK M
newUUN Q

DictionaryUUR \
<UU\ ]
stringUU] c
,UUc d
objectUUe k
>UUk l
(UUl m
)UUm n
)VV 	
;VV	 

}WW 
public\\ 

string\\ 
OperationSummary\\ "
=>\\# %
$"\\& (
{\\( )
OperationType\\) 6
}\\6 7
$str\\7 K
{\\K L
TableId\\L S
}\\S T
$str\\T X
{\\X Y
	StaffName\\Y b
}\\b c
$str\\c g
{\\g h
	Timestamp\\h q
:\\q r
$str	\\r Ö
}
\\Ö Ü
$str
\\Ü ä
"
\\ä ã
;
\\ã å
publicaa 

boolaa 
HasChargeChangeaa 
=>aa  "
BeforeStateaa# .
.aa. /
TotalChargeaa/ :
!=aa; =

AfterStateaa> H
.aaH I
TotalChargeaaI T
;aaT U
publicff 

Moneyff 
ChargeDifferenceff !
=>ff" $

AfterStateff% /
.ff/ 0
TotalChargeff0 ;
-ff< =
BeforeStateff> I
.ffI J
TotalChargeffJ U
;ffU V
}gg 
publicll 
recordll #
TableOperationAuditDatall %
(ll% &
IReadOnlyListmm 
<mm 
Guidmm 
>mm 
TableIdsmm  
,mm  !
IReadOnlyListnn 
<nn 
Guidnn 
>nn 

SessionIdsnn "
,nn" #
Moneyoo 	
TotalChargeoo
 
,oo 
intpp 
TotalGuestCountpp 
,pp 
IReadOnlyListqq 
<qq 
Guidqq 
>qq 
EquipmentIdsqq $
,qq$ %
IReadOnlyListrr 
<rr 
Guidrr 
>rr 
	ServerIdsrr !
,rr! "
IReadOnlyDictionaryss 
<ss 
stringss 
,ss 
objectss  &
>ss& '
	StateDatass( 1
)tt 
{uu 
public
ÅÅ 

static
ÅÅ %
TableOperationAuditData
ÅÅ )
SingleTable
ÅÅ* 5
(
ÅÅ5 6
Guid
ÇÇ 
tableId
ÇÇ 
,
ÇÇ 
Guid
ÉÉ 
?
ÉÉ 
	sessionId
ÉÉ 
,
ÉÉ 
Money
ÑÑ 
charge
ÑÑ 
,
ÑÑ 
int
ÖÖ 

guestCount
ÖÖ 
,
ÖÖ 
IEnumerable
ÜÜ 
<
ÜÜ 
Guid
ÜÜ 
>
ÜÜ 
?
ÜÜ 
equipmentIds
ÜÜ '
=
ÜÜ( )
null
ÜÜ* .
,
ÜÜ. /
IEnumerable
áá 
<
áá 
Guid
áá 
>
áá 
?
áá 
	serverIds
áá $
=
áá% &
null
áá' +
,
áá+ ,
IDictionary
àà 
<
àà 
string
àà 
,
àà 
object
àà "
>
àà" #
?
àà# $
	stateData
àà% .
=
àà/ 0
null
àà1 5
)
àà5 6
{
ââ 
return
ää 
new
ää %
TableOperationAuditData
ää *
(
ää* +
new
ãã 
[
ãã 
]
ãã 
{
ãã 
tableId
ãã 
}
ãã 
,
ãã 
	sessionId
åå 
.
åå 
HasValue
åå 
?
åå  
new
åå! $
[
åå$ %
]
åå% &
{
åå' (
	sessionId
åå) 2
.
åå2 3
Value
åå3 8
}
åå9 :
:
åå; <
Array
åå= B
.
ååB C
Empty
ååC H
<
ååH I
Guid
ååI M
>
ååM N
(
ååN O
)
ååO P
,
ååP Q
charge
çç 
,
çç 

guestCount
éé 
,
éé 
equipmentIds
èè 
?
èè 
.
èè 
ToList
èè  
(
èè  !
)
èè! "
??
èè# %
new
èè& )
List
èè* .
<
èè. /
Guid
èè/ 3
>
èè3 4
(
èè4 5
)
èè5 6
,
èè6 7
	serverIds
êê 
?
êê 
.
êê 
ToList
êê 
(
êê 
)
êê 
??
êê  "
new
êê# &
List
êê' +
<
êê+ ,
Guid
êê, 0
>
êê0 1
(
êê1 2
)
êê2 3
,
êê3 4
	stateData
ëë 
?
ëë 
.
ëë 
ToDictionary
ëë #
(
ëë# $
kvp
ëë$ '
=>
ëë( *
kvp
ëë+ .
.
ëë. /
Key
ëë/ 2
,
ëë2 3
kvp
ëë4 7
=>
ëë8 :
kvp
ëë; >
.
ëë> ?
Value
ëë? D
)
ëëD E
??
ëëF H
new
ëëI L

Dictionary
ëëM W
<
ëëW X
string
ëëX ^
,
ëë^ _
object
ëë` f
>
ëëf g
(
ëëg h
)
ëëh i
)
íí 	
;
íí	 

}
ìì 
public
†† 

static
†† %
TableOperationAuditData
†† )
MultipleTables
††* 8
(
††8 9
IEnumerable
°° 
<
°° 
Guid
°° 
>
°° 
tableIds
°° "
,
°°" #
IEnumerable
¢¢ 
<
¢¢ 
Guid
¢¢ 
>
¢¢ 

sessionIds
¢¢ $
,
¢¢$ %
Money
££ 
totalCharge
££ 
,
££ 
int
§§ 
totalGuestCount
§§ 
,
§§ 
IEnumerable
•• 
<
•• 
Guid
•• 
>
•• 
?
•• 
equipmentIds
•• '
=
••( )
null
••* .
,
••. /
IEnumerable
¶¶ 
<
¶¶ 
Guid
¶¶ 
>
¶¶ 
?
¶¶ 
	serverIds
¶¶ $
=
¶¶% &
null
¶¶' +
,
¶¶+ ,
IDictionary
ßß 
<
ßß 
string
ßß 
,
ßß 
object
ßß "
>
ßß" #
?
ßß# $
	stateData
ßß% .
=
ßß/ 0
null
ßß1 5
)
ßß5 6
{
®® 
return
©© 
new
©© %
TableOperationAuditData
©© *
(
©©* +
tableIds
™™ 
?
™™ 
.
™™ 
ToList
™™ 
(
™™ 
)
™™ 
??
™™ !
new
™™" %
List
™™& *
<
™™* +
Guid
™™+ /
>
™™/ 0
(
™™0 1
)
™™1 2
,
™™2 3

sessionIds
´´ 
?
´´ 
.
´´ 
ToList
´´ 
(
´´ 
)
´´  
??
´´! #
new
´´$ '
List
´´( ,
<
´´, -
Guid
´´- 1
>
´´1 2
(
´´2 3
)
´´3 4
,
´´4 5
totalCharge
¨¨ 
,
¨¨ 
totalGuestCount
≠≠ 
,
≠≠ 
equipmentIds
ÆÆ 
?
ÆÆ 
.
ÆÆ 
ToList
ÆÆ  
(
ÆÆ  !
)
ÆÆ! "
??
ÆÆ# %
new
ÆÆ& )
List
ÆÆ* .
<
ÆÆ. /
Guid
ÆÆ/ 3
>
ÆÆ3 4
(
ÆÆ4 5
)
ÆÆ5 6
,
ÆÆ6 7
	serverIds
ØØ 
?
ØØ 
.
ØØ 
ToList
ØØ 
(
ØØ 
)
ØØ 
??
ØØ  "
new
ØØ# &
List
ØØ' +
<
ØØ+ ,
Guid
ØØ, 0
>
ØØ0 1
(
ØØ1 2
)
ØØ2 3
,
ØØ3 4
	stateData
∞∞ 
?
∞∞ 
.
∞∞ 
ToDictionary
∞∞ #
(
∞∞# $
kvp
∞∞$ '
=>
∞∞( *
kvp
∞∞+ .
.
∞∞. /
Key
∞∞/ 2
,
∞∞2 3
kvp
∞∞4 7
=>
∞∞8 :
kvp
∞∞; >
.
∞∞> ?
Value
∞∞? D
)
∞∞D E
??
∞∞F H
new
∞∞I L

Dictionary
∞∞M W
<
∞∞W X
string
∞∞X ^
,
∞∞^ _
object
∞∞` f
>
∞∞f g
(
∞∞g h
)
∞∞h i
)
±± 	
;
±±	 

}
≤≤ 
public
∑∑ 

int
∑∑ 

TableCount
∑∑ 
=>
∑∑ 
TableIds
∑∑ %
.
∑∑% &
Count
∑∑& +
;
∑∑+ ,
public
ºº 

int
ºº 
SessionCount
ºº 
=>
ºº 

SessionIds
ºº )
.
ºº) *
Count
ºº* /
;
ºº/ 0
public
¡¡ 

bool
¡¡ 
IsMergedState
¡¡ 
=>
¡¡  
TableIds
¡¡! )
.
¡¡) *
Count
¡¡* /
>
¡¡0 1
$num
¡¡2 3
&&
¡¡4 6

SessionIds
¡¡7 A
.
¡¡A B
Count
¡¡B G
==
¡¡H J
$num
¡¡K L
;
¡¡L M
public
∆∆ 

bool
∆∆ 
IsIndividualState
∆∆ !
=>
∆∆" $
TableIds
∆∆% -
.
∆∆- .
Count
∆∆. 3
==
∆∆4 6

SessionIds
∆∆7 A
.
∆∆A B
Count
∆∆B G
;
∆∆G H
}«« Èc
ÄC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\TableMergeValidationResult.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public

 
record

 &
TableMergeValidationResult

 (
(

( )
bool 
IsValid	 
, 
IReadOnlyList 
< 
string 
> 
ValidationErrors *
,* +
IReadOnlyList 
< 
string 
> 
? 
Warnings #
=$ %
null& *
) 
{ 
public 

static &
TableMergeValidationResult ,
Valid- 2
(2 3
IEnumerable3 >
<> ?
string? E
>E F
?F G
warningsH P
=Q R
nullS W
)W X
{ 
return 
new &
TableMergeValidationResult -
(- .
true 
, 
Array 
. 
Empty 
< 
string 
> 
(  
)  !
,! "
warnings 
? 
. 
ToList 
( 
) 
?? !
new" %
List& *
<* +
string+ 1
>1 2
(2 3
)3 4
) 	
;	 

} 
public$$ 

static$$ &
TableMergeValidationResult$$ ,
Invalid$$- 4
($$4 5
IEnumerable%% 
<%% 
string%% 
>%% 
errors%% "
,%%" #
IEnumerable&& 
<&& 
string&& 
>&& 
?&& 
warnings&& %
=&&& '
null&&( ,
)&&, -
{'' 
if(( 

((( 
errors(( 
==(( 
null(( 
||(( 
!(( 
errors(( %
.((% &
Any((& )
((() *
)((* +
)((+ ,
{)) 	
throw** 
new** 
ArgumentException** '
(**' (
$str**( \
,**\ ]
nameof**^ d
(**d e
errors**e k
)**k l
)**l m
;**m n
}++ 	
return-- 
new-- &
TableMergeValidationResult-- -
(--- .
false.. 
,.. 
errors// 
.// 
ToList// 
(// 
)// 
,// 
warnings00 
?00 
.00 
ToList00 
(00 
)00 
??00 !
new00" %
List00& *
<00* +
string00+ 1
>001 2
(002 3
)003 4
)11 	
;11	 

}22 
public99 

static99 &
TableMergeValidationResult99 ,
SingleError99- 8
(998 9
string999 ?
error99@ E
)99E F
{:: 
if;; 

(;; 
string;; 
.;; 
IsNullOrWhiteSpace;; %
(;;% &
error;;& +
);;+ ,
);;, -
{<< 	
throw== 
new== 
ArgumentException== '
(==' (
$str==( H
,==H I
nameof==J P
(==P Q
error==Q V
)==V W
)==W X
;==X Y
}>> 	
return@@ 
new@@ &
TableMergeValidationResult@@ -
(@@- .
false@@. 3
,@@3 4
new@@5 8
[@@8 9
]@@9 :
{@@; <
error@@= B
}@@C D
)@@D E
;@@E F
}AA 
publicFF 

intFF 
TotalIssueCountFF 
=>FF !
ValidationErrorsFF" 2
.FF2 3
CountFF3 8
+FF9 :
(FF; <
WarningsFF< D
?FFD E
.FFE F
CountFFF K
??FFL N
$numFFO P
)FFP Q
;FFQ R
publicKK 

boolKK 
HasWarningsKK 
=>KK 
WarningsKK '
!=KK( *
nullKK+ /
&&KK0 2
WarningsKK3 ;
.KK; <
AnyKK< ?
(KK? @
)KK@ A
;KKA B
publicQQ 

stringQQ 
GetFormattedIssuesQQ $
(QQ$ %
)QQ% &
{RR 
varSS 
issuesSS 
=SS 
newSS 
ListSS 
<SS 
stringSS $
>SS$ %
(SS% &
)SS& '
;SS' (
ifUU 

(UU 
ValidationErrorsUU 
.UU 
AnyUU  
(UU  !
)UU! "
)UU" #
{VV 	
issuesWW 
.WW 
AddWW 
(WW 
$strWW  
)WW  !
;WW! "
issuesXX 
.XX 
AddRangeXX 
(XX 
ValidationErrorsXX ,
.XX, -
SelectXX- 3
(XX3 4
eXX4 5
=>XX6 8
$"XX9 ;
$strXX; ?
{XX? @
eXX@ A
}XXA B
"XXB C
)XXC D
)XXD E
;XXE F
}YY 	
if[[ 

([[ 
HasWarnings[[ 
)[[ 
{\\ 	
if]] 
(]] 
issues]] 
.]] 
Any]] 
(]] 
)]] 
)]] 
issues]] $
.]]$ %
Add]]% (
(]]( )
$str]]) +
)]]+ ,
;]], -
issues^^ 
.^^ 
Add^^ 
(^^ 
$str^^ "
)^^" #
;^^# $
issues__ 
.__ 
AddRange__ 
(__ 
Warnings__ $
.__$ %
Select__% +
(__+ ,
w__, -
=>__. 0
$"__1 3
$str__3 7
{__7 8
w__8 9
}__9 :
"__: ;
)__; <
)__< =
;__= >
}`` 	
returnbb 
stringbb 
.bb 
Joinbb 
(bb 
Environmentbb &
.bb& '
NewLinebb' .
,bb. /
issuesbb0 6
)bb6 7
;bb7 8
}cc 
}dd 
publicii 
recordii &
TableSplitValidationResultii (
(ii( )
booljj 
IsValidjj	 
,jj 
IReadOnlyListkk 
<kk 
stringkk 
>kk 
ValidationErrorskk *
,kk* +
IReadOnlyListll 
<ll 
stringll 
>ll 
?ll 
Warningsll #
=ll$ %
nullll& *
)mm 
{nn 
publictt 

statictt &
TableSplitValidationResulttt ,
Validtt- 2
(tt2 3
IEnumerablett3 >
<tt> ?
stringtt? E
>ttE F
?ttF G
warningsttH P
=ttQ R
nullttS W
)ttW X
{uu 
returnvv 
newvv &
TableSplitValidationResultvv -
(vv- .
trueww 
,ww 
Arrayxx 
.xx 
Emptyxx 
<xx 
stringxx 
>xx 
(xx  
)xx  !
,xx! "
warningsyy 
?yy 
.yy 
ToListyy 
(yy 
)yy 
??yy !
newyy" %
Listyy& *
<yy* +
stringyy+ 1
>yy1 2
(yy2 3
)yy3 4
)zz 	
;zz	 

}{{ 
public
ÉÉ 

static
ÉÉ (
TableSplitValidationResult
ÉÉ ,
Invalid
ÉÉ- 4
(
ÉÉ4 5
IEnumerable
ÑÑ 
<
ÑÑ 
string
ÑÑ 
>
ÑÑ 
errors
ÑÑ "
,
ÑÑ" #
IEnumerable
ÖÖ 
<
ÖÖ 
string
ÖÖ 
>
ÖÖ 
?
ÖÖ 
warnings
ÖÖ %
=
ÖÖ& '
null
ÖÖ( ,
)
ÖÖ, -
{
ÜÜ 
if
áá 

(
áá 
errors
áá 
==
áá 
null
áá 
||
áá 
!
áá 
errors
áá %
.
áá% &
Any
áá& )
(
áá) *
)
áá* +
)
áá+ ,
{
àà 	
throw
ââ 
new
ââ 
ArgumentException
ââ '
(
ââ' (
$str
ââ( \
,
ââ\ ]
nameof
ââ^ d
(
ââd e
errors
ââe k
)
ââk l
)
ââl m
;
ââm n
}
ää 	
return
åå 
new
åå (
TableSplitValidationResult
åå -
(
åå- .
false
çç 
,
çç 
errors
éé 
.
éé 
ToList
éé 
(
éé 
)
éé 
,
éé 
warnings
èè 
?
èè 
.
èè 
ToList
èè 
(
èè 
)
èè 
??
èè !
new
èè" %
List
èè& *
<
èè* +
string
èè+ 1
>
èè1 2
(
èè2 3
)
èè3 4
)
êê 	
;
êê	 

}
ëë 
public
òò 

static
òò (
TableSplitValidationResult
òò ,
SingleError
òò- 8
(
òò8 9
string
òò9 ?
error
òò@ E
)
òòE F
{
ôô 
if
öö 

(
öö 
string
öö 
.
öö  
IsNullOrWhiteSpace
öö %
(
öö% &
error
öö& +
)
öö+ ,
)
öö, -
{
õõ 	
throw
úú 
new
úú 
ArgumentException
úú '
(
úú' (
$str
úú( H
,
úúH I
nameof
úúJ P
(
úúP Q
error
úúQ V
)
úúV W
)
úúW X
;
úúX Y
}
ùù 	
return
üü 
new
üü (
TableSplitValidationResult
üü -
(
üü- .
false
üü. 3
,
üü3 4
new
üü5 8
[
üü8 9
]
üü9 :
{
üü; <
error
üü= B
}
üüC D
)
üüD E
;
üüE F
}
†† 
public
•• 

int
•• 
TotalIssueCount
•• 
=>
•• !
ValidationErrors
••" 2
.
••2 3
Count
••3 8
+
••9 :
(
••; <
Warnings
••< D
?
••D E
.
••E F
Count
••F K
??
••L N
$num
••O P
)
••P Q
;
••Q R
public
™™ 

bool
™™ 
HasWarnings
™™ 
=>
™™ 
Warnings
™™ '
!=
™™( *
null
™™+ /
&&
™™0 2
Warnings
™™3 ;
.
™™; <
Any
™™< ?
(
™™? @
)
™™@ A
;
™™A B
public
∞∞ 

string
∞∞  
GetFormattedIssues
∞∞ $
(
∞∞$ %
)
∞∞% &
{
±± 
var
≤≤ 
issues
≤≤ 
=
≤≤ 
new
≤≤ 
List
≤≤ 
<
≤≤ 
string
≤≤ $
>
≤≤$ %
(
≤≤% &
)
≤≤& '
;
≤≤' (
if
¥¥ 

(
¥¥ 
ValidationErrors
¥¥ 
.
¥¥ 
Any
¥¥  
(
¥¥  !
)
¥¥! "
)
¥¥" #
{
µµ 	
issues
∂∂ 
.
∂∂ 
Add
∂∂ 
(
∂∂ 
$str
∂∂  
)
∂∂  !
;
∂∂! "
issues
∑∑ 
.
∑∑ 
AddRange
∑∑ 
(
∑∑ 
ValidationErrors
∑∑ ,
.
∑∑, -
Select
∑∑- 3
(
∑∑3 4
e
∑∑4 5
=>
∑∑6 8
$"
∑∑9 ;
$str
∑∑; ?
{
∑∑? @
e
∑∑@ A
}
∑∑A B
"
∑∑B C
)
∑∑C D
)
∑∑D E
;
∑∑E F
}
∏∏ 	
if
∫∫ 

(
∫∫ 
HasWarnings
∫∫ 
)
∫∫ 
{
ªª 	
if
ºº 
(
ºº 
issues
ºº 
.
ºº 
Any
ºº 
(
ºº 
)
ºº 
)
ºº 
issues
ºº $
.
ºº$ %
Add
ºº% (
(
ºº( )
$str
ºº) +
)
ºº+ ,
;
ºº, -
issues
ΩΩ 
.
ΩΩ 
Add
ΩΩ 
(
ΩΩ 
$str
ΩΩ "
)
ΩΩ" #
;
ΩΩ# $
issues
ææ 
.
ææ 
AddRange
ææ 
(
ææ 
Warnings
ææ $
.
ææ$ %
Select
ææ% +
(
ææ+ ,
w
ææ, -
=>
ææ. 0
$"
ææ1 3
$str
ææ3 7
{
ææ7 8
w
ææ8 9
}
ææ9 :
"
ææ: ;
)
ææ; <
)
ææ< =
;
ææ= >
}
øø 	
return
¡¡ 
string
¡¡ 
.
¡¡ 
Join
¡¡ 
(
¡¡ 
Environment
¡¡ &
.
¡¡& '
NewLine
¡¡' .
,
¡¡. /
issues
¡¡0 6
)
¡¡6 7
;
¡¡7 8
}
¬¬ 
}√√ Ô,
vC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\TableMergeStatus.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public		 
record		 
TableMergeStatus		 
(		 
Guid

 
TableId

	 
,

 
bool 
IsMerged	 
, 
Guid 
? 	
MergedSessionId
 
= 
null  
,  !
Guid 
? 	
PrimaryTableId
 
= 
null 
,  
IReadOnlyList 
< 
Guid 
> 
? 
MergedTableIds '
=( )
null* .
,. /
DateTime 
? 
MergeTimestamp 
= 
null #
,# $
string 

?
 
MergeReason 
= 
null 
, 
Guid 
? 	
MergedByStaffId
 
= 
null  
) 
{ 
public 

static 
TableMergeStatus "
	NotMerged# ,
(, -
Guid- 1
tableId2 9
)9 :
{ 
if 

( 
tableId 
== 
Guid 
. 
Empty !
)! "
{ 	
throw 
new 
ArgumentException '
(' (
$str( C
,C D
nameofE K
(K L
tableIdL S
)S T
)T U
;U V
} 	
return   
new   
TableMergeStatus   #
(  # $
tableId  $ +
,  + ,
false  - 2
)  2 3
;  3 4
}!! 
public.. 

static.. 
TableMergeStatus.. "
Merged..# )
(..) *
Guid// 
tableId// 
,// 
Guid00 
mergedSessionId00 
,00 
Guid11 
primaryTableId11 
,11 
IEnumerable22 
<22 
Guid22 
>22 
mergedTableIds22 (
,22( )
DateTime33 
mergeTimestamp33 
,33  
string44 
mergeReason44 
,44 
Guid55 
mergedByStaffId55 
)55 
{66 
if77 

(77 
tableId77 
==77 
Guid77 
.77 
Empty77 !
)77! "
{88 	
throw99 
new99 
ArgumentException99 '
(99' (
$str99( C
,99C D
nameof99E K
(99K L
tableId99L S
)99S T
)99T U
;99U V
}:: 	
if<< 

(<< 
mergedSessionId<< 
==<< 
Guid<< #
.<<# $
Empty<<$ )
)<<) *
{== 	
throw>> 
new>> 
ArgumentException>> '
(>>' (
$str>>( L
,>>L M
nameof>>N T
(>>T U
mergedSessionId>>U d
)>>d e
)>>e f
;>>f g
}?? 	
ifAA 

(AA 
primaryTableIdAA 
==AA 
GuidAA "
.AA" #
EmptyAA# (
)AA( )
{BB 	
throwCC 
newCC 
ArgumentExceptionCC '
(CC' (
$strCC( K
,CCK L
nameofCCM S
(CCS T
primaryTableIdCCT b
)CCb c
)CCc d
;CCd e
}DD 	
ifFF 

(FF 
mergedByStaffIdFF 
==FF 
GuidFF #
.FF# $
EmptyFF$ )
)FF) *
{GG 	
throwHH 
newHH 
ArgumentExceptionHH '
(HH' (
$strHH( C
,HHC D
nameofHHE K
(HHK L
mergedByStaffIdHHL [
)HH[ \
)HH\ ]
;HH] ^
}II 	
ifKK 

(KK 
stringKK 
.KK 
IsNullOrWhiteSpaceKK %
(KK% &
mergeReasonKK& 1
)KK1 2
)KK2 3
{LL 	
throwMM 
newMM 
ArgumentExceptionMM '
(MM' (
$strMM( G
,MMG H
nameofMMI O
(MMO P
mergeReasonMMP [
)MM[ \
)MM\ ]
;MM] ^
}NN 	
returnPP 
newPP 
TableMergeStatusPP #
(PP# $
tableIdQQ 
,QQ 
trueRR 
,RR 
mergedSessionIdSS 
,SS 
primaryTableIdTT 
,TT 
mergedTableIdsUU 
?UU 
.UU 
ToListUU "
(UU" #
)UU# $
,UU$ %
mergeTimestampVV 
,VV 
mergeReasonWW 
,WW 
mergedByStaffIdXX 
)YY 	
;YY	 

}ZZ 
public__ 

bool__ 
IsPrimaryTable__ 
=>__ !
IsMerged__" *
&&__+ -
PrimaryTableId__. <
==__= ?
TableId__@ G
;__G H
publicdd 

booldd 
IsSecondaryTabledd  
=>dd! #
IsMergeddd$ ,
&&dd- /
PrimaryTableIddd0 >
!=dd? A
TableIdddB I
;ddI J
publicii 

intii 
MergedTableCountii 
=>ii  "
MergedTableIdsii# 1
?ii1 2
.ii2 3
Countii3 8
??ii9 ;
(ii< =
IsMergedii= E
?iiF G
$numiiH I
:iiJ K
$numiiL M
)iiM N
;iiN O
}jj è
wC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\SplitPaymentEntry.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public 
record 
SplitPaymentEntry 
{		 
public

 

PaymentType

 
Method

 
{

 
get

  #
;

# $
init

% )
;

) *
}

+ ,
public 

Money 
Amount 
{ 
get 
; 
init #
;# $
}% &
public 

SplitPaymentEntry 
( 
PaymentType (
method) /
,/ 0
Money1 6
amount7 =
)= >
{ 
if 

( 
amount 
<= 
Money 
. 
Zero  
(  !
)! "
)" #
{ 	
throw 
new 
ArgumentException '
(' (
$str( J
,J K
nameofL R
(R S
amountS Y
)Y Z
)Z [
;[ \
} 	
Method 
= 
method 
; 
Amount 
= 
amount 
; 
} 
} È
zC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\SessionControlResult.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public

 
record

  
SessionControlResult

 "
(

" #
bool 
IsSuccessful	 
, 
string 

?
 
ErrorMessage 
= 
null 
,  
SessionControlData 
? 
Data 
= 
null #
) 
{ 
public 

static  
SessionControlResult &
Success' .
(. /
SessionControlData/ A
?A B
dataC G
=H I
nullJ N
)N O
=>P R
new 
( 
true 
, 
null 
, 
data 
) 
; 
public 

static  
SessionControlResult &
NotFound' /
(/ 0
)0 1
=>2 4
new 
( 
false 
, 
$str &
)& '
;' (
public 

static  
SessionControlResult &
InvalidState' 3
(3 4
string4 :
message; B
)B C
=>D F
new   
(   
false   
,   
message   
)   
;   
public%% 

static%%  
SessionControlResult%% &
Unauthorized%%' 3
(%%3 4
string%%4 :
message%%; B
=%%C D
$str%%E ]
)%%] ^
=>%%_ a
new&& 
(&& 
false&& 
,&& 
message&& 
)&& 
;&& 
public++ 

static++  
SessionControlResult++ &
ValidationError++' 6
(++6 7
string++7 =
message++> E
)++E F
=>++G I
new,, 
(,, 
false,, 
,,, 
message,, 
),, 
;,, 
}-- 
public22 
record22 
SessionControlData22  
(22  !
Guid33 
	SessionId33	 
,33 
TableSessionStatus44 
Status44 
,44 
DateTime55 
?55 
PausedAt55 
,55 
TimeSpan66 
TotalPausedDuration66  
,66  !
Money77 	
CurrentCharge77
 
)88 
;88 –*
wC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\SessionAuditEntry.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public 
record 
SessionAuditEntry 
(  
Guid		 
	SessionId			 
,		 
string

 

Action

 
,

 
string 

Details 
, 
Guid 
UserId	 
, 
DateTime 
	Timestamp 
) 
{ 
public 

static 
SessionAuditEntry #
Create$ *
(* +
Guid+ /
	sessionId0 9
,9 :
string; A
actionB H
,H I
stringJ P
detailsQ X
,X Y
GuidZ ^
userId_ e
)e f
{ 
return 
new 
SessionAuditEntry $
($ %
	sessionId% .
,. /
action0 6
,6 7
details8 ?
,? @
userIdA G
,G H
DateTimeI Q
.Q R
UtcNowR X
)X Y
;Y Z
} 
public%% 

static%% 
SessionAuditEntry%% #
SessionStarted%%$ 2
(%%2 3
Guid%%3 7
	sessionId%%8 A
,%%A B
Guid%%C G
userId%%H N
,%%N O
Guid%%P T
tableId%%U \
,%%\ ]
int%%^ a

guestCount%%b l
)%%l m
{&& 
return'' 
Create'' 
('' 
	sessionId'' 
,''  
$str''! 1
,''1 2
$"''3 5
$str''5 <
{''< =
tableId''= D
}''D E
$str''E O
{''O P

guestCount''P Z
}''Z [
"''[ \
,''\ ]
userId''^ d
)''d e
;''e f
}(( 
public11 

static11 
SessionAuditEntry11 #
SessionPaused11$ 1
(111 2
Guid112 6
	sessionId117 @
,11@ A
Guid11B F
userId11G M
,11M N
string11O U
reason11V \
)11\ ]
{22 
return33 
Create33 
(33 
	sessionId33 
,33  
$str33! 0
,330 1
$"332 4
$str334 <
{33< =
reason33= C
}33C D
"33D E
,33E F
userId33G M
)33M N
;33N O
}44 
public<< 

static<< 
SessionAuditEntry<< #
SessionResumed<<$ 2
(<<2 3
Guid<<3 7
	sessionId<<8 A
,<<A B
Guid<<C G
userId<<H N
)<<N O
{== 
return>> 
Create>> 
(>> 
	sessionId>> 
,>>  
$str>>! 1
,>>1 2
$str>>3 D
,>>D E
userId>>F L
)>>L M
;>>M N
}?? 
publicII 

staticII 
SessionAuditEntryII #
SessionEndedII$ 0
(II0 1
GuidII1 5
	sessionIdII6 ?
,II? @
GuidIIA E
userIdIIF L
,IIL M
decimalIIN U
totalChargeIIV a
,IIa b
TimeSpanIIc k
durationIIl t
)IIt u
{JJ 
returnKK 
CreateKK 
(KK 
	sessionIdKK 
,KK  
$strKK! /
,KK/ 0
$"KK1 3
$strKK3 =
{KK= >
durationKK> F
}KKF G
$strKKG Q
{KKQ R
totalChargeKKR ]
:KK] ^
$strKK^ _
}KK_ `
"KK` a
,KKa b
userIdKKc i
)KKi j
;KKj k
}LL 
publicVV 

staticVV 
SessionAuditEntryVV #
GuestCountUpdatedVV$ 5
(VV5 6
GuidVV6 :
	sessionIdVV; D
,VVD E
GuidVVF J
userIdVVK Q
,VVQ R
intVVS V
oldCountVVW _
,VV_ `
intVVa d
newCountVVe m
)VVm n
{WW 
returnXX 
CreateXX 
(XX 
	sessionIdXX 
,XX  
$strXX! 4
,XX4 5
$"XX6 8
$strXX8 >
{XX> ?
oldCountXX? G
}XXG H
$strXXH N
{XXN O
newCountXXO W
}XXW X
"XXX Y
,XXY Z
userIdXX[ a
)XXa b
;XXb c
}YY 
publicdd 

staticdd 
SessionAuditEntrydd #
SessionTransferreddd$ 6
(dd6 7
Guiddd7 ;
	sessionIddd< E
,ddE F
GuidddG K
userIdddL R
,ddR S
GuidddT X
fromTableIdddY d
,ddd e
Guidddf j
	toTableIdddk t
,ddt u
stringddv |
reason	dd} É
)
ddÉ Ñ
{ee 
returnff 
Createff 
(ff 
	sessionIdff 
,ff  
$strff! 5
,ff5 6
$"ff7 9
$strff9 ?
{ff? @
fromTableIdff@ K
}ffK L
$strffL R
{ffR S
	toTableIdffS \
}ff\ ]
$strff] g
{ffg h
reasonffh n
}ffn o
"ffo p
,ffp q
userIdffr x
)ffx y
;ffy z
}gg 
}hh ã

rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\SessionAlert.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public 
record 
SessionAlert 
( 
Guid		 
	SessionId			 
,		 
Guid

 
TableId

	 
,

 
SessionAlertType 
	AlertType 
, 
string 

Message 
, 
DateTime 
	CreatedAt 
,  
SessionAlertSeverity 
Severity !
=" # 
SessionAlertSeverity$ 8
.8 9
Medium9 ?
) 
; 
public 
enum 
SessionAlertType 
{ 
	LongPause 
, 
CapacityIssue 
, 
LongSession## 
,##  
EquipmentMaintenance(( 
,(( 
General-- 
}.. 
public33 
enum33  
SessionAlertSeverity33  
{44 
Low88 
,88 
Medium== 

,==
 
HighBB 
,BB 	
CriticalGG 
}HH ê
~C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\ServerPerformanceMetrics.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public 
record $
ServerPerformanceMetrics &
(& '
Guid		 
ServerId			 
,		 
string

 


ServerName

 
,

 
DateTime 
FromDate 
, 
DateTime 
ToDate 
, 
int 
TotalSessionsServed 
, 
TimeSpan 
TotalServiceTime 
, 
Money 	
TotalSalesGenerated
 
, 
Money 	
TotalTipsEarned
 
, 
decimal "
AverageSessionDuration "
," #
decimal %
CustomerSatisfactionScore %
,% &
int 
PrimarySessionCount 
, 
int !
SecondarySessionCount 
, 
Money 	 
AverageTipPerSession
 
, 
decimal 
SalesPerHour 
) 
{ 
public 

decimal  
AverageTipPercentage '
=>( *
TotalSalesGenerated 
. 
Amount "
># $
$num% &
? 
( 
TotalTipsEarned 
. 
Amount %
/& '
TotalSalesGenerated( ;
.; <
Amount< B
)B C
*D E
$numF I
: 
$num 
; 
public$$ 

decimal$$ 
SessionsPerHour$$ "
=>$$# %
TotalServiceTime%% 
.%% 

TotalHours%% #
>%%$ %
$num%%& '
?&& 
(&& 
decimal&& 
)&& 
(&& 
TotalSessionsServed&& +
/&&, -
TotalServiceTime&&. >
.&&> ?

TotalHours&&? I
)&&I J
:'' 
$num'' 
;'' 
}(( Ì
|C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\ServerAssignmentResult.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public 
record "
ServerAssignmentResult $
($ %
bool		 
IsSuccessful			 
,		 
string

 

?


 
ErrorMessage

 
=

 
null

 
,

   
ServerAssignmentData 
? 
Data 
=  
null! %
) 
{ 
public 

static "
ServerAssignmentResult (
Success) 0
(0 1 
ServerAssignmentData1 E
?E F
dataG K
=L M
nullN R
)R S
=>T V
new 
( 
true 
, 
null 
, 
data 
) 
; 
public 

static "
ServerAssignmentResult (
NotFound) 1
(1 2
string2 8

entityType9 C
=D E
$strF O
)O P
=>Q S
new 
( 
false 
, 
$" 
{ 

entityType  
}  !
$str! +
"+ ,
), -
;- .
public 

static "
ServerAssignmentResult (
InvalidOperation) 9
(9 :
string: @
messageA H
)H I
=>J L
new 
( 
false 
, 
message 
) 
; 
public 

static "
ServerAssignmentResult (
ValidationError) 8
(8 9
string9 ?
message@ G
)G H
=>I K
new 
( 
false 
, 
message 
) 
; 
} 
public 
record  
ServerAssignmentData "
(" #
Guid 
AssignmentId	 
, 
Guid   
	SessionId  	 
,   
Guid!! 
ServerId!!	 
,!! 
bool"" 
	IsPrimary""	 
,"" 
decimal##  
AllocationPercentage##  
,##  !
DateTime$$ 

AssignedAt$$ 
)%% 
;%% ÷
uC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\ServerAnalytics.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public		 
record		 
ServerAnalytics		 
(		 
Guid

 
ServerId

	 
,

 
string 


ServerName 
, 
DateTime 
FromDate 
, 
DateTime 
ToDate 
, $
ServerPerformanceMetrics 
PerformanceMetrics /
,/ 0
IReadOnlyList 
< 
DailyServerMetrics $
>$ %
DailyBreakdown& 4
,4 5!
CommissionCalculation 
CommissionData (
,( )
ServerRanking 
Ranking 
) 
; 
public 
record 
DailyServerMetrics  
(  !
DateTime 
Date 
, 
int 
SessionsServed 
, 
TimeSpan 
HoursWorked 
, 
Money 	
SalesGenerated
 
, 
Money 	

TipsEarned
 
, 
decimal 
AverageSessionValue 
) 
; 
public## 
record## !
CommissionCalculation## #
(### $
Money$$ 	

BaseSalary$$
 
,$$ 
Money%% 	
CommissionEarned%%
 
,%% 
decimal&& 
CommissionRate&& 
,&& 
Money'' 	
TotalCompensation''
 
,'' 
Money(( 	
BonusEligible((
 
))) 
;)) 
public.. 
record.. 
ServerRanking.. 
(.. 
int// 
	SalesRank// 
,// 
int00 
TipsRank00 
,00 
int11 
SessionCountRank11 
,11 
int22 $
CustomerSatisfactionRank22  
,22  !
int33 
OverallRank33 
,33 
int44 
TotalServers44 
)55 
;55 £
pC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\RecipeLine.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public 
class 

RecipeLine 
{ 
public 

Guid 
InventoryItemId 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public		 

decimal		 
Quantity		 
{		 
get		 !
;		! "
private		# *
set		+ .
;		. /
}		0 1
public 


RecipeLine 
( 
Guid 
inventoryItemId *
,* +
decimal, 3
quantity4 <
)< =
{ 
if 

( 
inventoryItemId 
== 
Guid #
.# $
Empty$ )
)) *
throw+ 0
new1 4
ArgumentException5 F
(F G
$strG b
)b c
;c d
if 

( 
quantity 
<= 
$num 
) 
throw  
new! $
ArgumentException% 6
(6 7
$str7 R
)R S
;S T
InventoryItemId 
= 
inventoryItemId )
;) *
Quantity 
= 
quantity 
; 
} 
} œ'
}C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\PricingSimulationResult.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public

 
sealed

 
record

 #
PricingSimulationResult

 ,
(

, -
Money 	

BaseCharge
 
, 
Money 	
FirstHourCharge
 
, 
Money 	 
RemainingHoursCharge
 
, 
Money 	 
MinimumChargeApplied
 
, 
Money 	
FinalCharge
 
, 
TimeSpan 
RoundedDuration 
, 
IReadOnlyList 
< 
string 
> 
AppliedRules &
) 
{ 
public 

static #
PricingSimulationResult )
CreateSimple* 6
(6 7
Money7 <
finalCharge= H
,H I
TimeSpanJ R
originalDurationS c
)c d
{ 
return 
new #
PricingSimulationResult *
(* +

BaseCharge 
: 
finalCharge #
,# $
FirstHourCharge 
: 
Money "
." #
Zero# '
(' (
)( )
,) * 
RemainingHoursCharge  
:  !
Money" '
.' (
Zero( ,
(, -
)- .
,. / 
MinimumChargeApplied    
:    !
Money  " '
.  ' (
Zero  ( ,
(  , -
)  - .
,  . /
FinalCharge!! 
:!! 
finalCharge!! $
,!!$ %
RoundedDuration"" 
:"" 
originalDuration"" -
,""- .
AppliedRules## 
:## 
new## 
List## "
<##" #
string### )
>##) *
{##+ ,
$str##- C
}##D E
)$$ 	
;$$	 

}%% 
public22 

static22 #
PricingSimulationResult22 )
CreateDetailed22* 8
(228 9
Money33 

baseCharge33 
,33 
Money44 
firstHourCharge44 
,44 
Money55  
remainingHoursCharge55 "
,55" #
Money66  
minimumChargeApplied66 "
,66" #
Money77 
finalCharge77 
,77 
TimeSpan88 
roundedDuration88  
,88  !
IReadOnlyList99 
<99 
string99 
>99 
appliedRules99 *
)99* +
{:: 
return;; 
new;; #
PricingSimulationResult;; *
(;;* +

BaseCharge<< 
:<< 

baseCharge<< "
,<<" #
FirstHourCharge== 
:== 
firstHourCharge== ,
,==, - 
RemainingHoursCharge>>  
:>>  ! 
remainingHoursCharge>>" 6
,>>6 7 
MinimumChargeApplied??  
:??  ! 
minimumChargeApplied??" 6
,??6 7
FinalCharge@@ 
:@@ 
finalCharge@@ $
,@@$ %
RoundedDurationAA 
:AA 
roundedDurationAA ,
,AA, -
AppliedRulesBB 
:BB 
appliedRulesBB &
??BB' )
newBB* -
ListBB. 2
<BB2 3
stringBB3 9
>BB9 :
(BB: ;
)BB; <
)CC 	
;CC	 

}DD 
publicJJ 

boolJJ #
WasMinimumChargeAppliedJJ '
(JJ' (
)JJ( )
{KK 
returnLL  
MinimumChargeAppliedLL #
.LL# $
AmountLL$ *
>LL+ ,
$numLL- .
;LL. /
}MM 
publicSS 

boolSS &
WasFirstHourPricingAppliedSS *
(SS* +
)SS+ ,
{TT 
returnUU 
FirstHourChargeUU 
.UU 
AmountUU %
>UU& '
$numUU( )
;UU) *
}VV 
public\\ 

Money\\ "
GetEffectiveHourlyRate\\ '
(\\' (
)\\( )
{]] 
if^^ 

(^^ 
RoundedDuration^^ 
.^^ 

TotalHours^^ &
<=^^' )
$num^^* +
)^^+ ,
{__ 	
return`` 
Money`` 
.`` 
Zero`` 
(`` 
)`` 
;``  
}aa 	
varcc 
hoursDecimalcc 
=cc 
(cc 
decimalcc #
)cc# $
RoundedDurationcc$ 3
.cc3 4

TotalHourscc4 >
;cc> ?
returndd 
FinalChargedd 
/dd 
hoursDecimaldd )
;dd) *
}ee 
publickk 

Moneykk 
RoundingAdjustmentkk #
=>kk$ &
FinalChargekk' 2
-kk3 4

BaseChargekk5 ?
;kk? @
}ll ô
uC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\PricingScenario.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public

 
sealed

 
record

 
PricingScenario

 $
(

$ %
TimeSpan 
Duration 
, 
	TableType 
	TableType 
, 
int 

GuestCount 
, 
DateTime 
	StartTime 
, 
bool 
HasMemberDiscount	 
= 
false "
) 
{ 
public 

static 
PricingScenario !
CreateBasic" -
(- .
TimeSpan. 6
duration7 ?
,? @
	TableTypeA J
	tableTypeK T
,T U
intV Y

guestCountZ d
)d e
{ 
return 
new 
PricingScenario "
(" #
Duration 
: 
duration 
, 
	TableType 
: 
	tableType  
,  !

GuestCount 
: 

guestCount "
," #
	StartTime 
: 
DateTime 
.  
UtcNow  &
)   	
;  	 

}!! 
public++ 

static++ 
PricingScenario++ !$
CreateWithMemberDiscount++" :
(++: ;
TimeSpan,, 
duration,, 
,,, 
	TableType-- 
	tableType-- 
,-- 
int.. 

guestCount.. 
,.. 
DateTime// 
	startTime// 
)// 
{00 
return11 
new11 
PricingScenario11 "
(11" #
Duration22 
:22 
duration22 
,22 
	TableType33 
:33 
	tableType33  
,33  !

GuestCount44 
:44 

guestCount44 "
,44" #
	StartTime55 
:55 
	startTime55  
,55  !
HasMemberDiscount66 
:66 
true66 #
)77 	
;77	 

}88 
}99 Ñ
tC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\OverrideResult.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public

 
sealed

 
record

 
OverrideResult

 #
(

# $
bool 
IsSuccessful	 
, 
string 

?
 
ErrorMessage 
= 
null 
,  
OverrideData 
? 
Data 
= 
null 
) 
{ 
public 

static 
OverrideResult  
Success! (
(( )
OverrideData) 5
?5 6
data7 ;
=< =
null> B
)B C
=>D F
new 
( 
true 
, 
null 
, 
data 
) 
; 
public 

static 
OverrideResult  
Unauthorized! -
(- .
). /
=>0 2
new 
( 
false 
, 
$str 3
)3 4
;4 5
public## 

static## 
OverrideResult##  
NotFound##! )
(##) *
)##* +
=>##, .
new$$ 
($$ 
false$$ 
,$$ 
$str$$ &
)$$& '
;$$' (
public++ 

static++ 
OverrideResult++  
InvalidOperation++! 1
(++1 2
string++2 8
message++9 @
)++@ A
=>++B D
new,, 
(,, 
false,, 
,,, 
message,, 
),, 
;,, 
public33 

static33 
OverrideResult33  
ValidationError33! 0
(330 1
string331 7
message338 ?
)33? @
=>33A C
new44 
(44 
false44 
,44 
$"44 
$str44 '
{44' (
message44( /
}44/ 0
"440 1
)441 2
;442 3
}55 
public:: 
sealed:: 
record:: 
OverrideData:: !
(::! "
Guid;; 
	SessionId;;	 
,;; 
OverrideType<< 
OverrideType<< 
,<< 
string== 

OriginalValue== 
,== 
string>> 

NewValue>> 
,>> 
Guid?? 
	ManagerId??	 
,?? 
DateTime@@ 
	Timestamp@@ 
)AA 
{BB 
publicLL 

staticLL 
OverrideDataLL 
CreateLL %
(LL% &
GuidMM 
	sessionIdMM 
,MM 
OverrideTypeNN 
overrideTypeNN !
,NN! "
stringOO 
originalValueOO 
,OO 
stringPP 
newValuePP 
,PP 
GuidQQ 
	managerIdQQ 
)QQ 
{RR 
returnSS 
newSS 
OverrideDataSS 
(SS  
	SessionIdTT 
:TT 
	sessionIdTT  
,TT  !
OverrideTypeUU 
:UU 
overrideTypeUU &
,UU& '
OriginalValueVV 
:VV 
originalValueVV (
,VV( )
NewValueWW 
:WW 
newValueWW 
,WW 
	ManagerIdXX 
:XX 
	managerIdXX  
,XX  !
	TimestampYY 
:YY 
DateTimeYY 
.YY  
UtcNowYY  &
)ZZ 	
;ZZ	 

}[[ 
}\\ Ä4
xC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\OverrideAuditEntry.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public

 
sealed

 
record

 
OverrideAuditEntry

 '
(

' (
Guid 
Id	 
, 
Guid 
	SessionId	 
, 
OverrideType 
OverrideType 
, 
string 

OriginalValue 
, 
string 

NewValue 
, 
string 

Reason 
, 
Guid 
	ManagerId	 
, 
DateTime 
	Timestamp 
) 
{ 
public 

static 
OverrideAuditEntry $
Create% +
(+ ,
Guid   
	sessionId   
,   
OverrideType!! 
overrideType!! !
,!!! "
string"" 
originalValue"" 
,"" 
string## 
newValue## 
,## 
string$$ 
reason$$ 
,$$ 
Guid%% 
	managerId%% 
)%% 
{&& 
if'' 

('' 
	sessionId'' 
=='' 
Guid'' 
.'' 
Empty'' #
)''# $
{(( 	
throw)) 
new)) 
ArgumentException)) '
())' (
$str))( E
,))E F
nameof))G M
())M N
	sessionId))N W
)))W X
)))X Y
;))Y Z
}** 	
if,, 

(,, 
	managerId,, 
==,, 
Guid,, 
.,, 
Empty,, #
),,# $
{-- 	
throw.. 
new.. 
ArgumentException.. '
(..' (
$str..( E
,..E F
nameof..G M
(..M N
	managerId..N W
)..W X
)..X Y
;..Y Z
}// 	
if11 

(11 
string11 
.11 
IsNullOrWhiteSpace11 %
(11% &
reason11& ,
)11, -
)11- .
{22 	
throw33 
new33 
ArgumentException33 '
(33' (
$str33( A
,33A B
nameof33C I
(33I J
reason33J P
)33P Q
)33Q R
;33R S
}44 	
return66 
new66 
OverrideAuditEntry66 %
(66% &
Id77 
:77 
Guid77 
.77 
NewGuid77 
(77 
)77 
,77 
	SessionId88 
:88 
	sessionId88  
,88  !
OverrideType99 
:99 
overrideType99 &
,99& '
OriginalValue:: 
::: 
originalValue:: (
??::) +
string::, 2
.::2 3
Empty::3 8
,::8 9
NewValue;; 
:;; 
newValue;; 
??;; !
string;;" (
.;;( )
Empty;;) .
,;;. /
Reason<< 
:<< 
reason<< 
.<< 
Trim<< 
(<<  
)<<  !
,<<! "
	ManagerId== 
:== 
	managerId==  
,==  !
	Timestamp>> 
:>> 
DateTime>> 
.>>  
UtcNow>>  &
)?? 	
;??	 

}@@ 
publicFF 

stringFF 
GetDescriptionFF  
(FF  !
)FF! "
{GG 
returnHH 
OverrideTypeHH 
switchHH "
{II 	
OverrideTypeJJ 
.JJ 
TimeAdjustmentJJ '
=>JJ( *
$"JJ+ -
$strJJ- @
{JJ@ A
OriginalValueJJA N
}JJN O
$strJJO S
{JJS T
NewValueJJT \
}JJ\ ]
"JJ] ^
,JJ^ _
OverrideTypeKK 
.KK 
PricingOverrideKK (
=>KK) +
$"KK, .
$strKK. D
{KKD E
OriginalValueKKE R
}KKR S
$strKKS W
{KKW X
NewValueKKX `
}KK` a
"KKa b
,KKb c
OverrideTypeLL 
.LL 
ForceEndSessionLL (
=>LL) +
$"LL, .
$strLL. G
{LLG H
OriginalValueLLH U
}LLU V
$strLLV W
"LLW X
,LLX Y
OverrideTypeMM 
.MM 
GuestCountOverrideMM +
=>MM, .
$"MM/ 1
$strMM1 J
{MMJ K
OriginalValueMMK X
}MMX Y
$strMMY ]
{MM] ^
NewValueMM^ f
}MMf g
"MMg h
,MMh i
OverrideTypeNN 
.NN 
RateOverrideNN %
=>NN& (
$"NN) +
$strNN+ D
{NND E
OriginalValueNNE R
}NNR S
$strNNS W
{NNW X
NewValueNNX `
}NN` a
"NNa b
,NNb c
_OO 
=>OO 
$"OO 
$strOO 
{OO 
OriginalValueOO +
}OO+ ,
$strOO, /
{OO/ 0
NewValueOO0 8
}OO8 9
"OO9 :
}PP 	
;PP	 

}QQ 
publicWW 

boolWW 
IsSignificantChangeWW #
(WW# $
)WW$ %
{XX 
returnYY 
OverrideTypeYY 
switchYY "
{ZZ 	
OverrideType[[ 
.[[ 
PricingOverride[[ (
=>[[) +
true[[, 0
,[[0 1
OverrideType\\ 
.\\ 
ForceEndSession\\ (
=>\\) +
true\\, 0
,\\0 1
OverrideType]] 
.]] 
RateOverride]] %
=>]]& (
true]]) -
,]]- .
OverrideType^^ 
.^^ 
TimeAdjustment^^ '
=>^^( *
!^^+ ,
string^^, 2
.^^2 3
Equals^^3 9
(^^9 :
OriginalValue^^: G
,^^G H
NewValue^^I Q
,^^Q R
StringComparison^^S c
.^^c d
OrdinalIgnoreCase^^d u
)^^u v
,^^v w
OverrideType__ 
.__ 
GuestCountOverride__ +
=>__, .
!__/ 0
string__0 6
.__6 7
Equals__7 =
(__= >
OriginalValue__> K
,__K L
NewValue__M U
,__U V
StringComparison__W g
.__g h
OrdinalIgnoreCase__h y
)__y z
,__z {
_`` 
=>`` 
true`` 
}aa 	
;aa	 

}bb 
}cc ≥c
kC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\Money.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public		 
sealed		 
record		 
Money		 
:		 
IComparable		 (
<		( )
Money		) .
>		. /
,		/ 0
IComparable		1 <
{

 
private 
const 
int 
DecimalPlaces #
=$ %
$num& '
;' (
private 
const 
string 
DefaultCurrency (
=) *
$str+ 0
;0 1
public 

decimal 
Amount 
{ 
get 
;  
}! "
public 

string 
Currency 
{ 
get  
;  !
}" #
public 

Money 
( 
decimal 
amount 
,  
string! '
currency( 0
=1 2
DefaultCurrency3 B
)B C
{ 
if   

(   
amount   
<   
$num   
)   
{!! 	
throw"" 
new"" 
ArgumentException"" '
(""' (
$str""( J
,""J K
nameof""L R
(""R S
amount""S Y
)""Y Z
)""Z [
;""[ \
}## 	
if(( 

((( 
string(( 
.(( 
IsNullOrWhiteSpace(( %
(((% &
currency((& .
)((. /
)((/ 0
{)) 	
currency** 
=** 
DefaultCurrency** &
;**& '
}++ 	
Amount.. 
=.. 
Math.. 
... 
Round.. 
(.. 
amount.. "
,.." #
DecimalPlaces..$ 1
,..1 2
MidpointRounding..3 C
...C D
AwayFromZero..D P
)..P Q
;..Q R
Currency// 
=// 
currency// 
.// 
ToUpperInvariant// ,
(//, -
)//- .
;//. /
}00 
public77 

static77 
Money77 
Zero77 
(77 
string77 #
currency77$ ,
=77- .
DefaultCurrency77/ >
)77> ?
=>77@ B
new77C F
(77F G
$num77G I
,77I J
currency77K S
)77S T
;77T U
public@@ 

static@@ 
Money@@ 
operator@@  
+@@! "
(@@" #
Money@@# (
left@@) -
,@@- .
Money@@/ 4
right@@5 :
)@@: ;
{AA 
ifBB 

(BB 
leftBB 
.BB 
CurrencyBB 
!=BB 
rightBB "
.BB" #
CurrencyBB# +
)BB+ ,
{CC 	
throwDD 
newDD %
InvalidOperationExceptionDD /
(DD/ 0
$"DD0 2
$strDD2 ^
{DD^ _
leftDD_ c
.DDc d
CurrencyDDd l
}DDl m
$strDDm r
{DDr s
rightDDs x
.DDx y
Currency	DDy Å
}
DDÅ Ç
$str
DDÇ É
"
DDÉ Ñ
)
DDÑ Ö
;
DDÖ Ü
}EE 	
returnGG 
newGG 
MoneyGG 
(GG 
leftGG 
.GG 
AmountGG $
+GG% &
rightGG' ,
.GG, -
AmountGG- 3
,GG3 4
leftGG5 9
.GG9 :
CurrencyGG: B
)GGB C
;GGC D
}HH 
publicQQ 

staticQQ 
MoneyQQ 
operatorQQ  
-QQ! "
(QQ" #
MoneyQQ# (
leftQQ) -
,QQ- .
MoneyQQ/ 4
rightQQ5 :
)QQ: ;
{RR 
ifSS 

(SS 
leftSS 
.SS 
CurrencySS 
!=SS 
rightSS "
.SS" #
CurrencySS# +
)SS+ ,
{TT 	
throwUU 
newUU %
InvalidOperationExceptionUU /
(UU/ 0
$"UU0 2
$strUU2 c
{UUc d
leftUUd h
.UUh i
CurrencyUUi q
}UUq r
$strUUr w
{UUw x
rightUUx }
.UU} ~
Currency	UU~ Ü
}
UUÜ á
$str
UUá à
"
UUà â
)
UUâ ä
;
UUä ã
}VV 	
varXX 
resultXX 
=XX 
leftXX 
.XX 
AmountXX  
-XX! "
rightXX# (
.XX( )
AmountXX) /
;XX/ 0
ifYY 

(YY 
resultYY 
<YY 
$numYY 
)YY 
{ZZ 	
throw[[ 
new[[ %
InvalidOperationException[[ /
([[/ 0
$str[[0 ^
)[[^ _
;[[_ `
}\\ 	
return^^ 
new^^ 
Money^^ 
(^^ 
result^^ 
,^^  
left^^! %
.^^% &
Currency^^& .
)^^. /
;^^/ 0
}__ 
publichh 

statichh 
Moneyhh 
operatorhh  
*hh! "
(hh" #
Moneyhh# (
moneyhh) .
,hh. /
decimalhh0 7
factorhh8 >
)hh> ?
{ii 
ifjj 

(jj 
factorjj 
<jj 
$numjj 
)jj 
{kk 	
throwll 
newll 
ArgumentExceptionll '
(ll' (
$strll( S
,llS T
nameofllU [
(ll[ \
factorll\ b
)llb c
)llc d
;lld e
}mm 	
returnoo 
newoo 
Moneyoo 
(oo 
moneyoo 
.oo 
Amountoo %
*oo& '
factoroo( .
,oo. /
moneyoo0 5
.oo5 6
Currencyoo6 >
)oo> ?
;oo? @
}pp 
publicxx 

staticxx 
Moneyxx 
operatorxx  
*xx! "
(xx" #
decimalxx# *
factorxx+ 1
,xx1 2
Moneyxx3 8
moneyxx9 >
)xx> ?
=>xx@ B
moneyxxC H
*xxI J
factorxxK Q
;xxQ R
public
ÅÅ 

static
ÅÅ 
Money
ÅÅ 
operator
ÅÅ  
/
ÅÅ! "
(
ÅÅ" #
Money
ÅÅ# (
money
ÅÅ) .
,
ÅÅ. /
decimal
ÅÅ0 7
divisor
ÅÅ8 ?
)
ÅÅ? @
{
ÇÇ 
if
ÉÉ 

(
ÉÉ 
divisor
ÉÉ 
<=
ÉÉ 
$num
ÉÉ 
)
ÉÉ 
{
ÑÑ 	
throw
ÖÖ 
new
ÖÖ 
ArgumentException
ÖÖ '
(
ÖÖ' (
$str
ÖÖ( L
,
ÖÖL M
nameof
ÖÖN T
(
ÖÖT U
divisor
ÖÖU \
)
ÖÖ\ ]
)
ÖÖ] ^
;
ÖÖ^ _
}
ÜÜ 	
return
àà 
new
àà 
Money
àà 
(
àà 
money
àà 
.
àà 
Amount
àà %
/
àà& '
divisor
àà( /
,
àà/ 0
money
àà1 6
.
àà6 7
Currency
àà7 ?
)
àà? @
;
àà@ A
}
ââ 
public
èè 

static
èè 
bool
èè 
operator
èè 
<
èè  !
(
èè! "
Money
èè" '
left
èè( ,
,
èè, -
Money
èè. 3
right
èè4 9
)
èè9 :
{
êê 
if
ëë 

(
ëë 
left
ëë 
.
ëë 
Currency
ëë 
!=
ëë 
right
ëë "
.
ëë" #
Currency
ëë# +
)
ëë+ ,
{
íí 	
throw
ìì 
new
ìì '
InvalidOperationException
ìì /
(
ìì/ 0
$"
ìì0 2
$str
ìì2 b
{
ììb c
left
ììc g
.
ììg h
Currency
ììh p
}
ììp q
$str
ììq v
{
ììv w
right
ììw |
.
ìì| }
Currencyìì} Ö
}ììÖ Ü
$strììÜ á
"ììá à
)ììà â
;ììâ ä
}
îî 	
return
ññ 
left
ññ 
.
ññ 
Amount
ññ 
<
ññ 
right
ññ "
.
ññ" #
Amount
ññ# )
;
ññ) *
}
óó 
public
úú 

static
úú 
bool
úú 
operator
úú 
>
úú  !
(
úú! "
Money
úú" '
left
úú( ,
,
úú, -
Money
úú. 3
right
úú4 9
)
úú9 :
{
ùù 
if
ûû 

(
ûû 
left
ûû 
.
ûû 
Currency
ûû 
!=
ûû 
right
ûû "
.
ûû" #
Currency
ûû# +
)
ûû+ ,
{
üü 	
throw
†† 
new
†† '
InvalidOperationException
†† /
(
††/ 0
$"
††0 2
$str
††2 b
{
††b c
left
††c g
.
††g h
Currency
††h p
}
††p q
$str
††q v
{
††v w
right
††w |
.
††| }
Currency††} Ö
}††Ö Ü
$str††Ü á
"††á à
)††à â
;††â ä
}
°° 	
return
££ 
left
££ 
.
££ 
Amount
££ 
>
££ 
right
££ "
.
££" #
Amount
££# )
;
££) *
}
§§ 
public
©© 

static
©© 
bool
©© 
operator
©© 
<=
©©  "
(
©©" #
Money
©©# (
left
©©) -
,
©©- .
Money
©©/ 4
right
©©5 :
)
©©: ;
=>
©©< >
left
©©? C
<
©©D E
right
©©F K
||
©©L N
left
©©O S
==
©©T V
right
©©W \
;
©©\ ]
public
ÆÆ 

static
ÆÆ 
bool
ÆÆ 
operator
ÆÆ 
>=
ÆÆ  "
(
ÆÆ" #
Money
ÆÆ# (
left
ÆÆ) -
,
ÆÆ- .
Money
ÆÆ/ 4
right
ÆÆ5 :
)
ÆÆ: ;
=>
ÆÆ< >
left
ÆÆ? C
>
ÆÆD E
right
ÆÆF K
||
ÆÆL N
left
ÆÆO S
==
ÆÆT V
right
ÆÆW \
;
ÆÆ\ ]
public
≥≥ 

override
≥≥ 
string
≥≥ 
ToString
≥≥ #
(
≥≥# $
)
≥≥$ %
=>
≥≥& (
$"
≥≥) +
{
≥≥+ ,
Currency
≥≥, 4
}
≥≥4 5
$str
≥≥5 6
{
≥≥6 7
Amount
≥≥7 =
:
≥≥= >
$str
≥≥> @
}
≥≥@ A
"
≥≥A B
;
≥≥B C
public
µµ 

int
µµ 
	CompareTo
µµ 
(
µµ 
Money
µµ 
?
µµ 
other
µµ  %
)
µµ% &
{
∂∂ 
if
∑∑ 

(
∑∑ 
other
∑∑ 
is
∑∑ 
null
∑∑ 
)
∑∑ 
return
∑∑ !
$num
∑∑" #
;
∑∑# $
if
∏∏ 

(
∏∏ 
Currency
∏∏ 
!=
∏∏ 
other
∏∏ 
.
∏∏ 
Currency
∏∏ &
)
∏∏& '
throw
ππ 
new
ππ 
ArgumentException
ππ '
(
ππ' (
$"
ππ( *
$str
ππ* Z
{
ππZ [
Currency
ππ[ c
}
ππc d
$str
ππd i
{
ππi j
other
ππj o
.
ππo p
Currency
ππp x
}
ππx y
$str
ππy z
"
ππz {
)
ππ{ |
;
ππ| }
return
∫∫ 
Amount
∫∫ 
.
∫∫ 
	CompareTo
∫∫ 
(
∫∫  
other
∫∫  %
.
∫∫% &
Amount
∫∫& ,
)
∫∫, -
;
∫∫- .
}
ªª 
public
ΩΩ 

int
ΩΩ 
	CompareTo
ΩΩ 
(
ΩΩ 
object
ΩΩ 
?
ΩΩ  
obj
ΩΩ! $
)
ΩΩ$ %
{
ææ 
if
øø 

(
øø 
obj
øø 
is
øø 
null
øø 
)
øø 
return
øø 
$num
øø  !
;
øø! "
if
¿¿ 

(
¿¿ 
obj
¿¿ 
is
¿¿ 
Money
¿¿ 
other
¿¿ 
)
¿¿ 
return
¿¿  &
	CompareTo
¿¿' 0
(
¿¿0 1
other
¿¿1 6
)
¿¿6 7
;
¿¿7 8
throw
¡¡ 
new
¡¡ 
ArgumentException
¡¡ #
(
¡¡# $
$"
¡¡$ &
$str
¡¡& =
{
¡¡= >
nameof
¡¡> D
(
¡¡D E
Money
¡¡E J
)
¡¡J K
}
¡¡K L
"
¡¡L M
)
¡¡M N
;
¡¡N O
}
¬¬ 
}√√ „&
}C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\ValueObjects\EquipmentTransferResult.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
ValueObjects &
;& '
public		 
record		 #
EquipmentTransferResult		 %
(		% &
bool

 
IsSuccessful

	 
,

 
string 

?
 
ErrorMessage 
= 
null 
,  !
EquipmentTransferData 
? 
Data 
=  !
null" &
) 
{ 
public 

static #
EquipmentTransferResult )
Success* 1
(1 2!
EquipmentTransferData2 G
?G H
dataI M
=N O
nullP T
)T U
=>V X
new 
( 
true 
, 
null 
, 
data 
) 
; 
public 

static #
EquipmentTransferResult )
NotFound* 2
(2 3
string3 9

entityType: D
=E F
$strG R
)R S
=>T V
new 
( 
false 
, 
$" 
{ 

entityType  
}  !
$str! +
"+ ,
), -
;- .
public 

static #
EquipmentTransferResult )
InvalidOperation* :
(: ;
string; A
messageB I
)I J
=>K M
new 
( 
false 
, 
message 
) 
; 
public 

static #
EquipmentTransferResult )
ValidationError* 9
(9 :
string: @
messageA H
)H I
=>J L
new 
( 
false 
, 
message 
) 
; 
} 
public 
record !
EquipmentTransferData #
(# $
Guid   
FromTableId  	 
,   
Guid!! 
	ToTableId!!	 
,!! 
IReadOnlyList"" 
<"" 
Guid"" 
>"" #
TransferredEquipmentIds"" /
,""/ 0
IReadOnlyList## 
<## 
Guid## 
>## 
FailedEquipmentIds## *
,##* +
DateTime$$ 
TransferTimestamp$$ 
)%% 
;%% 
public** 
record** ,
 ServerAssignmentManagementResult** .
(**. /
bool++ 
IsSuccessful++	 
,++ 
string,, 

?,,
 
ErrorMessage,, 
=,, 
null,, 
,,,  *
ServerAssignmentManagementData-- "
?--" #
Data--$ (
=--) *
null--+ /
).. 
{// 
public00 

static00 ,
 ServerAssignmentManagementResult00 2
Success003 :
(00: ;*
ServerAssignmentManagementData00; Y
?00Y Z
data00[ _
=00` a
null00b f
)00f g
=>00h j
new11 
(11 
true11 
,11 
null11 
,11 
data11 
)11 
;11 
public33 

static33 ,
 ServerAssignmentManagementResult33 2
InvalidOperation333 C
(33C D
string33D J
message33K R
)33R S
=>33T V
new44 
(44 
false44 
,44 
message44 
)44 
;44 
public66 

static66 ,
 ServerAssignmentManagementResult66 2
ValidationError663 B
(66B C
string66C I
message66J Q
)66Q R
=>66S U
new77 
(77 
false77 
,77 
message77 
)77 
;77 
}88 
public== 
record== *
ServerAssignmentManagementData== ,
(==, -
TableOperationType>> 
OperationType>> $
,>>$ %
IReadOnlyList?? 
<?? 
Guid?? 
>?? 
TableIds??  
,??  !
IReadOnlyDictionary@@ 
<@@ 
Guid@@ 
,@@ 
IReadOnlyList@@ +
<@@+ ,
Guid@@, 0
>@@0 1
>@@1 2$
ServerAssignmentsByTable@@3 K
,@@K L
DateTimeAA 
OperationTimestampAA 
)BB 
;BB 
publicGG 
enumGG $
ServerAssignmentStrategyGG $
{HH 
KeepExistingLL 
,LL 

MergeEqualQQ 
,QQ 

UsePrimaryVV 
,VV 
CustomAllocation[[ 
,[[ 
ClearAll`` 
}aa Ω¿
xC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\TableOperationsService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Services "
;" #
public 
class "
TableOperationsService #
:$ %#
ITableOperationsService& =
{ 
private 
readonly 

Dictionary 
<  
Guid  $
,$ %
TableSession& 2
>2 3
	_sessions4 =
=> ?
new@ C
(C D
)D E
;E F
private 
readonly 

Dictionary 
<  
Guid  $
,$ %
Table& +
>+ ,
_tables- 4
=5 6
new7 :
(: ;
); <
;< =
private 
readonly 

Dictionary 
<  
Guid  $
,$ %
TableMergeStatus& 6
>6 7
_mergeStatuses8 F
=G H
newI L
(L M
)M N
;N O
private 
readonly 
List 
< $
TableOperationAuditEntry 2
>2 3
_auditEntries4 A
=B C
newD G
(G H
)H I
;I J
public 

async 
Task 
<  
TableOperationResult *
>* +
MergeTablesAsync, <
(< =
Guid 
primaryTableId 
, 
IEnumerable 
< 
Guid 
> 
secondaryTableIds +
,+ ,
string 
reason 
, 
Guid 
staffId 
) 
{ 
try   
{!! 	
if## 
(## 
primaryTableId## 
==## !
Guid##" &
.##& '
Empty##' ,
)##, -
{$$ 
return%%  
TableOperationResult%% +
.%%+ ,
ValidationError%%, ;
(%%; <
$str%%< ^
)%%^ _
;%%_ `
}&& 
if(( 
((( 
staffId(( 
==(( 
Guid(( 
.((  
Empty((  %
)((% &
{)) 
return**  
TableOperationResult** +
.**+ ,
ValidationError**, ;
(**; <
$str**< V
)**V W
;**W X
}++ 
if-- 
(-- 
string-- 
.-- 
IsNullOrWhiteSpace-- )
(--) *
reason--* 0
)--0 1
)--1 2
{.. 
return//  
TableOperationResult// +
.//+ ,
ValidationError//, ;
(//; <
$str//< ^
)//^ _
;//_ `
}00 
var22 
secondaryIds22 
=22 
secondaryTableIds22 0
?220 1
.221 2
ToList222 8
(228 9
)229 :
??22; =
new22> A
List22B F
<22F G
Guid22G K
>22K L
(22L M
)22M N
;22N O
if33 
(33 
!33 
secondaryIds33 
.33 
Any33 !
(33! "
)33" #
)33# $
{44 
return55  
TableOperationResult55 +
.55+ ,
ValidationError55, ;
(55; <
$str55< p
)55p q
;55q r
}66 
if88 
(88 
secondaryIds88 
.88 
Contains88 %
(88% &
primaryTableId88& 4
)884 5
)885 6
{99 
return::  
TableOperationResult:: +
.::+ ,
ValidationError::, ;
(::; <
$str::< n
)::n o
;::o p
};; 
if== 
(== 
secondaryIds== 
.== 
Any==  
(==  !
id==! #
=>==$ &
id==' )
====* ,
Guid==- 1
.==1 2
Empty==2 7
)==7 8
)==8 9
{>> 
return??  
TableOperationResult?? +
.??+ ,
ValidationError??, ;
(??; <
$str??< c
)??c d
;??d e
}@@ 
varCC 
validationResultCC  
=CC! "
awaitCC# (#
ValidateTableMergeAsyncCC) @
(CC@ A
primaryTableIdCCA O
,CCO P
secondaryIdsCCQ ]
)CC] ^
;CC^ _
ifDD 
(DD 
!DD 
validationResultDD !
.DD! "
IsValidDD" )
)DD) *
{EE 
returnFF  
TableOperationResultFF +
.FF+ ,
ValidationErrorFF, ;
(FF; <
stringFF< B
.FFB C
JoinFFC G
(FFG H
$strFFH L
,FFL M
validationResultFFN ^
.FF^ _
ValidationErrorsFF_ o
)FFo p
)FFp q
;FFq r
}GG 
varJJ 
allTableIdsJJ 
=JJ 
newJJ !
[JJ! "
]JJ" #
{JJ$ %
primaryTableIdJJ& 4
}JJ5 6
.JJ6 7
ConcatJJ7 =
(JJ= >
secondaryIdsJJ> J
)JJJ K
.JJK L
ToListJJL R
(JJR S
)JJS T
;JJT U
varKK 
totalChargesBeforeKK "
=KK# $
MoneyKK% *
.KK* +
ZeroKK+ /
(KK/ 0
)KK0 1
;KK1 2
varLL 
totalChargesAfterLL !
=LL" #
MoneyLL$ )
.LL) *
ZeroLL* .
(LL. /
)LL/ 0
;LL0 1
foreachOO 
(OO 
varOO 
tableIdOO  
inOO! #
allTableIdsOO$ /
)OO/ 0
{PP 
ifQQ 
(QQ 
	_sessionsQQ 
.QQ 
ContainsKeyQQ )
(QQ) *
tableIdQQ* 1
)QQ1 2
)QQ2 3
{RR 
totalChargesBeforeSS &
+=SS' )
	_sessionsSS* 3
[SS3 4
tableIdSS4 ;
]SS; <
.SS< =
TotalChargeSS= H
;SSH I
}TT 
}UU 
varXX 
mergedSessionIdXX 
=XX  !
GuidXX" &
.XX& '
NewGuidXX' .
(XX. /
)XX/ 0
;XX0 1
totalChargesAfterYY 
=YY 
totalChargesBeforeYY  2
;YY2 3
foreach\\ 
(\\ 
var\\ 
tableId\\  
in\\! #
allTableIds\\$ /
)\\/ 0
{]] 
_mergeStatuses^^ 
[^^ 
tableId^^ &
]^^& '
=^^( )
TableMergeStatus^^* :
.^^: ;
Merged^^; A
(^^A B
tableId__ 
,__ 
mergedSessionId`` #
,``# $
primaryTableIdaa "
,aa" #
allTableIdsbb 
,bb  
DateTimecc 
.cc 
UtcNowcc #
,cc# $
reasondd 
,dd 
staffIdee 
)ff 
;ff 
}gg 
varjj 

auditEntryjj 
=jj $
TableOperationAuditEntryjj 5
.jj5 6
Createjj6 <
(jj< =
primaryTableIdkk 
,kk 
TableOperationTypell "
.ll" #
Mergell# (
,ll( )
staffIdmm 
,mm 
$"nn 
$strnn 
{nn 
staffIdnn  
}nn  !
"nn! "
,nn" #
reasonoo 
,oo #
TableOperationAuditDatapp '
.pp' (
MultipleTablespp( 6
(pp6 7
allTableIdspp7 B
,ppB C
newppD G
ListppH L
<ppL M
GuidppM Q
>ppQ R
(ppR S
)ppS T
,ppT U
totalChargesBeforeppV h
,pph i
$numppj k
)ppk l
,ppl m#
TableOperationAuditDataqq '
.qq' (
SingleTableqq( 3
(qq3 4
primaryTableIdqq4 B
,qqB C
mergedSessionIdqqD S
,qqS T
totalChargesAfterqqU f
,qqf g
$numqqh i
)qqi j
)rr 
;rr 
_auditEntriesss 
.ss 
Addss 
(ss 

auditEntryss (
)ss( )
;ss) *
varuu 
operationDatauu 
=uu 
newuu  #
TableOperationDatauu$ 6
(uu6 7

auditEntryvv 
.vv 
Idvv 
,vv 
TableOperationTypeww "
.ww" #
Mergeww# (
,ww( )
allTableIdsxx 
,xx 
mergedSessionIdyy 
,yy  
nullzz 
,zz 
totalChargesBefore{{ "
,{{" #
totalChargesAfter|| !
,||! "
DateTime}} 
.}} 
UtcNow}} 
,}}  
staffId~~ 
,~~ 
reason 
)
ÄÄ 
;
ÄÄ 
return
ÇÇ "
TableOperationResult
ÇÇ '
.
ÇÇ' (
Success
ÇÇ( /
(
ÇÇ/ 0
operationData
ÇÇ0 =
)
ÇÇ= >
;
ÇÇ> ?
}
ÉÉ 	
catch
ÑÑ 
(
ÑÑ 
	Exception
ÑÑ 
ex
ÑÑ 
)
ÑÑ 
{
ÖÖ 	
return
ÜÜ "
TableOperationResult
ÜÜ '
.
ÜÜ' (
InvalidOperation
ÜÜ( 8
(
ÜÜ8 9
$"
ÜÜ9 ;
$str
ÜÜ; S
{
ÜÜS T
ex
ÜÜT V
.
ÜÜV W
Message
ÜÜW ^
}
ÜÜ^ _
"
ÜÜ_ `
)
ÜÜ` a
;
ÜÜa b
}
áá 	
}
àà 
public
çç 

async
çç 
Task
çç 
<
çç "
TableOperationResult
çç *
>
çç* +
SplitTablesAsync
çç, <
(
çç< =
Guid
éé 
mergedSessionId
éé 
,
éé "
TableSplitAllocation
èè 
splitAllocation
èè ,
,
èè, -
string
êê 
reason
êê 
,
êê 
Guid
ëë 
staffId
ëë 
)
ëë 
{
íí 
try
ìì 
{
îî 	
if
ññ 
(
ññ 
mergedSessionId
ññ 
==
ññ  "
Guid
ññ# '
.
ññ' (
Empty
ññ( -
)
ññ- .
{
óó 
return
òò "
TableOperationResult
òò +
.
òò+ ,
ValidationError
òò, ;
(
òò; <
$str
òò< _
)
òò_ `
;
òò` a
}
ôô 
if
õõ 
(
õõ 
staffId
õõ 
==
õõ 
Guid
õõ 
.
õõ  
Empty
õõ  %
)
õõ% &
{
úú 
return
ùù "
TableOperationResult
ùù +
.
ùù+ ,
ValidationError
ùù, ;
(
ùù; <
$str
ùù< V
)
ùùV W
;
ùùW X
}
ûû 
if
†† 
(
†† 
string
†† 
.
††  
IsNullOrWhiteSpace
†† )
(
††) *
reason
††* 0
)
††0 1
)
††1 2
{
°° 
return
¢¢ "
TableOperationResult
¢¢ +
.
¢¢+ ,
ValidationError
¢¢, ;
(
¢¢; <
$str
¢¢< ^
)
¢¢^ _
;
¢¢_ `
}
££ 
if
•• 
(
•• 
splitAllocation
•• 
==
••  "
null
••# '
)
••' (
{
¶¶ 
return
ßß "
TableOperationResult
ßß +
.
ßß+ ,
ValidationError
ßß, ;
(
ßß; <
$str
ßß< ]
)
ßß] ^
;
ßß^ _
}
®® 
if
™™ 
(
™™ 
!
™™ 
splitAllocation
™™  
.
™™  !
IsValid
™™! (
(
™™( )
)
™™) *
)
™™* +
{
´´ 
return
¨¨ "
TableOperationResult
¨¨ +
.
¨¨+ ,
ValidationError
¨¨, ;
(
¨¨; <
$str
¨¨< Y
)
¨¨Y Z
;
¨¨Z [
}
≠≠ 
var
∞∞ 
validationResult
∞∞  
=
∞∞! "
await
∞∞# (%
ValidateTableSplitAsync
∞∞) @
(
∞∞@ A
mergedSessionId
∞∞A P
,
∞∞P Q
splitAllocation
∞∞R a
)
∞∞a b
;
∞∞b c
if
±± 
(
±± 
!
±± 
validationResult
±± !
.
±±! "
IsValid
±±" )
)
±±) *
{
≤≤ 
return
≥≥ "
TableOperationResult
≥≥ +
.
≥≥+ ,
ValidationError
≥≥, ;
(
≥≥; <
string
≥≥< B
.
≥≥B C
Join
≥≥C G
(
≥≥G H
$str
≥≥H L
,
≥≥L M
validationResult
≥≥N ^
.
≥≥^ _
ValidationErrors
≥≥_ o
)
≥≥o p
)
≥≥p q
;
≥≥q r
}
¥¥ 
var
∑∑  
totalChargesBefore
∑∑ "
=
∑∑# $
new
∑∑% (
Money
∑∑) .
(
∑∑. /
$num
∑∑/ 4
)
∑∑4 5
;
∑∑5 6
var
∏∏ 
totalChargesAfter
∏∏ !
=
∏∏" #
Money
∏∏$ )
.
∏∏) *
Zero
∏∏* .
(
∏∏. /
)
∏∏/ 0
;
∏∏0 1
var
∫∫ !
resultingSessionIds
∫∫ #
=
∫∫$ %
new
∫∫& )
List
∫∫* .
<
∫∫. /
Guid
∫∫/ 3
>
∫∫3 4
(
∫∫4 5
)
∫∫5 6
;
∫∫6 7
foreach
ªª 
(
ªª 
var
ªª 

allocation
ªª #
in
ªª$ &
splitAllocation
ªª' 6
.
ªª6 7
TableAllocations
ªª7 G
.
ªªG H
Values
ªªH N
)
ªªN O
{
ºº 
var
ΩΩ 
	sessionId
ΩΩ 
=
ΩΩ 
Guid
ΩΩ  $
.
ΩΩ$ %
NewGuid
ΩΩ% ,
(
ΩΩ, -
)
ΩΩ- .
;
ΩΩ. /!
resultingSessionIds
ææ #
.
ææ# $
Add
ææ$ '
(
ææ' (
	sessionId
ææ( 1
)
ææ1 2
;
ææ2 3
var
¿¿ 
allocatedCharge
¿¿ #
=
¿¿$ %
new
¿¿& )
Money
¿¿* /
(
¿¿/ 0 
totalChargesBefore
¿¿0 B
.
¿¿B C
Amount
¿¿C I
*
¿¿J K
(
¿¿L M

allocation
¿¿M W
.
¿¿W X
ChargePercentage
¿¿X h
/
¿¿i j
$num
¿¿k o
)
¿¿o p
)
¿¿p q
;
¿¿q r
totalChargesAfter
¡¡ !
+=
¡¡" $
allocatedCharge
¡¡% 4
;
¡¡4 5
}
¬¬ 
var
≈≈ 
tableIds
≈≈ 
=
≈≈ 
splitAllocation
≈≈ *
.
≈≈* +
TableAllocations
≈≈+ ;
.
≈≈; <
Keys
≈≈< @
.
≈≈@ A
ToList
≈≈A G
(
≈≈G H
)
≈≈H I
;
≈≈I J
foreach
∆∆ 
(
∆∆ 
var
∆∆ 
tableId
∆∆  
in
∆∆! #
tableIds
∆∆$ ,
)
∆∆, -
{
«« 
_mergeStatuses
»» 
[
»» 
tableId
»» &
]
»»& '
=
»»( )
TableMergeStatus
»»* :
.
»»: ;
	NotMerged
»»; D
(
»»D E
tableId
»»E L
)
»»L M
;
»»M N
}
…… 
var
ÃÃ 

auditEntry
ÃÃ 
=
ÃÃ &
TableOperationAuditEntry
ÃÃ 5
.
ÃÃ5 6
Create
ÃÃ6 <
(
ÃÃ< =
tableIds
ÕÕ 
.
ÕÕ 
First
ÕÕ 
(
ÕÕ 
)
ÕÕ  
,
ÕÕ  ! 
TableOperationType
ŒŒ "
.
ŒŒ" #
Split
ŒŒ# (
,
ŒŒ( )
staffId
œœ 
,
œœ 
$"
–– 
$str
–– 
{
–– 
staffId
––  
}
––  !
"
––! "
,
––" #
reason
—— 
,
—— %
TableOperationAuditData
““ '
.
““' (
SingleTable
““( 3
(
““3 4
tableIds
““4 <
.
““< =
First
““= B
(
““B C
)
““C D
,
““D E
mergedSessionId
““F U
,
““U V 
totalChargesBefore
““W i
,
““i j
$num
““k l
)
““l m
,
““m n%
TableOperationAuditData
”” '
.
””' (
MultipleTables
””( 6
(
””6 7
tableIds
””7 ?
,
””? @!
resultingSessionIds
””A T
,
””T U
totalChargesAfter
””V g
,
””g h
$num
””i j
)
””j k
)
‘‘ 
;
‘‘ 
_auditEntries
’’ 
.
’’ 
Add
’’ 
(
’’ 

auditEntry
’’ (
)
’’( )
;
’’) *
var
◊◊ 
operationData
◊◊ 
=
◊◊ 
new
◊◊  # 
TableOperationData
◊◊$ 6
(
◊◊6 7

auditEntry
ÿÿ 
.
ÿÿ 
Id
ÿÿ 
,
ÿÿ  
TableOperationType
ŸŸ "
.
ŸŸ" #
Split
ŸŸ# (
,
ŸŸ( )
tableIds
⁄⁄ 
,
⁄⁄ 
null
€€ 
,
€€ !
resultingSessionIds
‹‹ #
,
‹‹# $ 
totalChargesBefore
›› "
,
››" #
totalChargesAfter
ﬁﬁ !
,
ﬁﬁ! "
DateTime
ﬂﬂ 
.
ﬂﬂ 
UtcNow
ﬂﬂ 
,
ﬂﬂ  
staffId
‡‡ 
,
‡‡ 
reason
·· 
)
‚‚ 
;
‚‚ 
return
‰‰ "
TableOperationResult
‰‰ '
.
‰‰' (
Success
‰‰( /
(
‰‰/ 0
operationData
‰‰0 =
)
‰‰= >
;
‰‰> ?
}
ÂÂ 	
catch
ÊÊ 
(
ÊÊ 
	Exception
ÊÊ 
ex
ÊÊ 
)
ÊÊ 
{
ÁÁ 	
return
ËË "
TableOperationResult
ËË '
.
ËË' (
InvalidOperation
ËË( 8
(
ËË8 9
$"
ËË9 ;
$str
ËË; S
{
ËËS T
ex
ËËT V
.
ËËV W
Message
ËËW ^
}
ËË^ _
"
ËË_ `
)
ËË` a
;
ËËa b
}
ÈÈ 	
}
ÍÍ 
public
ÔÔ 

async
ÔÔ 
Task
ÔÔ 
<
ÔÔ 
TableMergeStatus
ÔÔ &
>
ÔÔ& '&
GetTableMergeStatusAsync
ÔÔ( @
(
ÔÔ@ A
Guid
ÔÔA E
tableId
ÔÔF M
)
ÔÔM N
{
 
await
ÒÒ 
Task
ÒÒ 
.
ÒÒ 
CompletedTask
ÒÒ  
;
ÒÒ  !
if
ÛÛ 

(
ÛÛ 
tableId
ÛÛ 
==
ÛÛ 
Guid
ÛÛ 
.
ÛÛ 
Empty
ÛÛ !
)
ÛÛ! "
{
ÙÙ 	
throw
ıı 
new
ıı 
ArgumentException
ıı '
(
ıı' (
$str
ıı( C
,
ııC D
nameof
ııE K
(
ııK L
tableId
ııL S
)
ııS T
)
ııT U
;
ııU V
}
ˆˆ 	
return
¯¯ 
_mergeStatuses
¯¯ 
.
¯¯ 
TryGetValue
¯¯ )
(
¯¯) *
tableId
¯¯* 1
,
¯¯1 2
out
¯¯3 6
var
¯¯7 :
status
¯¯; A
)
¯¯A B
?
˘˘ 
status
˘˘ 
:
˙˙ 
TableMergeStatus
˙˙ 
.
˙˙ 
	NotMerged
˙˙ (
(
˙˙( )
tableId
˙˙) 0
)
˙˙0 1
;
˙˙1 2
}
˚˚ 
public
ÄÄ 

async
ÄÄ 
Task
ÄÄ 
<
ÄÄ (
TableMergeValidationResult
ÄÄ 0
>
ÄÄ0 1%
ValidateTableMergeAsync
ÄÄ2 I
(
ÄÄI J
Guid
ÅÅ 
primaryTableId
ÅÅ 
,
ÅÅ 
IEnumerable
ÇÇ 
<
ÇÇ 
Guid
ÇÇ 
>
ÇÇ 
secondaryTableIds
ÇÇ +
)
ÇÇ+ ,
{
ÉÉ 
await
ÑÑ 
Task
ÑÑ 
.
ÑÑ 
CompletedTask
ÑÑ  
;
ÑÑ  !
var
ÜÜ 
errors
ÜÜ 
=
ÜÜ 
new
ÜÜ 
List
ÜÜ 
<
ÜÜ 
string
ÜÜ $
>
ÜÜ$ %
(
ÜÜ% &
)
ÜÜ& '
;
ÜÜ' (
var
áá 
warnings
áá 
=
áá 
new
áá 
List
áá 
<
áá  
string
áá  &
>
áá& '
(
áá' (
)
áá( )
;
áá) *
if
ää 

(
ää 
primaryTableId
ää 
==
ää 
Guid
ää "
.
ää" #
Empty
ää# (
)
ää( )
{
ãã 	
errors
åå 
.
åå 
Add
åå 
(
åå 
$str
åå 9
)
åå9 :
;
åå: ;
}
çç 	
var
èè 
secondaryIds
èè 
=
èè 
secondaryTableIds
èè ,
?
èè, -
.
èè- .
ToList
èè. 4
(
èè4 5
)
èè5 6
??
èè7 9
new
èè: =
List
èè> B
<
èèB C
Guid
èèC G
>
èèG H
(
èèH I
)
èèI J
;
èèJ K
if
êê 

(
êê 
!
êê 
secondaryIds
êê 
.
êê 
Any
êê 
(
êê 
)
êê 
)
êê  
{
ëë 	
errors
íí 
.
íí 
Add
íí 
(
íí 
$str
íí A
)
ííA B
;
ííB C
}
ìì 	
if
ïï 

(
ïï 
secondaryIds
ïï 
.
ïï 
Contains
ïï !
(
ïï! "
primaryTableId
ïï" 0
)
ïï0 1
)
ïï1 2
{
ññ 	
errors
óó 
.
óó 
Add
óó 
(
óó 
$str
óó I
)
óóI J
;
óóJ K
}
òò 	
if
öö 

(
öö 
secondaryIds
öö 
.
öö 
Any
öö 
(
öö 
id
öö 
=>
öö  "
id
öö# %
==
öö& (
Guid
öö) -
.
öö- .
Empty
öö. 3
)
öö3 4
)
öö4 5
{
õõ 	
errors
úú 
.
úú 
Add
úú 
(
úú 
$str
úú >
)
úú> ?
;
úú? @
}
ùù 	
if
†† 

(
†† 
secondaryIds
†† 
.
†† 
Count
†† 
!=
†† !
secondaryIds
††" .
.
††. /
Distinct
††/ 7
(
††7 8
)
††8 9
.
††9 :
Count
††: ?
(
††? @
)
††@ A
)
††A B
{
°° 	
errors
¢¢ 
.
¢¢ 
Add
¢¢ 
(
¢¢ 
$str
¢¢ F
)
¢¢F G
;
¢¢G H
}
££ 	
var
¶¶ 
allTableIds
¶¶ 
=
¶¶ 
new
¶¶ 
[
¶¶ 
]
¶¶ 
{
¶¶  !
primaryTableId
¶¶" 0
}
¶¶1 2
.
¶¶2 3
Concat
¶¶3 9
(
¶¶9 :
secondaryIds
¶¶: F
)
¶¶F G
;
¶¶G H
foreach
ßß 
(
ßß 
var
ßß 
tableId
ßß 
in
ßß 
allTableIds
ßß  +
)
ßß+ ,
{
®® 	
if
©© 
(
©© 
_mergeStatuses
©© 
.
©© 
TryGetValue
©© *
(
©©* +
tableId
©©+ 2
,
©©2 3
out
©©4 7
var
©©8 ;
status
©©< B
)
©©B C
&&
©©D F
status
©©G M
.
©©M N
IsMerged
©©N V
)
©©V W
{
™™ 
errors
´´ 
.
´´ 
Add
´´ 
(
´´ 
$"
´´ 
$str
´´ #
{
´´# $
tableId
´´$ +
}
´´+ ,
$str
´´, G
"
´´G H
)
´´H I
;
´´I J
}
¨¨ 
}
≠≠ 	
if
∞∞ 

(
∞∞ 
secondaryIds
∞∞ 
.
∞∞ 
Count
∞∞ 
>
∞∞  
$num
∞∞! "
)
∞∞" #
{
±± 	
warnings
≤≤ 
.
≤≤ 
Add
≤≤ 
(
≤≤ 
$str
≤≤ P
)
≤≤P Q
;
≤≤Q R
}
≥≥ 	
return
µµ 
errors
µµ 
.
µµ 
Any
µµ 
(
µµ 
)
µµ 
?
∂∂ (
TableMergeValidationResult
∂∂ (
.
∂∂( )
Invalid
∂∂) 0
(
∂∂0 1
errors
∂∂1 7
,
∂∂7 8
warnings
∂∂9 A
)
∂∂A B
:
∑∑ (
TableMergeValidationResult
∑∑ (
.
∑∑( )
Valid
∑∑) .
(
∑∑. /
warnings
∑∑/ 7
)
∑∑7 8
;
∑∑8 9
}
∏∏ 
public
ΩΩ 

async
ΩΩ 
Task
ΩΩ 
<
ΩΩ (
TableSplitValidationResult
ΩΩ 0
>
ΩΩ0 1%
ValidateTableSplitAsync
ΩΩ2 I
(
ΩΩI J
Guid
ææ 
mergedSessionId
ææ 
,
ææ "
TableSplitAllocation
øø 
splitAllocation
øø ,
)
øø, -
{
¿¿ 
await
¡¡ 
Task
¡¡ 
.
¡¡ 
CompletedTask
¡¡  
;
¡¡  !
var
√√ 
errors
√√ 
=
√√ 
new
√√ 
List
√√ 
<
√√ 
string
√√ $
>
√√$ %
(
√√% &
)
√√& '
;
√√' (
var
ƒƒ 
warnings
ƒƒ 
=
ƒƒ 
new
ƒƒ 
List
ƒƒ 
<
ƒƒ  
string
ƒƒ  &
>
ƒƒ& '
(
ƒƒ' (
)
ƒƒ( )
;
ƒƒ) *
if
«« 

(
«« 
mergedSessionId
«« 
==
«« 
Guid
«« #
.
««# $
Empty
««$ )
)
««) *
{
»» 	
errors
…… 
.
…… 
Add
…… 
(
…… 
$str
…… :
)
……: ;
;
……; <
}
   	
if
ÃÃ 

(
ÃÃ 
splitAllocation
ÃÃ 
==
ÃÃ 
null
ÃÃ #
)
ÃÃ# $
{
ÕÕ 	
errors
ŒŒ 
.
ŒŒ 
Add
ŒŒ 
(
ŒŒ 
$str
ŒŒ 8
)
ŒŒ8 9
;
ŒŒ9 :
return
œœ (
TableSplitValidationResult
œœ -
.
œœ- .
Invalid
œœ. 5
(
œœ5 6
errors
œœ6 <
)
œœ< =
;
œœ= >
}
–– 	
if
““ 

(
““ 
!
““ 
splitAllocation
““ 
.
““ 
IsValid
““ $
(
““$ %
)
““% &
)
““& '
{
”” 	
errors
‘‘ 
.
‘‘ 
Add
‘‘ 
(
‘‘ 
$str
‘‘ F
)
‘‘F G
;
‘‘G H
}
’’ 	
foreach
ÿÿ 
(
ÿÿ 
var
ÿÿ 

allocation
ÿÿ 
in
ÿÿ  "
splitAllocation
ÿÿ# 2
.
ÿÿ2 3
TableAllocations
ÿÿ3 C
.
ÿÿC D
Values
ÿÿD J
)
ÿÿJ K
{
ŸŸ 	
if
⁄⁄ 
(
⁄⁄ 

allocation
⁄⁄ 
.
⁄⁄ 

GuestCount
⁄⁄ %
<=
⁄⁄& (
$num
⁄⁄) *
)
⁄⁄* +
{
€€ 
errors
‹‹ 
.
‹‹ 
Add
‹‹ 
(
‹‹ 
$"
‹‹ 
$str
‹‹ 3
{
‹‹3 4

allocation
‹‹4 >
.
‹‹> ?
TableId
‹‹? F
}
‹‹F G
$str
‹‹G a
"
‹‹a b
)
‹‹b c
;
‹‹c d
}
›› 
if
ﬂﬂ 
(
ﬂﬂ 

allocation
ﬂﬂ 
.
ﬂﬂ 
ChargePercentage
ﬂﬂ +
<=
ﬂﬂ, .
$num
ﬂﬂ/ 0
||
ﬂﬂ1 3

allocation
ﬂﬂ4 >
.
ﬂﬂ> ?
ChargePercentage
ﬂﬂ? O
>
ﬂﬂP Q
$num
ﬂﬂR U
)
ﬂﬂU V
{
‡‡ 
errors
·· 
.
·· 
Add
·· 
(
·· 
$"
·· 
$str
·· 9
{
··9 :

allocation
··: D
.
··D E
TableId
··E L
}
··L M
$str
··M g
"
··g h
)
··h i
;
··i j
}
‚‚ 
}
„„ 	
if
ÊÊ 

(
ÊÊ 
splitAllocation
ÊÊ 
.
ÊÊ 

TableCount
ÊÊ &
<
ÊÊ' (
$num
ÊÊ) *
)
ÊÊ* +
{
ÁÁ 	
errors
ËË 
.
ËË 
Add
ËË 
(
ËË 
$str
ËË C
)
ËËC D
;
ËËD E
}
ÈÈ 	
if
ÏÏ 

(
ÏÏ 
splitAllocation
ÏÏ 
.
ÏÏ 

TableCount
ÏÏ &
>
ÏÏ' (
$num
ÏÏ) *
)
ÏÏ* +
{
ÌÌ 	
warnings
ÓÓ 
.
ÓÓ 
Add
ÓÓ 
(
ÓÓ 
$str
ÓÓ S
)
ÓÓS T
;
ÓÓT U
}
ÔÔ 	
return
ÒÒ 
errors
ÒÒ 
.
ÒÒ 
Any
ÒÒ 
(
ÒÒ 
)
ÒÒ 
?
ÚÚ (
TableSplitValidationResult
ÚÚ (
.
ÚÚ( )
Invalid
ÚÚ) 0
(
ÚÚ0 1
errors
ÚÚ1 7
,
ÚÚ7 8
warnings
ÚÚ9 A
)
ÚÚA B
:
ÛÛ (
TableSplitValidationResult
ÛÛ (
.
ÛÛ( )
Valid
ÛÛ) .
(
ÛÛ. /
warnings
ÛÛ/ 7
)
ÛÛ7 8
;
ÛÛ8 9
}
ÙÙ 
public
˘˘ 

async
˘˘ 
Task
˘˘ 
<
˘˘ 
IEnumerable
˘˘ !
<
˘˘! "&
TableOperationAuditEntry
˘˘" :
>
˘˘: ;
>
˘˘; <.
 GetTableOperationAuditTrailAsync
˘˘= ]
(
˘˘] ^
Guid
˙˙ 
tableId
˙˙ 
,
˙˙ 
DateTime
˚˚ 
fromDate
˚˚ 
,
˚˚ 
DateTime
¸¸ 
toDate
¸¸ 
)
¸¸ 
{
˝˝ 
await
˛˛ 
Task
˛˛ 
.
˛˛ 
CompletedTask
˛˛  
;
˛˛  !
if
ÄÄ 

(
ÄÄ 
tableId
ÄÄ 
==
ÄÄ 
Guid
ÄÄ 
.
ÄÄ 
Empty
ÄÄ !
)
ÄÄ! "
{
ÅÅ 	
throw
ÇÇ 
new
ÇÇ 
ArgumentException
ÇÇ '
(
ÇÇ' (
$str
ÇÇ( C
,
ÇÇC D
nameof
ÇÇE K
(
ÇÇK L
tableId
ÇÇL S
)
ÇÇS T
)
ÇÇT U
;
ÇÇU V
}
ÉÉ 	
if
ÖÖ 

(
ÖÖ 
fromDate
ÖÖ 
>
ÖÖ 
toDate
ÖÖ 
)
ÖÖ 
{
ÜÜ 	
throw
áá 
new
áá 
ArgumentException
áá '
(
áá' (
$str
áá( L
,
ááL M
nameof
ááN T
(
ááT U
fromDate
ááU ]
)
áá] ^
)
áá^ _
;
áá_ `
}
àà 	
return
ää 
_auditEntries
ää 
.
ãã 
Where
ãã 
(
ãã 
entry
ãã 
=>
ãã 
entry
ãã !
.
ãã! "
TableId
ãã" )
==
ãã* ,
tableId
ãã- 4
||
ãã5 7
entry
åå  
.
åå  !
BeforeState
åå! ,
.
åå, -
TableIds
åå- 5
.
åå5 6
Contains
åå6 >
(
åå> ?
tableId
åå? F
)
ååF G
||
ååH J
entry
çç  
.
çç  !

AfterState
çç! +
.
çç+ ,
TableIds
çç, 4
.
çç4 5
Contains
çç5 =
(
çç= >
tableId
çç> E
)
ççE F
)
ççF G
.
éé 
Where
éé 
(
éé 
entry
éé 
=>
éé 
entry
éé !
.
éé! "
	Timestamp
éé" +
>=
éé, .
fromDate
éé/ 7
&&
éé8 :
entry
éé; @
.
éé@ A
	Timestamp
ééA J
<=
ééK M
toDate
ééN T
)
ééT U
.
èè 
OrderByDescending
èè 
(
èè 
entry
èè $
=>
èè% '
entry
èè( -
.
èè- .
	Timestamp
èè. 7
)
èè7 8
.
êê 
ToList
êê 
(
êê 
)
êê 
;
êê 
}
ëë 
public
ññ 

async
ññ 
Task
ññ 
<
ññ %
EquipmentTransferResult
ññ -
>
ññ- .$
TransferEquipmentAsync
ññ/ E
(
ññE F
Guid
óó 
fromTableId
óó 
,
óó 
Guid
òò 
	toTableId
òò 
,
òò 
IEnumerable
ôô 
<
ôô 
Guid
ôô 
>
ôô 
equipmentIds
ôô &
)
ôô& '
{
öö 
await
õõ 
Task
õõ 
.
õõ 
CompletedTask
õõ  
;
õõ  !
try
ùù 
{
ûû 	
if
üü 
(
üü 
fromTableId
üü 
==
üü 
Guid
üü #
.
üü# $
Empty
üü$ )
)
üü) *
{
†† 
return
°° %
EquipmentTransferResult
°° .
.
°°. /
ValidationError
°°/ >
(
°°> ?
$str
°°? ^
)
°°^ _
;
°°_ `
}
¢¢ 
if
§§ 
(
§§ 
	toTableId
§§ 
==
§§ 
Guid
§§ !
.
§§! "
Empty
§§" '
)
§§' (
{
•• 
return
¶¶ %
EquipmentTransferResult
¶¶ .
.
¶¶. /
ValidationError
¶¶/ >
(
¶¶> ?
$str
¶¶? \
)
¶¶\ ]
;
¶¶] ^
}
ßß 
if
©© 
(
©© 
fromTableId
©© 
==
©© 
	toTableId
©© (
)
©©( )
{
™™ 
return
´´ %
EquipmentTransferResult
´´ .
.
´´. /
ValidationError
´´/ >
(
´´> ?
$str
´´? l
)
´´l m
;
´´m n
}
¨¨ 
var
ÆÆ 
equipmentList
ÆÆ 
=
ÆÆ 
equipmentIds
ÆÆ  ,
?
ÆÆ, -
.
ÆÆ- .
ToList
ÆÆ. 4
(
ÆÆ4 5
)
ÆÆ5 6
??
ÆÆ7 9
new
ÆÆ: =
List
ÆÆ> B
<
ÆÆB C
Guid
ÆÆC G
>
ÆÆG H
(
ÆÆH I
)
ÆÆI J
;
ÆÆJ K
if
ØØ 
(
ØØ 
!
ØØ 
equipmentList
ØØ 
.
ØØ 
Any
ØØ "
(
ØØ" #
)
ØØ# $
)
ØØ$ %
{
∞∞ 
return
±± %
EquipmentTransferResult
±± .
.
±±. /
ValidationError
±±/ >
(
±±> ?
$str
±±? f
)
±±f g
;
±±g h
}
≤≤ 
if
¥¥ 
(
¥¥ 
equipmentList
¥¥ 
.
¥¥ 
Any
¥¥ !
(
¥¥! "
id
¥¥" $
=>
¥¥% '
id
¥¥( *
==
¥¥+ -
Guid
¥¥. 2
.
¥¥2 3
Empty
¥¥3 8
)
¥¥8 9
)
¥¥9 :
{
µµ 
return
∂∂ %
EquipmentTransferResult
∂∂ .
.
∂∂. /
ValidationError
∂∂/ >
(
∂∂> ?
$str
∂∂? `
)
∂∂` a
;
∂∂a b
}
∑∑ 
var
∫∫ 
transferData
∫∫ 
=
∫∫ 
new
∫∫ "#
EquipmentTransferData
∫∫# 8
(
∫∫8 9
fromTableId
ªª 
,
ªª 
	toTableId
ºº 
,
ºº 
equipmentList
ΩΩ 
,
ΩΩ 
new
ææ 
List
ææ 
<
ææ 
Guid
ææ 
>
ææ 
(
ææ 
)
ææ  
,
ææ  !
DateTime
øø 
.
øø 
UtcNow
øø 
)
¿¿ 
;
¿¿ 
return
¬¬ %
EquipmentTransferResult
¬¬ *
.
¬¬* +
Success
¬¬+ 2
(
¬¬2 3
transferData
¬¬3 ?
)
¬¬? @
;
¬¬@ A
}
√√ 	
catch
ƒƒ 
(
ƒƒ 
	Exception
ƒƒ 
ex
ƒƒ 
)
ƒƒ 
{
≈≈ 	
return
∆∆ %
EquipmentTransferResult
∆∆ *
.
∆∆* +
InvalidOperation
∆∆+ ;
(
∆∆; <
$"
∆∆< >
$str
∆∆> Y
{
∆∆Y Z
ex
∆∆Z \
.
∆∆\ ]
Message
∆∆] d
}
∆∆d e
"
∆∆e f
)
∆∆f g
;
∆∆g h
}
«« 	
}
»» 
public
ÕÕ 

async
ÕÕ 
Task
ÕÕ 
<
ÕÕ .
 ServerAssignmentManagementResult
ÕÕ 6
>
ÕÕ6 79
+ManageServerAssignmentsDuringOperationAsync
ÕÕ8 c
(
ÕÕc d 
TableOperationType
ŒŒ 
operationType
ŒŒ (
,
ŒŒ( )
IEnumerable
œœ 
<
œœ 
Guid
œœ 
>
œœ 
tableIds
œœ "
,
œœ" #&
ServerAssignmentStrategy
––  &
serverAssignmentStrategy
––! 9
)
––9 :
{
—— 
await
““ 
Task
““ 
.
““ 
CompletedTask
““  
;
““  !
try
‘‘ 
{
’’ 	
var
÷÷ 
tableIdList
÷÷ 
=
÷÷ 
tableIds
÷÷ &
?
÷÷& '
.
÷÷' (
ToList
÷÷( .
(
÷÷. /
)
÷÷/ 0
??
÷÷1 3
new
÷÷4 7
List
÷÷8 <
<
÷÷< =
Guid
÷÷= A
>
÷÷A B
(
÷÷B C
)
÷÷C D
;
÷÷D E
if
ÿÿ 
(
ÿÿ 
!
ÿÿ 
tableIdList
ÿÿ 
.
ÿÿ 
Any
ÿÿ  
(
ÿÿ  !
)
ÿÿ! "
)
ÿÿ" #
{
ŸŸ 
return
⁄⁄ .
 ServerAssignmentManagementResult
⁄⁄ 7
.
⁄⁄7 8
ValidationError
⁄⁄8 G
(
⁄⁄G H
$str
⁄⁄H k
)
⁄⁄k l
;
⁄⁄l m
}
€€ 
if
›› 
(
›› 
tableIdList
›› 
.
›› 
Any
›› 
(
››  
id
››  "
=>
››# %
id
››& (
==
››) +
Guid
››, 0
.
››0 1
Empty
››1 6
)
››6 7
)
››7 8
{
ﬁﬁ 
return
ﬂﬂ .
 ServerAssignmentManagementResult
ﬂﬂ 7
.
ﬂﬂ7 8
ValidationError
ﬂﬂ8 G
(
ﬂﬂG H
$str
ﬂﬂH e
)
ﬂﬂe f
;
ﬂﬂf g
}
‡‡ 
var
„„ 
serverAssignments
„„ !
=
„„" #
new
„„$ '

Dictionary
„„( 2
<
„„2 3
Guid
„„3 7
,
„„7 8
IReadOnlyList
„„9 F
<
„„F G
Guid
„„G K
>
„„K L
>
„„L M
(
„„M N
)
„„N O
;
„„O P
foreach
ÂÂ 
(
ÂÂ 
var
ÂÂ 
tableId
ÂÂ  
in
ÂÂ! #
tableIdList
ÂÂ$ /
)
ÂÂ/ 0
{
ÊÊ 
var
ÁÁ 
	serverIds
ÁÁ 
=
ÁÁ &
serverAssignmentStrategy
ÁÁ  8
switch
ÁÁ9 ?
{
ËË &
ServerAssignmentStrategy
ÈÈ ,
.
ÈÈ, -
KeepExisting
ÈÈ- 9
=>
ÈÈ: <
new
ÈÈ= @
List
ÈÈA E
<
ÈÈE F
Guid
ÈÈF J
>
ÈÈJ K
{
ÈÈL M
Guid
ÈÈN R
.
ÈÈR S
NewGuid
ÈÈS Z
(
ÈÈZ [
)
ÈÈ[ \
}
ÈÈ] ^
,
ÈÈ^ _&
ServerAssignmentStrategy
ÍÍ ,
.
ÍÍ, -

MergeEqual
ÍÍ- 7
=>
ÍÍ8 :
new
ÍÍ; >
List
ÍÍ? C
<
ÍÍC D
Guid
ÍÍD H
>
ÍÍH I
{
ÍÍJ K
Guid
ÍÍL P
.
ÍÍP Q
NewGuid
ÍÍQ X
(
ÍÍX Y
)
ÍÍY Z
,
ÍÍZ [
Guid
ÍÍ\ `
.
ÍÍ` a
NewGuid
ÍÍa h
(
ÍÍh i
)
ÍÍi j
}
ÍÍk l
,
ÍÍl m&
ServerAssignmentStrategy
ÎÎ ,
.
ÎÎ, -

UsePrimary
ÎÎ- 7
=>
ÎÎ8 :
new
ÎÎ; >
List
ÎÎ? C
<
ÎÎC D
Guid
ÎÎD H
>
ÎÎH I
{
ÎÎJ K
tableIdList
ÎÎL W
.
ÎÎW X
First
ÎÎX ]
(
ÎÎ] ^
)
ÎÎ^ _
}
ÎÎ` a
,
ÎÎa b&
ServerAssignmentStrategy
ÏÏ ,
.
ÏÏ, -
ClearAll
ÏÏ- 5
=>
ÏÏ6 8
new
ÏÏ9 <
List
ÏÏ= A
<
ÏÏA B
Guid
ÏÏB F
>
ÏÏF G
(
ÏÏG H
)
ÏÏH I
,
ÏÏI J
_
ÌÌ 
=>
ÌÌ 
new
ÌÌ 
List
ÌÌ !
<
ÌÌ! "
Guid
ÌÌ" &
>
ÌÌ& '
{
ÌÌ( )
Guid
ÌÌ* .
.
ÌÌ. /
NewGuid
ÌÌ/ 6
(
ÌÌ6 7
)
ÌÌ7 8
}
ÌÌ9 :
}
ÓÓ 
;
ÓÓ 
serverAssignments
 !
[
! "
tableId
" )
]
) *
=
+ ,
	serverIds
- 6
;
6 7
}
ÒÒ 
var
ÛÛ 
managementData
ÛÛ 
=
ÛÛ  
new
ÛÛ! $,
ServerAssignmentManagementData
ÛÛ% C
(
ÛÛC D
operationType
ÙÙ 
,
ÙÙ 
tableIdList
ıı 
,
ıı 
serverAssignments
ˆˆ !
,
ˆˆ! "
DateTime
˜˜ 
.
˜˜ 
UtcNow
˜˜ 
)
¯¯ 
;
¯¯ 
return
˙˙ .
 ServerAssignmentManagementResult
˙˙ 3
.
˙˙3 4
Success
˙˙4 ;
(
˙˙; <
managementData
˙˙< J
)
˙˙J K
;
˙˙K L
}
˚˚ 	
catch
¸¸ 
(
¸¸ 
	Exception
¸¸ 
ex
¸¸ 
)
¸¸ 
{
˝˝ 	
return
˛˛ .
 ServerAssignmentManagementResult
˛˛ 3
.
˛˛3 4
InvalidOperation
˛˛4 D
(
˛˛D E
$"
˛˛E G
$str
˛˛G l
{
˛˛l m
ex
˛˛m o
.
˛˛o p
Message
˛˛p w
}
˛˛w x
"
˛˛x y
)
˛˛y z
;
˛˛z {
}
ˇˇ 	
}
ÄÄ 
}ÅÅ ¿´
yC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\ServerAssignmentService.cs
	namespace		 	
Magidesk		
 
.		 
Domain		 
.		 
Services		 "
;		" #
public 
class #
ServerAssignmentService $
:% &$
IServerAssignmentService' ?
{ 
public 

async 
Task 
< "
ServerAssignmentResult ,
>, -&
AssignServerToSessionAsync. H
(H I
Guid 
	sessionId 
, 
Guid 
serverId 
, 
bool 
	isPrimary 
= 
true 
, 
decimal  
allocationPercentage $
=% &
$num' +
)+ ,
{ 
try 
{ 	
if 
( 
	sessionId 
== 
Guid !
.! "
Empty" '
)' (
return "
ServerAssignmentResult -
.- .
ValidationError. =
(= >
$str> Z
)Z [
;[ \
if 
( 
serverId 
== 
Guid  
.  !
Empty! &
)& '
return "
ServerAssignmentResult -
.- .
ValidationError. =
(= >
$str> Y
)Y Z
;Z [
if 
(  
allocationPercentage $
<=% '
$num( )
||* , 
allocationPercentage- A
>B C
$numD G
)G H
return   "
ServerAssignmentResult   -
.  - .
ValidationError  . =
(  = >
$str  > o
)  o p
;  p q
var## 

assignment## 
=## 
ServerAssignment## -
.##- .
Create##. 4
(##4 5
	sessionId##5 >
,##> ?
serverId##@ H
,##H I
	isPrimary##J S
,##S T 
allocationPercentage##U i
)##i j
;##j k
var%% 
data%% 
=%% 
new%%  
ServerAssignmentData%% /
(%%/ 0

assignment&& 
.&& 
Id&& 
,&& 

assignment'' 
.'' 
	SessionId'' $
,''$ %

assignment(( 
.(( 
ServerId(( #
,((# $

assignment)) 
.)) 
	IsPrimary)) $
,))$ %

assignment** 
.**  
AllocationPercentage** /
,**/ 0

assignment++ 
.++ 

AssignedAt++ %
),, 
;,, 
return.. "
ServerAssignmentResult.. )
...) *
Success..* 1
(..1 2
data..2 6
)..6 7
;..7 8
}// 	
catch00 
(00 *
BusinessRuleViolationException00 -
ex00. 0
)000 1
{11 	
return22 "
ServerAssignmentResult22 )
.22) *
ValidationError22* 9
(229 :
ex22: <
.22< =
Message22= D
)22D E
;22E F
}33 	
catch44 
(44 
ArgumentException44  
ex44! #
)44# $
{55 	
return66 "
ServerAssignmentResult66 )
.66) *
ValidationError66* 9
(669 :
ex66: <
.66< =
Message66= D
)66D E
;66E F
}77 	
}88 
public:: 

async:: 
Task:: 
<:: "
ServerAssignmentResult:: ,
>::, -
ReassignServerAsync::. A
(::A B
Guid;; 
	sessionId;; 
,;; 
Guid<< 
newServerId<< 
,<< 
string== 
reason== 
)== 
{>> 
try?? 
{@@ 	
ifBB 
(BB 
	sessionIdBB 
==BB 
GuidBB !
.BB! "
EmptyBB" '
)BB' (
returnCC "
ServerAssignmentResultCC -
.CC- .
ValidationErrorCC. =
(CC= >
$strCC> Z
)CCZ [
;CC[ \
ifEE 
(EE 
newServerIdEE 
==EE 
GuidEE #
.EE# $
EmptyEE$ )
)EE) *
returnFF "
ServerAssignmentResultFF -
.FF- .
ValidationErrorFF. =
(FF= >
$strFF> Y
)FFY Z
;FFZ [
ifHH 
(HH 
stringHH 
.HH 
IsNullOrWhiteSpaceHH )
(HH) *
reasonHH* 0
)HH0 1
)HH1 2
returnII "
ServerAssignmentResultII -
.II- .
ValidationErrorII. =
(II= >
$strII> c
)IIc d
;IId e
varRR 

assignmentRR 
=RR 
ServerAssignmentRR -
.RR- .
CreateRR. 4
(RR4 5
	sessionIdRR5 >
,RR> ?
newServerIdRR@ K
,RRK L
trueRRM Q
,RRQ R
$numRRS W
)RRW X
;RRX Y
varTT 
dataTT 
=TT 
newTT  
ServerAssignmentDataTT /
(TT/ 0

assignmentUU 
.UU 
IdUU 
,UU 

assignmentVV 
.VV 
	SessionIdVV $
,VV$ %

assignmentWW 
.WW 
ServerIdWW #
,WW# $

assignmentXX 
.XX 
	IsPrimaryXX $
,XX$ %

assignmentYY 
.YY  
AllocationPercentageYY /
,YY/ 0

assignmentZZ 
.ZZ 

AssignedAtZZ %
)[[ 
;[[ 
return]] "
ServerAssignmentResult]] )
.]]) *
Success]]* 1
(]]1 2
data]]2 6
)]]6 7
;]]7 8
}^^ 	
catch__ 
(__ *
BusinessRuleViolationException__ -
ex__. 0
)__0 1
{`` 	
returnaa "
ServerAssignmentResultaa )
.aa) *
ValidationErroraa* 9
(aa9 :
exaa: <
.aa< =
Messageaa= D
)aaD E
;aaE F
}bb 	
catchcc 
(cc 
ArgumentExceptioncc  
excc! #
)cc# $
{dd 	
returnee "
ServerAssignmentResultee )
.ee) *
ValidationErroree* 9
(ee9 :
exee: <
.ee< =
Messageee= D
)eeD E
;eeE F
}ff 	
}gg 
publicii 

asyncii 
Taskii 
<ii "
ServerAssignmentResultii ,
>ii, -#
AddSecondaryServerAsyncii. E
(iiE F
Guidjj 
	sessionIdjj 
,jj 
Guidkk 
serverIdkk 
,kk 
decimalll  
allocationPercentagell $
)ll$ %
{mm 
trynn 
{oo 	
ifqq 
(qq 
	sessionIdqq 
==qq 
Guidqq !
.qq! "
Emptyqq" '
)qq' (
returnrr "
ServerAssignmentResultrr -
.rr- .
ValidationErrorrr. =
(rr= >
$strrr> Z
)rrZ [
;rr[ \
iftt 
(tt 
serverIdtt 
==tt 
Guidtt  
.tt  !
Emptytt! &
)tt& '
returnuu "
ServerAssignmentResultuu -
.uu- .
ValidationErroruu. =
(uu= >
$struu> Y
)uuY Z
;uuZ [
ifww 
(ww  
allocationPercentageww $
<=ww% '
$numww( )
||ww* , 
allocationPercentageww- A
>wwB C
$numwwD G
)wwG H
returnxx "
ServerAssignmentResultxx -
.xx- .
ValidationErrorxx. =
(xx= >
$strxx> o
)xxo p
;xxp q
var{{ 

assignment{{ 
={{ 
ServerAssignment{{ -
.{{- .
Create{{. 4
({{4 5
	sessionId{{5 >
,{{> ?
serverId{{@ H
,{{H I
false{{J O
,{{O P 
allocationPercentage{{Q e
){{e f
;{{f g
var}} 
data}} 
=}} 
new}}  
ServerAssignmentData}} /
(}}/ 0

assignment~~ 
.~~ 
Id~~ 
,~~ 

assignment 
. 
	SessionId $
,$ %

assignment
ÄÄ 
.
ÄÄ 
ServerId
ÄÄ #
,
ÄÄ# $

assignment
ÅÅ 
.
ÅÅ 
	IsPrimary
ÅÅ $
,
ÅÅ$ %

assignment
ÇÇ 
.
ÇÇ "
AllocationPercentage
ÇÇ /
,
ÇÇ/ 0

assignment
ÉÉ 
.
ÉÉ 

AssignedAt
ÉÉ %
)
ÑÑ 
;
ÑÑ 
return
ÜÜ $
ServerAssignmentResult
ÜÜ )
.
ÜÜ) *
Success
ÜÜ* 1
(
ÜÜ1 2
data
ÜÜ2 6
)
ÜÜ6 7
;
ÜÜ7 8
}
áá 	
catch
àà 
(
àà ,
BusinessRuleViolationException
àà -
ex
àà. 0
)
àà0 1
{
ââ 	
return
ää $
ServerAssignmentResult
ää )
.
ää) *
ValidationError
ää* 9
(
ää9 :
ex
ää: <
.
ää< =
Message
ää= D
)
ääD E
;
ääE F
}
ãã 	
catch
åå 
(
åå 
ArgumentException
åå  
ex
åå! #
)
åå# $
{
çç 	
return
éé $
ServerAssignmentResult
éé )
.
éé) *
ValidationError
éé* 9
(
éé9 :
ex
éé: <
.
éé< =
Message
éé= D
)
ééD E
;
ééE F
}
èè 	
}
êê 
public
íí 

async
íí 
Task
íí 
<
íí $
ServerAssignmentResult
íí ,
>
íí, -)
RemoveServerAssignmentAsync
íí. I
(
ííI J
Guid
ìì 
	sessionId
ìì 
,
ìì 
Guid
îî 
serverId
îî 
)
îî 
{
ïï 
try
ññ 
{
óó 	
if
ôô 
(
ôô 
	sessionId
ôô 
==
ôô 
Guid
ôô !
.
ôô! "
Empty
ôô" '
)
ôô' (
return
öö $
ServerAssignmentResult
öö -
.
öö- .
ValidationError
öö. =
(
öö= >
$str
öö> Z
)
ööZ [
;
öö[ \
if
úú 
(
úú 
serverId
úú 
==
úú 
Guid
úú  
.
úú  !
Empty
úú! &
)
úú& '
return
ùù $
ServerAssignmentResult
ùù -
.
ùù- .
ValidationError
ùù. =
(
ùù= >
$str
ùù> Y
)
ùùY Z
;
ùùZ [
return
§§ $
ServerAssignmentResult
§§ )
.
§§) *
Success
§§* 1
(
§§1 2
)
§§2 3
;
§§3 4
}
•• 	
catch
¶¶ 
(
¶¶ 
System
¶¶ 
.
¶¶ '
InvalidOperationException
¶¶ /
ex
¶¶0 2
)
¶¶2 3
{
ßß 	
return
®® $
ServerAssignmentResult
®® )
.
®®) *
InvalidOperation
®®* :
(
®®: ;
ex
®®; =
.
®®= >
Message
®®> E
)
®®E F
;
®®F G
}
©© 	
}
™™ 
public
¨¨ 

async
¨¨ 
Task
¨¨ 
<
¨¨ !
TipAllocationResult
¨¨ )
>
¨¨) *)
CalculateTipAllocationAsync
¨¨+ F
(
¨¨F G
Guid
≠≠ 
	sessionId
≠≠ 
,
≠≠ 
Money
ÆÆ 
totalTipAmount
ÆÆ 
)
ÆÆ 
{
ØØ 
try
∞∞ 
{
±± 	
if
≥≥ 
(
≥≥ 
	sessionId
≥≥ 
==
≥≥ 
Guid
≥≥ !
.
≥≥! "
Empty
≥≥" '
)
≥≥' (
return
¥¥ !
TipAllocationResult
¥¥ *
.
¥¥* +
ValidationError
¥¥+ :
(
¥¥: ;
	sessionId
¥¥; D
,
¥¥D E
totalTipAmount
¥¥F T
,
¥¥T U
$str
¥¥V r
)
¥¥r s
;
¥¥s t
if
∂∂ 
(
∂∂ 
totalTipAmount
∂∂ 
.
∂∂ 
Amount
∂∂ %
<
∂∂& '
$num
∂∂( )
)
∂∂) *
return
∑∑ !
TipAllocationResult
∑∑ *
.
∑∑* +
ValidationError
∑∑+ :
(
∑∑: ;
	sessionId
∑∑; D
,
∑∑D E
totalTipAmount
∑∑F T
,
∑∑T U
$str
∑∑V u
)
∑∑u v
;
∑∑v w
var
¿¿ 
allocations
¿¿ 
=
¿¿ 
new
¿¿ !
List
¿¿" &
<
¿¿& '!
ServerTipAllocation
¿¿' :
>
¿¿: ;
{
¡¡ 
new
¬¬ !
ServerTipAllocation
¬¬ '
(
¬¬' (
Guid
√√ 
.
√√ 
NewGuid
√√  
(
√√  !
)
√√! "
,
√√" #
$str
ƒƒ #
,
ƒƒ# $
$num
≈≈ 
,
≈≈ 
totalTipAmount
∆∆ "
,
∆∆" #
true
«« 
)
»» 
}
…… 
;
…… 
return
ÀÀ !
TipAllocationResult
ÀÀ &
.
ÀÀ& '
Success
ÀÀ' .
(
ÀÀ. /
	sessionId
ÀÀ/ 8
,
ÀÀ8 9
totalTipAmount
ÀÀ: H
,
ÀÀH I
allocations
ÀÀJ U
)
ÀÀU V
;
ÀÀV W
}
ÃÃ 	
catch
ÕÕ 
(
ÕÕ 
	Exception
ÕÕ 
ex
ÕÕ 
)
ÕÕ 
{
ŒŒ 	
return
œœ !
TipAllocationResult
œœ &
.
œœ& '
ValidationError
œœ' 6
(
œœ6 7
	sessionId
œœ7 @
,
œœ@ A
totalTipAmount
œœB P
,
œœP Q
ex
œœR T
.
œœT U
Message
œœU \
)
œœ\ ]
;
œœ] ^
}
–– 	
}
—— 
public
”” 

async
”” 
Task
”” 
<
”” &
ServerPerformanceMetrics
”” .
>
””. /.
 GetServerPerformanceMetricsAsync
””0 P
(
””P Q
Guid
‘‘ 
serverId
‘‘ 
,
‘‘ 
DateTime
’’ 
fromDate
’’ 
,
’’ 
DateTime
÷÷ 
toDate
÷÷ 
)
÷÷ 
{
◊◊ 
if
ŸŸ 

(
ŸŸ 
serverId
ŸŸ 
==
ŸŸ 
Guid
ŸŸ 
.
ŸŸ 
Empty
ŸŸ "
)
ŸŸ" #
throw
⁄⁄ 
new
⁄⁄ 
ArgumentException
⁄⁄ '
(
⁄⁄' (
$str
⁄⁄( C
,
⁄⁄C D
nameof
⁄⁄E K
(
⁄⁄K L
serverId
⁄⁄L T
)
⁄⁄T U
)
⁄⁄U V
;
⁄⁄V W
if
‹‹ 

(
‹‹ 
fromDate
‹‹ 
>
‹‹ 
toDate
‹‹ 
)
‹‹ 
throw
›› 
new
›› 
ArgumentException
›› '
(
››' (
$str
››( K
,
››K L
nameof
››M S
(
››S T
fromDate
››T \
)
››\ ]
)
››] ^
;
››^ _
return
‚‚ 
new
‚‚ &
ServerPerformanceMetrics
‚‚ +
(
‚‚+ ,
ServerId
„„ 
:
„„ 
serverId
„„ 
,
„„ 

ServerName
‰‰ 
:
‰‰ 
$str
‰‰ '
,
‰‰' (
FromDate
ÂÂ 
:
ÂÂ 
fromDate
ÂÂ 
,
ÂÂ 
ToDate
ÊÊ 
:
ÊÊ 
toDate
ÊÊ 
,
ÊÊ !
TotalSessionsServed
ÁÁ 
:
ÁÁ  
$num
ÁÁ! "
,
ÁÁ" #
TotalServiceTime
ËË 
:
ËË 
TimeSpan
ËË &
.
ËË& '
Zero
ËË' +
,
ËË+ ,!
TotalSalesGenerated
ÈÈ 
:
ÈÈ  
Money
ÈÈ! &
.
ÈÈ& '
Zero
ÈÈ' +
(
ÈÈ+ ,
)
ÈÈ, -
,
ÈÈ- .
TotalTipsEarned
ÍÍ 
:
ÍÍ 
Money
ÍÍ "
.
ÍÍ" #
Zero
ÍÍ# '
(
ÍÍ' (
)
ÍÍ( )
,
ÍÍ) *$
AverageSessionDuration
ÎÎ "
:
ÎÎ" #
$num
ÎÎ$ %
,
ÎÎ% &'
CustomerSatisfactionScore
ÏÏ %
:
ÏÏ% &
$num
ÏÏ' (
,
ÏÏ( )!
PrimarySessionCount
ÌÌ 
:
ÌÌ  
$num
ÌÌ! "
,
ÌÌ" ##
SecondarySessionCount
ÓÓ !
:
ÓÓ! "
$num
ÓÓ# $
,
ÓÓ$ %"
AverageTipPerSession
ÔÔ  
:
ÔÔ  !
Money
ÔÔ" '
.
ÔÔ' (
Zero
ÔÔ( ,
(
ÔÔ, -
)
ÔÔ- .
,
ÔÔ. /
SalesPerHour
 
:
 
$num
 
)
ÒÒ 	
;
ÒÒ	 

}
ÚÚ 
public
ÙÙ 

async
ÙÙ 
Task
ÙÙ 
<
ÙÙ 
IEnumerable
ÙÙ !
<
ÙÙ! "
ServerAssignment
ÙÙ" 2
>
ÙÙ2 3
>
ÙÙ3 4-
GetActiveServerAssignmentsAsync
ÙÙ5 T
(
ÙÙT U
Guid
ÙÙU Y
	sessionId
ÙÙZ c
)
ÙÙc d
{
ıı 
if
˜˜ 

(
˜˜ 
	sessionId
˜˜ 
==
˜˜ 
Guid
˜˜ 
.
˜˜ 
Empty
˜˜ #
)
˜˜# $
throw
¯¯ 
new
¯¯ 
ArgumentException
¯¯ '
(
¯¯' (
$str
¯¯( D
,
¯¯D E
nameof
¯¯F L
(
¯¯L M
	sessionId
¯¯M V
)
¯¯V W
)
¯¯W X
;
¯¯X Y
return
˝˝ 
new
˝˝ 
List
˝˝ 
<
˝˝ 
ServerAssignment
˝˝ (
>
˝˝( )
(
˝˝) *
)
˝˝* +
;
˝˝+ ,
}
˛˛ 
public
ÄÄ 

async
ÄÄ 
Task
ÄÄ 
<
ÄÄ 
bool
ÄÄ 
>
ÄÄ 0
"ValidateAllocationPercentagesAsync
ÄÄ >
(
ÄÄ> ?
Guid
ÄÄ? C
	sessionId
ÄÄD M
)
ÄÄM N
{
ÅÅ 
if
ÉÉ 

(
ÉÉ 
	sessionId
ÉÉ 
==
ÉÉ 
Guid
ÉÉ 
.
ÉÉ 
Empty
ÉÉ #
)
ÉÉ# $
return
ÑÑ 
false
ÑÑ 
;
ÑÑ 
return
ãã 
true
ãã 
;
ãã 
}
åå 
public
éé 

async
éé 
Task
éé 
<
éé 
ServerAnalytics
éé %
>
éé% &%
GetServerAnalyticsAsync
éé' >
(
éé> ?
Guid
èè 
serverId
èè 
,
èè 
DateTime
êê 
fromDate
êê 
,
êê 
DateTime
ëë 
toDate
ëë 
)
ëë 
{
íí 
if
îî 

(
îî 
serverId
îî 
==
îî 
Guid
îî 
.
îî 
Empty
îî "
)
îî" #
throw
ïï 
new
ïï 
ArgumentException
ïï '
(
ïï' (
$str
ïï( C
,
ïïC D
nameof
ïïE K
(
ïïK L
serverId
ïïL T
)
ïïT U
)
ïïU V
;
ïïV W
if
óó 

(
óó 
fromDate
óó 
>
óó 
toDate
óó 
)
óó 
throw
òò 
new
òò 
ArgumentException
òò '
(
òò' (
$str
òò( K
,
òòK L
nameof
òòM S
(
òòS T
fromDate
òòT \
)
òò\ ]
)
òò] ^
;
òò^ _
var
úú  
performanceMetrics
úú 
=
úú  
await
úú! &.
 GetServerPerformanceMetricsAsync
úú' G
(
úúG H
serverId
úúH P
,
úúP Q
fromDate
úúR Z
,
úúZ [
toDate
úú\ b
)
úúb c
;
úúc d
return
ûû 
new
ûû 
ServerAnalytics
ûû "
(
ûû" #
ServerId
üü 
:
üü 
serverId
üü 
,
üü 

ServerName
†† 
:
†† 
$str
†† '
,
††' (
FromDate
°° 
:
°° 
fromDate
°° 
,
°° 
ToDate
¢¢ 
:
¢¢ 
toDate
¢¢ 
,
¢¢  
PerformanceMetrics
££ 
:
££  
performanceMetrics
££  2
,
££2 3
DailyBreakdown
§§ 
:
§§ 
new
§§ 
List
§§  $
<
§§$ % 
DailyServerMetrics
§§% 7
>
§§7 8
(
§§8 9
)
§§9 :
,
§§: ;
CommissionData
•• 
:
•• 
new
•• #
CommissionCalculation
••  5
(
••5 6

BaseSalary
¶¶ 
:
¶¶ 
Money
¶¶ !
.
¶¶! "
Zero
¶¶" &
(
¶¶& '
)
¶¶' (
,
¶¶( )
CommissionEarned
ßß  
:
ßß  !
Money
ßß" '
.
ßß' (
Zero
ßß( ,
(
ßß, -
)
ßß- .
,
ßß. /
CommissionRate
®® 
:
®® 
$num
®®  !
,
®®! "
TotalCompensation
©© !
:
©©! "
Money
©©# (
.
©©( )
Zero
©©) -
(
©©- .
)
©©. /
,
©©/ 0
BonusEligible
™™ 
:
™™ 
Money
™™ $
.
™™$ %
Zero
™™% )
(
™™) *
)
™™* +
)
´´ 
,
´´ 
Ranking
¨¨ 
:
¨¨ 
new
¨¨ 
ServerRanking
¨¨ &
(
¨¨& '
	SalesRank
≠≠ 
:
≠≠ 
$num
≠≠ 
,
≠≠ 
TipsRank
ÆÆ 
:
ÆÆ 
$num
ÆÆ 
,
ÆÆ 
SessionCountRank
ØØ  
:
ØØ  !
$num
ØØ" #
,
ØØ# $&
CustomerSatisfactionRank
∞∞ (
:
∞∞( )
$num
∞∞* +
,
∞∞+ ,
OverallRank
±± 
:
±± 
$num
±± 
,
±± 
TotalServers
≤≤ 
:
≤≤ 
$num
≤≤ 
)
≥≥ 
)
¥¥ 	
;
¥¥	 

}
µµ 
}∂∂ ´'
pC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\PricingService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Services "
;" #
public 
class 
PricingService 
: 
IPricingService -
{ 
public 

Money 
CalculateTimeCharge $
($ %
TimeSpan 
billableTime 
, 
	TableType 
	tableType 
) 
{ 
if 

( 
	tableType 
== 
null 
) 
{ 	
throw 
new !
ArgumentNullException +
(+ ,
nameof, 2
(2 3
	tableType3 <
)< =
)= >
;> ?
} 	
if 

( 
billableTime 
< 
TimeSpan #
.# $
Zero$ (
)( )
{   	
throw!! 
new!! 
ArgumentException!! '
(!!' (
$str!!( K
,!!K L
nameof!!M S
(!!S T
billableTime!!T `
)!!` a
)!!a b
;!!b c
}"" 	
if%% 

(%% 
billableTime%% 
==%% 
TimeSpan%% $
.%%$ %
Zero%%% )
)%%) *
{&& 	
return'' 
new'' 
Money'' 
('' 
$num'' 
)''  
;''  !
}(( 	
var++ 
roundedMinutes++ 
=++ 
	RoundTime++ &
(++& '
billableTime++' 3
,++3 4
	tableType++5 >
.++> ?
RoundingMinutes++? N
)++N O
;++O P
if.. 

(.. 
roundedMinutes.. 
<.. 
	tableType.. &
...& '
MinimumMinutes..' 5
)..5 6
{// 	
roundedMinutes00 
=00 
	tableType00 &
.00& '
MinimumMinutes00' 5
;005 6
}11 	
var44 
totalCharge44 
=44 "
CalculateChargeForTime44 0
(440 1
roundedMinutes441 ?
,44? @
	tableType44A J
)44J K
;44K L
return88 
new88 
Money88 
(88 
totalCharge88 $
)88$ %
;88% &
}99 
privateAA 
intAA 
	RoundTimeAA 
(AA 
TimeSpanAA "
timeAA# '
,AA' (
intAA) ,
roundingMinutesAA- <
)AA< =
{BB 
varCC 
totalMinutesCC 
=CC 
(CC 
intCC 
)CC  
MathCC  $
.CC$ %
CeilingCC% ,
(CC, -
timeCC- 1
.CC1 2
TotalMinutesCC2 >
)CC> ?
;CC? @
ifFF 

(FF 
roundingMinutesFF 
<=FF 
$numFF  
)FF  !
{GG 	
returnHH 
totalMinutesHH 
;HH  
}II 	
varMM 
	intervalsMM 
=MM 
(MM 
intMM 
)MM 
MathMM !
.MM! "
CeilingMM" )
(MM) *
(MM* +
doubleMM+ 1
)MM1 2
totalMinutesMM2 >
/MM? @
roundingMinutesMMA P
)MMP Q
;MMQ R
returnNN 
	intervalsNN 
*NN 
roundingMinutesNN *
;NN* +
}OO 
privateWW 
decimalWW "
CalculateChargeForTimeWW *
(WW* +
intWW+ .
totalMinutesWW/ ;
,WW; <
	TableTypeWW= F
	tableTypeWWG P
)WWP Q
{XX 
decimalYY 
totalChargeYY 
=YY 
$numYY  
;YY  !
intZZ 
remainingMinutesZZ 
=ZZ 
totalMinutesZZ +
;ZZ+ ,
if]] 

(]] 
	tableType]] 
.]] 
FirstHourRate]] #
.]]# $
HasValue]]$ ,
&&]]- /
totalMinutes]]0 <
>=]]= ?
$num]]@ B
)]]B C
{^^ 	
totalCharge`` 
+=`` 
	tableType`` $
.``$ %
FirstHourRate``% 2
.``2 3
Value``3 8
;``8 9
remainingMinutesaa 
-=aa 
$numaa  "
;aa" #
}bb 	
ifee 

(ee 
remainingMinutesee 
>ee 
$numee  
)ee  !
{ff 	
varhh 
remainingHourshh 
=hh  
remainingMinuteshh! 1
/hh2 3
$numhh4 9
;hh9 :
totalChargeii 
+=ii 
remainingHoursii )
*ii* +
	tableTypeii, 5
.ii5 6

HourlyRateii6 @
;ii@ A
}jj 	
returnll 
totalChargell 
;ll 
}mm 
}nn Õ
qC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\PriceCalculator.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Services "
;" #
public 
class 
PriceCalculator 
{ 
public 

void '
RecalculateFractionalPrices +
(+ ,
List, 0
<0 1
OrderLineModifier1 B
>B C
	modifiersD M
,M N
PriceStrategyO \
strategy] e
)e f
{ 
if 

( 
	modifiers 
== 
null 
||  
!! "
	modifiers" +
.+ ,
Any, /
(/ 0
)0 1
)1 2
return3 9
;9 :
var 
fractionalModifiers 
=  !
	modifiers" +
.+ ,
Where, 1
(1 2
m2 3
=>4 6
m7 8
.8 9
IsSectionWisePrice9 K
)K L
.L M
ToListM S
(S T
)T U
;U V
if 

( 
! 
fractionalModifiers  
.  !
Any! $
($ %
)% &
)& '
return( .
;. /
switch"" 
("" 
strategy"" 
)"" 
{## 	
case$$ 
PriceStrategy$$ 
.$$ 
SumOfHalves$$ *
:$$* +
case%% 
PriceStrategy%% 
.%% 
AverageOfHalves%% .
:%%. /
foreach'' 
('' 
var'' 
mod''  
in''! #
fractionalModifiers''$ 7
)''7 8
{(( 
var)) 
newPrice))  
=))! "
mod))# &
.))& '
	BasePrice))' 0
*))1 2
mod))3 6
.))6 7
PortionValue))7 C
;))C D
mod** 
.** 
UpdateUnitPrice** '
(**' (
newPrice**( 0
)**0 1
;**1 2
}++ 
break,, 
;,, 
case.. 
PriceStrategy.. 
... 
HighestHalf.. *
:..* +
case// 
PriceStrategy// 
.// 
WholePie// '
://' (
if11 
(11 
!11 
fractionalModifiers11 (
.11( )
Any11) ,
(11, -
)11- .
)11. /
break110 5
;115 6
var33 
maxBasePrice33  
=33! "
fractionalModifiers33# 6
.336 7
Max337 :
(33: ;
m33; <
=>33= ?
m33@ A
.33A B
	BasePrice33B K
)33K L
;33L M
if44 
(44 
maxBasePrice44  
==44! #
null44$ (
)44( )
maxBasePrice44* 6
=447 8
Money449 >
.44> ?
Zero44? C
(44C D
)44D E
;44E F
foreach66 
(66 
var66 
mod66  
in66! #
fractionalModifiers66$ 7
)667 8
{77 
var88 
newPrice88  
=88! "
maxBasePrice88# /
*880 1
mod882 5
.885 6
PortionValue886 B
;88B C
mod99 
.99 
UpdateUnitPrice99 '
(99' (
newPrice99( 0
)990 1
;991 2
}:: 
break;; 
;;; 
}<< 	
}== 
publicBB 

MoneyBB #
CalculateComboItemPriceBB (
(BB( )
MoneyBB) .
baseModifierPriceBB/ @
,BB@ A
MoneyBBB G
upchargeBBH P
)BBP Q
{CC 
returnHH 
upchargeHH 
;HH 
}II 
}JJ ü
yC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\ITableOperationsService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Services "
;" #
public 
	interface #
ITableOperationsService (
{ 
Task 
< 	 
TableOperationResult	 
> 
MergeTablesAsync /
(/ 0
Guid 
primaryTableId 
, 
IEnumerable 
< 
Guid 
> 
secondaryTableIds +
,+ ,
string 
reason 
, 
Guid 
staffId 
) 
; 
Task&& 
<&& 	 
TableOperationResult&&	 
>&& 
SplitTablesAsync&& /
(&&/ 0
Guid'' 
mergedSessionId'' 
,''  
TableSplitAllocation(( 
splitAllocation(( ,
,((, -
string)) 
reason)) 
,)) 
Guid** 
staffId** 
)** 
;** 
Task11 
<11 	
TableMergeStatus11	 
>11 $
GetTableMergeStatusAsync11 3
(113 4
Guid114 8
tableId119 @
)11@ A
;11A B
Task99 
<99 	&
TableMergeValidationResult99	 #
>99# $#
ValidateTableMergeAsync99% <
(99< =
Guid:: 
primaryTableId:: 
,:: 
IEnumerable;; 
<;; 
Guid;; 
>;; 
secondaryTableIds;; +
);;+ ,
;;;, -
TaskCC 
<CC 	&
TableSplitValidationResultCC	 #
>CC# $#
ValidateTableSplitAsyncCC% <
(CC< =
GuidDD 
mergedSessionIdDD 
,DD  
TableSplitAllocationEE 
splitAllocationEE ,
)EE, -
;EE- .
TaskNN 
<NN 	
IEnumerableNN	 
<NN $
TableOperationAuditEntryNN -
>NN- .
>NN. /,
 GetTableOperationAuditTrailAsyncNN0 P
(NNP Q
GuidOO 
tableIdOO 
,OO 
DateTimePP 
fromDatePP 
,PP 
DateTimeQQ 
toDateQQ 
)QQ 
;QQ 
TaskZZ 
<ZZ 	#
EquipmentTransferResultZZ	  
>ZZ  !"
TransferEquipmentAsyncZZ" 8
(ZZ8 9
Guid[[ 
fromTableId[[ 
,[[ 
Guid\\ 
	toTableId\\ 
,\\ 
IEnumerable]] 
<]] 
Guid]] 
>]] 
equipmentIds]] &
)]]& '
;]]' (
Taskff 
<ff 	,
 ServerAssignmentManagementResultff	 )
>ff) *7
+ManageServerAssignmentsDuringOperationAsyncff+ V
(ffV W
TableOperationTypegg 
operationTypegg (
,gg( )
IEnumerablehh 
<hh 
Guidhh 
>hh 
tableIdshh "
,hh" #$
ServerAssignmentStrategyii  $
serverAssignmentStrategyii! 9
)ii9 :
;ii: ;
}jj Û
xC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\ISessionControlService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Services "
;" #
public 
	interface "
ISessionControlService '
{ 
Task 
< 	 
SessionControlResult	 
> 
PauseSessionAsync 0
(0 1
Guid1 5
	sessionId6 ?
,? @
stringA G
reasonH N
)N O
;O P
Task 
< 	 
SessionControlResult	 
> 
ResumeSessionAsync 1
(1 2
Guid2 6
	sessionId7 @
)@ A
;A B
Task## 
<## 	 
SessionControlResult##	 
>## !
UpdateGuestCountAsync## 4
(##4 5
Guid##5 9
	sessionId##: C
,##C D
int##E H
newGuestCount##I V
,##V W
Guid##X \
staffId##] d
)##d e
;##e f
Task,, 
<,, 	 
SessionControlResult,,	 
>,,  
TransferSessionAsync,, 3
(,,3 4
Guid,,4 8
	sessionId,,9 B
,,,B C
Guid,,D H
targetTableId,,I V
,,,V W
string,,X ^
reason,,_ e
),,e f
;,,f g
Task22 
<22 	
IEnumerable22	 
<22 
SessionAlert22 !
>22! "
>22" #!
GetSessionAlertsAsync22$ 9
(229 :
)22: ;
;22; <
}33 ®
zC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\IServerAssignmentService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Services "
;" #
public 
	interface $
IServerAssignmentService )
{ 
Task 
< 	"
ServerAssignmentResult	 
>  &
AssignServerToSessionAsync! ;
(; <
Guid 
	sessionId 
, 
Guid 
serverId 
, 
bool 
	isPrimary 
= 
true 
, 
decimal  
allocationPercentage $
=% &
$num' +
)+ ,
;, -
Task$$ 
<$$ 	"
ServerAssignmentResult$$	 
>$$  
ReassignServerAsync$$! 4
($$4 5
Guid%% 
	sessionId%% 
,%% 
Guid&& 
newServerId&& 
,&& 
string'' 
reason'' 
)'' 
;'' 
Task00 
<00 	"
ServerAssignmentResult00	 
>00  #
AddSecondaryServerAsync00! 8
(008 9
Guid11 
	sessionId11 
,11 
Guid22 
serverId22 
,22 
decimal33  
allocationPercentage33 $
)33$ %
;33% &
Task;; 
<;; 	"
ServerAssignmentResult;;	 
>;;  '
RemoveServerAssignmentAsync;;! <
(;;< =
Guid<< 
	sessionId<< 
,<< 
Guid== 
serverId== 
)== 
;== 
TaskEE 
<EE 	
TipAllocationResultEE	 
>EE '
CalculateTipAllocationAsyncEE 9
(EE9 :
GuidFF 
	sessionIdFF 
,FF 
MoneyGG 
totalTipAmountGG 
)GG 
;GG 
TaskPP 
<PP 	$
ServerPerformanceMetricsPP	 !
>PP! ",
 GetServerPerformanceMetricsAsyncPP# C
(PPC D
GuidQQ 
serverIdQQ 
,QQ 
DateTimeRR 
fromDateRR 
,RR 
DateTimeSS 
toDateSS 
)SS 
;SS 
TaskZZ 
<ZZ 	
IEnumerableZZ	 
<ZZ 
ServerAssignmentZZ %
>ZZ% &
>ZZ& '+
GetActiveServerAssignmentsAsyncZZ( G
(ZZG H
GuidZZH L
	sessionIdZZM V
)ZZV W
;ZZW X
Taskaa 
<aa 	
boolaa	 
>aa .
"ValidateAllocationPercentagesAsyncaa 1
(aa1 2
Guidaa2 6
	sessionIdaa7 @
)aa@ A
;aaA B
Taskjj 
<jj 	
ServerAnalyticsjj	 
>jj #
GetServerAnalyticsAsyncjj 1
(jj1 2
Guidkk 
serverIdkk 
,kk 
DateTimell 
fromDatell 
,ll 
DateTimemm 
toDatemm 
)mm 
;mm 
}nn £
qC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\IPricingService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Services "
;" #
public		 
	interface		 
IPricingService		  
{

 
Money 	
CalculateTimeCharge
 
( 
TimeSpan 
billableTime 
, 
	TableType 
	tableType 
) 
; 
} Ÿ
yC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\IManagerOverrideService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Services "
;" #
public 
	interface #
IManagerOverrideService (
{ 
Task 
< 	
OverrideResult	 
> -
!ValidateManagerAuthorizationAsync :
(: ;
string; A

managerPinB L
,L M
GuidN R
userIdS Y
)Y Z
;Z [
Task 
< 	
OverrideResult	 
> $
ApplyTimeAdjustmentAsync 1
(1 2
Guid2 6
	sessionId7 @
,@ A
TimeSpanB J

adjustmentK U
,U V
stringW ]
reason^ d
,d e
Guidf j
	managerIdk t
)t u
;u v
Task'' 
<'' 	
OverrideResult''	 
>'' %
ApplyPricingOverrideAsync'' 2
(''2 3
Guid''3 7
	sessionId''8 A
,''A B
Money''C H
overrideAmount''I W
,''W X
string''Y _
reason''` f
,''f g
Guid''h l
	managerId''m v
)''v w
;''w x
Task00 
<00 	
OverrideResult00	 
>00  
ForceEndSessionAsync00 -
(00- .
Guid00. 2
	sessionId003 <
,00< =
string00> D
reason00E K
,00K L
Guid00M Q
	managerId00R [
)00[ \
;00\ ]
Task88 
<88 	
IEnumerable88	 
<88 
OverrideAuditEntry88 '
>88' (
>88( )&
GetOverrideAuditTrailAsync88* D
(88D E
DateTime88E M
fromDate88N V
,88V W
DateTime88X `
toDate88a g
)88g h
;88h i
Task?? 
<?? 	
IEnumerable??	 
<?? 
OverrideAuditEntry?? '
>??' (
>??( )-
!GetSessionOverrideAuditTrailAsync??* K
(??K L
Guid??L P
	sessionId??Q Z
)??Z [
;??[ \
TaskHH 
<HH 	
IEnumerableHH	 
<HH 
OverrideAuditEntryHH '
>HH' (
>HH( )-
!GetManagerOverrideAuditTrailAsyncHH* K
(HHK L
GuidHHL P
	managerIdHHQ Z
,HHZ [
DateTimeHH\ d
fromDateHHe m
,HHm n
DateTimeHHo w
toDateHHx ~
)HH~ 
;	HH Ä
}II “
rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\IGratuityService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Services "
;" #
public		 
	interface		 
IGratuityService		 !
{

 
GratuitySuggestions 
GetSuggestions &
(& '
Money' ,
subtotal- 5
)5 6
;6 7
void 
ApplyGratuity	 
( 
Ticket 
ticket $
,$ %
Money& +
amount, 2
,2 3
UserId4 :
?: ;
serverId< D
=E F
nullG K
)K L
;L M
} 
public"" 
record"" 
GratuitySuggestions"" !
(""! "
Money## 	
	Percent15##
 
,## 
Money$$ 	
	Percent18$$
 
,$$ 
Money%% 	
	Percent20%%
 
,%% 
Money&& 	
	Percent25&&
 
)'' 
;'' ∫
yC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\IAdvancedPricingService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Services "
;" #
public 
	interface #
IAdvancedPricingService (
:) *
IPricingService+ :
{ 
Task 
< 	
Money	 
> *
CalculateFirstHourPricingAsync .
(. /
TimeSpan/ 7
billableTime8 D
,D E
	TableTypeF O
	tableTypeP Y
)Y Z
;Z [
Task"" 
<"" 	
TimeSpan""	 
>"" "
ApplyTimeRoundingAsync"" )
("") *
TimeSpan""* 2
duration""3 ;
,""; <
TimeRoundingRule""= M
rule""N R
)""R S
;""S T
Task,, 
<,, 	
Money,,	 
>,, #
ApplyMinimumChargeAsync,, '
(,,' (
Money,,( -
calculatedCharge,,. >
,,,> ?
	TableType,,@ I
	tableType,,J S
),,S T
;,,T U
Task55 
<55 	#
PricingSimulationResult55	  
>55  ! 
SimulatePricingAsync55" 6
(556 7
PricingScenario557 F
scenario55G O
)55O P
;55P Q
Task>> 
<>> 	
bool>>	 
>>> %
ValidatePricingRulesAsync>> (
(>>( )
	TableType>>) 2
	tableType>>3 <
)>>< =
;>>= >
}??  
qC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\GratuityService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Services "
;" #
public 
class 
GratuityService 
: 
IGratuityService /
{ 
public 

GratuitySuggestions 
GetSuggestions -
(- .
Money. 3
subtotal4 <
)< =
{ 
if 

( 
subtotal 
== 
null 
) 
{ 	
throw 
new !
ArgumentNullException +
(+ ,
nameof, 2
(2 3
subtotal3 ;
); <
)< =
;= >
} 	
if 

( 
subtotal 
< 
Money 
. 
Zero !
(! "
)" #
)# $
{ 	
throw 
new *
BusinessRuleViolationException 4
(4 5
$str5 f
)f g
;g h
} 	
var 
	percent15 
= 
subtotal  
*! "
$num# (
;( )
var 
	percent18 
= 
subtotal  
*! "
$num# (
;( )
var   
	percent20   
=   
subtotal    
*  ! "
$num  # (
;  ( )
var!! 
	percent25!! 
=!! 
subtotal!!  
*!!! "
$num!!# (
;!!( )
return## 
new## 
GratuitySuggestions## &
(##& '
	Percent15$$ 
:$$ 
	percent15$$  
,$$  !
	Percent18%% 
:%% 
	percent18%%  
,%%  !
	Percent20&& 
:&& 
	percent20&&  
,&&  !
	Percent25'' 
:'' 
	percent25''  
)(( 	
;((	 

})) 
public// 

void// 
ApplyGratuity// 
(// 
Ticket// $
ticket//% +
,//+ ,
Money//- 2
amount//3 9
,//9 :
UserId//; A
?//A B
serverId//C K
=//L M
null//N R
)//R S
{00 
if11 

(11 
ticket11 
==11 
null11 
)11 
{22 	
throw33 
new33 !
ArgumentNullException33 +
(33+ ,
nameof33, 2
(332 3
ticket333 9
)339 :
)33: ;
;33; <
}44 	
if66 

(66 
amount66 
==66 
null66 
)66 
{77 	
throw88 
new88 !
ArgumentNullException88 +
(88+ ,
nameof88, 2
(882 3
amount883 9
)889 :
)88: ;
;88; <
}99 	
if;; 

(;; 
amount;; 
<;; 
Money;; 
.;; 
Zero;; 
(;;  
);;  !
);;! "
{<< 	
throw== 
new== *
BusinessRuleViolationException== 4
(==4 5
$str==5 Z
)==Z [
;==[ \
}>> 	
varAA 
ownerIdAA 
=AA 
serverIdAA 
??AA !
ticketAA" (
.AA( )
	CreatedByAA) 2
;AA2 3
varDD 
gratuityDD 
=DD 
GratuityDD 
.DD  
CreateDD  &
(DD& '
ticketIdEE 
:EE 
ticketEE 
.EE 
IdEE 
,EE  
amountFF 
:FF 
amountFF 
,FF 

terminalIdGG 
:GG 
ticketGG 
.GG 

TerminalIdGG )
,GG) *
ownerIdHH 
:HH 
ownerIdHH 
)II 	
;II	 

ticketLL 
.LL 
AddGratuityLL 
(LL 
gratuityLL #
)LL# $
;LL$ %
}MM 
}NN ´∞
xC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Services\AdvancedPricingService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Services "
;" #
public 
class "
AdvancedPricingService #
:$ %
PricingService& 4
,4 5#
IAdvancedPricingService6 M
{ 
public 

async 
Task 
< 
Money 
> *
CalculateFirstHourPricingAsync ;
(; <
TimeSpan< D
billableTimeE Q
,Q R
	TableTypeS \
	tableType] f
)f g
{ 
if 

( 
	tableType 
== 
null 
) 
{ 	
throw 
new !
ArgumentNullException +
(+ ,
nameof, 2
(2 3
	tableType3 <
)< =
)= >
;> ?
}   	
if"" 

("" 
billableTime"" 
<"" 
TimeSpan"" #
.""# $
Zero""$ (
)""( )
{## 	
throw$$ 
new$$ 
ArgumentException$$ '
($$' (
$str$$( K
,$$K L
nameof$$M S
($$S T
billableTime$$T `
)$$` a
)$$a b
;$$b c
}%% 	
if(( 

((( 
billableTime(( 
==(( 
TimeSpan(( $
.(($ %
Zero((% )
)(() *
{)) 	
return** 
Money** 
.** 
Zero** 
(** 
)** 
;**  
}++ 	
if.. 

(.. 
!.. 
	tableType.. 
... 
FirstHourRate.. $
...$ %
HasValue..% -
)..- .
{// 	
return00 
CalculateTimeCharge00 &
(00& '
billableTime00' 3
,003 4
	tableType005 >
)00> ?
;00? @
}11 	
var33 
firstHourTime33 
=33 
TimeSpan33 $
.33$ %
	FromHours33% .
(33. /
$num33/ 0
)330 1
;331 2
Money44 
totalCharge44 
=44 
Money44 !
.44! "
Zero44" &
(44& '
)44' (
;44( )
if66 

(66 
billableTime66 
<=66 
firstHourTime66 )
)66) *
{77 	
var99 
fraction99 
=99 
(99 
decimal99 #
)99# $
billableTime99$ 0
.990 1

TotalHours991 ;
;99; <
totalCharge:: 
=:: 
new:: 
Money:: #
(::# $
	tableType::$ -
.::- .
FirstHourRate::. ;
.::; <
Value::< A
*::B C
fraction::D L
)::L M
;::M N
};; 	
else<< 
{== 	
totalCharge?? 
=?? 
new?? 
Money?? #
(??# $
	tableType??$ -
.??- .
FirstHourRate??. ;
.??; <
Value??< A
)??A B
;??B C
var@@ 
remainingTime@@ 
=@@ 
billableTime@@  ,
-@@- .
firstHourTime@@/ <
;@@< =
varAA 
remainingChargeAA 
=AA  !*
CalculateTimeChargeForDurationAA" @
(AA@ A
remainingTimeAAA N
,AAN O
	tableTypeAAP Y
.AAY Z

HourlyRateAAZ d
)AAd e
;AAe f
totalChargeBB 
+=BB 
remainingChargeBB *
;BB* +
}CC 	
returnFF 
awaitFF #
ApplyMinimumChargeAsyncFF ,
(FF, -
totalChargeFF- 8
,FF8 9
	tableTypeFF: C
)FFC D
;FFD E
}GG 
publicQQ 

asyncQQ 
TaskQQ 
<QQ 
TimeSpanQQ 
>QQ "
ApplyTimeRoundingAsyncQQ  6
(QQ6 7
TimeSpanQQ7 ?
durationQQ@ H
,QQH I
TimeRoundingRuleQQJ Z
ruleQQ[ _
)QQ_ `
{RR 
ifSS 

(SS 
durationSS 
<SS 
TimeSpanSS 
.SS  
ZeroSS  $
)SS$ %
{TT 	
throwUU 
newUU 
ArgumentExceptionUU '
(UU' (
$strUU( F
,UUF G
nameofUUH N
(UUN O
durationUUO W
)UUW X
)UUX Y
;UUY Z
}VV 	
ifXX 

(XX 
durationXX 
==XX 
TimeSpanXX  
.XX  !
ZeroXX! %
)XX% &
{YY 	
returnZZ 
TimeSpanZZ 
.ZZ 
ZeroZZ  
;ZZ  !
}[[ 	
var]] 
totalMinutes]] 
=]] 
(]] 
int]] 
)]]  
Math]]  $
.]]$ %
Ceiling]]% ,
(]], -
duration]]- 5
.]]5 6
TotalMinutes]]6 B
)]]B C
;]]C D
var__ 
roundedMinutes__ 
=__ 
rule__ !
switch__" (
{`` 	
TimeRoundingRuleaa 
.aa 
Noneaa !
=>aa" $
totalMinutesaa% 1
,aa1 2
TimeRoundingRulebb 
.bb 
FifteenMinutesbb +
=>bb, .
RoundUpToIncrementbb/ A
(bbA B
totalMinutesbbB N
,bbN O
$numbbP R
)bbR S
,bbS T
TimeRoundingRulecc 
.cc 
ThirtyMinutescc *
=>cc+ -
RoundUpToIncrementcc. @
(cc@ A
totalMinutesccA M
,ccM N
$numccO Q
)ccQ R
,ccR S
TimeRoundingRuledd 
.dd 
SixtyMinutesdd )
=>dd* ,
RoundUpToIncrementdd- ?
(dd? @
totalMinutesdd@ L
,ddL M
$numddN P
)ddP Q
,ddQ R
_ee 
=>ee 
totalMinutesee 
}ff 	
;ff	 

returnhh 
TimeSpanhh 
.hh 
FromMinuteshh #
(hh# $
roundedMinuteshh$ 2
)hh2 3
;hh3 4
}ii 
publicss 

asyncss 
Taskss 
<ss 
Moneyss 
>ss #
ApplyMinimumChargeAsyncss 4
(ss4 5
Moneyss5 :
calculatedChargess; K
,ssK L
	TableTypessM V
	tableTypessW `
)ss` a
{tt 
ifuu 

(uu 
calculatedChargeuu 
==uu 
nulluu  $
)uu$ %
{vv 	
throwww 
newww !
ArgumentNullExceptionww +
(ww+ ,
nameofww, 2
(ww2 3
calculatedChargeww3 C
)wwC D
)wwD E
;wwE F
}xx 	
ifzz 

(zz 
	tableTypezz 
==zz 
nullzz 
)zz 
{{{ 	
throw|| 
new|| !
ArgumentNullException|| +
(||+ ,
nameof||, 2
(||2 3
	tableType||3 <
)||< =
)||= >
;||> ?
}}} 	
if
ÄÄ 

(
ÄÄ 
	tableType
ÄÄ 
.
ÄÄ 
MinimumCharge
ÄÄ #
==
ÄÄ$ &
null
ÄÄ' +
||
ÄÄ, .
	tableType
ÅÅ 
.
ÅÅ 
MinimumCharge
ÅÅ #
.
ÅÅ# $
Amount
ÅÅ$ *
<=
ÅÅ+ -
$num
ÅÅ. /
||
ÅÅ0 2
calculatedCharge
ÇÇ 
>=
ÇÇ 
	tableType
ÇÇ  )
.
ÇÇ) *
MinimumCharge
ÇÇ* 7
)
ÇÇ7 8
{
ÉÉ 	
return
ÑÑ 
calculatedCharge
ÑÑ #
;
ÑÑ# $
}
ÖÖ 	
return
àà 
	tableType
àà 
.
àà 
MinimumCharge
àà &
;
àà& '
}
ââ 
public
íí 

async
íí 
Task
íí 
<
íí %
PricingSimulationResult
íí -
>
íí- ."
SimulatePricingAsync
íí/ C
(
ííC D
PricingScenario
ííD S
scenario
ííT \
)
íí\ ]
{
ìì 
if
îî 

(
îî 
scenario
îî 
==
îî 
null
îî 
)
îî 
{
ïï 	
throw
ññ 
new
ññ #
ArgumentNullException
ññ +
(
ññ+ ,
nameof
ññ, 2
(
ññ2 3
scenario
ññ3 ;
)
ññ; <
)
ññ< =
;
ññ= >
}
óó 	
var
ôô 
appliedRules
ôô 
=
ôô 
new
ôô 
List
ôô #
<
ôô# $
string
ôô$ *
>
ôô* +
(
ôô+ ,
)
ôô, -
;
ôô- .
var
öö 
originalDuration
öö 
=
öö 
scenario
öö '
.
öö' (
Duration
öö( 0
;
öö0 1
var
ùù 
roundedDuration
ùù 
=
ùù 
await
ùù #$
ApplyTimeRoundingAsync
ùù$ :
(
ùù: ;
scenario
ùù; C
.
ùùC D
Duration
ùùD L
,
ùùL M
scenario
ùùN V
.
ùùV W
	TableType
ùùW `
.
ùù` a
RoundingRule
ùùa m
)
ùùm n
;
ùùn o
if
ûû 

(
ûû 
roundedDuration
ûû 
!=
ûû 
originalDuration
ûû /
)
ûû/ 0
{
üü 	
appliedRules
†† 
.
†† 
Add
†† 
(
†† 
$"
†† 
$str
†† 1
{
††1 2
originalDuration
††2 B
.
††B C
TotalMinutes
††C O
:
††O P
$str
††P R
}
††R S
$str
††S W
{
††W X
roundedDuration
††X g
.
††g h
TotalMinutes
††h t
:
††t u
$str
††u w
}
††w x
$str††x Ç
{††Ç É
scenario††É ã
.††ã å
	TableType††å ï
.††ï ñ
RoundingRule††ñ ¢
}††¢ £
$str††£ §
"††§ •
)††• ¶
;††¶ ß
}
°° 	
var
§§ 

baseCharge
§§ 
=
§§ ,
CalculateTimeChargeForDuration
§§ 7
(
§§7 8
roundedDuration
§§8 G
,
§§G H
scenario
§§I Q
.
§§Q R
	TableType
§§R [
.
§§[ \

HourlyRate
§§\ f
)
§§f g
;
§§g h
Money
ßß 
firstHourCharge
ßß 
=
ßß 
Money
ßß  %
.
ßß% &
Zero
ßß& *
(
ßß* +
)
ßß+ ,
;
ßß, -
Money
®® "
remainingHoursCharge
®® "
=
®®# $
Money
®®% *
.
®®* +
Zero
®®+ /
(
®®/ 0
)
®®0 1
;
®®1 2
if
™™ 

(
™™ 
scenario
™™ 
.
™™ 
	TableType
™™ 
.
™™ 
FirstHourRate
™™ ,
.
™™, -
HasValue
™™- 5
&&
™™6 8
roundedDuration
™™9 H
.
™™H I

TotalHours
™™I S
>=
™™T V
$num
™™W X
)
™™X Y
{
´´ 	
var
¨¨ 
firstHourTime
¨¨ 
=
¨¨ 
TimeSpan
¨¨  (
.
¨¨( )
	FromHours
¨¨) 2
(
¨¨2 3
$num
¨¨3 4
)
¨¨4 5
;
¨¨5 6
if
≠≠ 
(
≠≠ 
roundedDuration
≠≠ 
<=
≠≠  "
firstHourTime
≠≠# 0
)
≠≠0 1
{
ÆÆ 
var
∞∞ 
fraction
∞∞ 
=
∞∞ 
(
∞∞  
decimal
∞∞  '
)
∞∞' (
roundedDuration
∞∞( 7
.
∞∞7 8

TotalHours
∞∞8 B
;
∞∞B C
firstHourCharge
±± 
=
±±  !
new
±±" %
Money
±±& +
(
±±+ ,
scenario
±±, 4
.
±±4 5
	TableType
±±5 >
.
±±> ?
FirstHourRate
±±? L
.
±±L M
Value
±±M R
*
±±S T
fraction
±±U ]
)
±±] ^
;
±±^ _
appliedRules
≤≤ 
.
≤≤ 
Add
≤≤  
(
≤≤  !
$"
≤≤! #
$str
≤≤# G
{
≤≤G H
fraction
≤≤H P
:
≤≤P Q
$str
≤≤Q S
}
≤≤S T
$str
≤≤T Y
{
≤≤Y Z
scenario
≤≤Z b
.
≤≤b c
	TableType
≤≤c l
.
≤≤l m
FirstHourRate
≤≤m z
.
≤≤z {
Value≤≤{ Ä
:≤≤Ä Å
$str≤≤Å É
}≤≤É Ñ
"≤≤Ñ Ö
)≤≤Ö Ü
;≤≤Ü á
}
≥≥ 
else
¥¥ 
{
µµ 
firstHourCharge
∑∑ 
=
∑∑  !
new
∑∑" %
Money
∑∑& +
(
∑∑+ ,
scenario
∑∑, 4
.
∑∑4 5
	TableType
∑∑5 >
.
∑∑> ?
FirstHourRate
∑∑? L
.
∑∑L M
Value
∑∑M R
)
∑∑R S
;
∑∑S T
var
∏∏ 
remainingTime
∏∏ !
=
∏∏" #
roundedDuration
∏∏$ 3
-
∏∏4 5
firstHourTime
∏∏6 C
;
∏∏C D"
remainingHoursCharge
ππ $
=
ππ% &,
CalculateTimeChargeForDuration
ππ' E
(
ππE F
remainingTime
ππF S
,
ππS T
scenario
ππU ]
.
ππ] ^
	TableType
ππ^ g
.
ππg h

HourlyRate
ππh r
)
ππr s
;
ππs t
appliedRules
∫∫ 
.
∫∫ 
Add
∫∫  
(
∫∫  !
$"
∫∫! #
$str
∫∫# =
{
∫∫= >
scenario
∫∫> F
.
∫∫F G
	TableType
∫∫G P
.
∫∫P Q
FirstHourRate
∫∫Q ^
.
∫∫^ _
Value
∫∫_ d
:
∫∫d e
$str
∫∫e g
}
∫∫g h
"
∫∫h i
)
∫∫i j
;
∫∫j k
appliedRules
ªª 
.
ªª 
Add
ªª  
(
ªª  !
$"
ªª! #
$str
ªª# -
{
ªª- .
remainingTime
ªª. ;
.
ªª; <

TotalHours
ªª< F
:
ªªF G
$str
ªªG I
}
ªªI J
$str
ªªJ d
{
ªªd e
scenario
ªªe m
.
ªªm n
	TableType
ªªn w
.
ªªw x

HourlyRateªªx Ç
:ªªÇ É
$strªªÉ Ö
}ªªÖ Ü
$strªªÜ ã
"ªªã å
)ªªå ç
;ªªç é
}
ºº 
}
ΩΩ 	
var
¿¿ 
calculatedCharge
¿¿ 
=
¿¿ 
scenario
¿¿ '
.
¿¿' (
	TableType
¿¿( 1
.
¿¿1 2
FirstHourRate
¿¿2 ?
.
¿¿? @
HasValue
¿¿@ H
&&
¿¿I K
roundedDuration
¿¿L [
.
¿¿[ \

TotalHours
¿¿\ f
>=
¿¿g i
$num
¿¿j k
?
¡¡ 
firstHourCharge
¡¡ 
+
¡¡ "
remainingHoursCharge
¡¡  4
:
¬¬ 

baseCharge
¬¬ 
;
¬¬ 
var
≈≈ 
finalCharge
≈≈ 
=
≈≈ 
await
≈≈ %
ApplyMinimumChargeAsync
≈≈  7
(
≈≈7 8
calculatedCharge
≈≈8 H
,
≈≈H I
scenario
≈≈J R
.
≈≈R S
	TableType
≈≈S \
)
≈≈\ ]
;
≈≈] ^
if
∆∆ 

(
∆∆ 
finalCharge
∆∆ 
>
∆∆ 
calculatedCharge
∆∆ *
)
∆∆* +
{
«« 	
appliedRules
»» 
.
»» 
Add
»» 
(
»» 
$"
»» 
$str
»» 8
{
»»8 9
scenario
»»9 A
.
»»A B
	TableType
»»B K
.
»»K L
MinimumCharge
»»L Y
.
»»Y Z
Amount
»»Z `
:
»»` a
$str
»»a c
}
»»c d
"
»»d e
)
»»e f
;
»»f g
}
…… 	
if
ÃÃ 

(
ÃÃ 
scenario
ÃÃ 
.
ÃÃ 
HasMemberDiscount
ÃÃ &
)
ÃÃ& '
{
ÕÕ 	
appliedRules
ŒŒ 
.
ŒŒ 
Add
ŒŒ 
(
ŒŒ 
$str
ŒŒ U
)
ŒŒU V
;
ŒŒV W
}
œœ 	
return
—— %
PricingSimulationResult
—— &
.
——& '
CreateDetailed
——' 5
(
——5 6

baseCharge
““ 
:
““ 

baseCharge
““ "
,
““" #
firstHourCharge
”” 
:
”” 
firstHourCharge
”” ,
,
””, -"
remainingHoursCharge
‘‘  
:
‘‘  !"
remainingHoursCharge
‘‘" 6
,
‘‘6 7"
minimumChargeApplied
’’  
:
’’  !
finalCharge
’’" -
>
’’. /
calculatedCharge
’’0 @
?
’’A B
scenario
’’C K
.
’’K L
	TableType
’’L U
.
’’U V
MinimumCharge
’’V c
:
’’d e
Money
’’f k
.
’’k l
Zero
’’l p
(
’’p q
)
’’q r
,
’’r s
finalCharge
÷÷ 
:
÷÷ 
finalCharge
÷÷ $
,
÷÷$ %
roundedDuration
◊◊ 
:
◊◊ 
roundedDuration
◊◊ ,
,
◊◊, -
appliedRules
ÿÿ 
:
ÿÿ 
appliedRules
ÿÿ &
.
ÿÿ& '

AsReadOnly
ÿÿ' 1
(
ÿÿ1 2
)
ÿÿ2 3
)
ŸŸ 	
;
ŸŸ	 

}
⁄⁄ 
public
„„ 

async
„„ 
Task
„„ 
<
„„ 
bool
„„ 
>
„„ '
ValidatePricingRulesAsync
„„ 5
(
„„5 6
	TableType
„„6 ?
	tableType
„„@ I
)
„„I J
{
‰‰ 
if
ÂÂ 

(
ÂÂ 
	tableType
ÂÂ 
==
ÂÂ 
null
ÂÂ 
)
ÂÂ 
{
ÊÊ 	
throw
ÁÁ 
new
ÁÁ #
ArgumentNullException
ÁÁ +
(
ÁÁ+ ,
nameof
ÁÁ, 2
(
ÁÁ2 3
	tableType
ÁÁ3 <
)
ÁÁ< =
)
ÁÁ= >
;
ÁÁ> ?
}
ËË 	
if
ÎÎ 

(
ÎÎ 
!
ÎÎ 
	tableType
ÎÎ 
.
ÎÎ *
ValidatePricingConfiguration
ÎÎ 3
(
ÎÎ3 4
)
ÎÎ4 5
)
ÎÎ5 6
{
ÏÏ 	
return
ÌÌ 
false
ÌÌ 
;
ÌÌ 
}
ÓÓ 	
if
ÛÛ 

(
ÛÛ 
	tableType
ÛÛ 
.
ÛÛ 
FirstHourRate
ÛÛ #
.
ÛÛ# $
HasValue
ÛÛ$ ,
)
ÛÛ, -
{
ÙÙ 	
if
ˆˆ 
(
ˆˆ 
	tableType
ˆˆ 
.
ˆˆ 
FirstHourRate
ˆˆ '
.
ˆˆ' (
Value
ˆˆ( -
>
ˆˆ. /
	tableType
ˆˆ0 9
.
ˆˆ9 :

HourlyRate
ˆˆ: D
*
ˆˆE F
$num
ˆˆG H
)
ˆˆH I
{
˜˜ 
return
¯¯ 
false
¯¯ 
;
¯¯ 
}
˘˘ 
if
¸¸ 
(
¸¸ 
	tableType
¸¸ 
.
¸¸ 
FirstHourRate
¸¸ '
.
¸¸' (
Value
¸¸( -
<
¸¸. /
	tableType
¸¸0 9
.
¸¸9 :

HourlyRate
¸¸: D
*
¸¸E F
$num
¸¸G K
)
¸¸K L
{
˝˝ 
return
˛˛ 
false
˛˛ 
;
˛˛ 
}
ˇˇ 
}
ÄÄ 	
if
ÉÉ 

(
ÉÉ 
	tableType
ÉÉ 
.
ÉÉ 
MinimumCharge
ÉÉ #
!=
ÉÉ$ &
null
ÉÉ' +
&&
ÉÉ, .
	tableType
ÉÉ/ 8
.
ÉÉ8 9
MinimumCharge
ÉÉ9 F
.
ÉÉF G
Amount
ÉÉG M
>
ÉÉN O
$num
ÉÉP Q
)
ÉÉQ R
{
ÑÑ 	
var
ÜÜ 
twoHoursCharge
ÜÜ 
=
ÜÜ  
	tableType
ÜÜ! *
.
ÜÜ* +

HourlyRate
ÜÜ+ 5
*
ÜÜ6 7
$num
ÜÜ8 9
;
ÜÜ9 :
if
áá 
(
áá 
	tableType
áá 
.
áá 
MinimumCharge
áá '
.
áá' (
Amount
áá( .
>
áá/ 0
twoHoursCharge
áá1 ?
)
áá? @
{
àà 
return
ââ 
false
ââ 
;
ââ 
}
ää 
}
ãã 	
if
éé 

(
éé 
	tableType
éé 
.
éé 
MinimumMinutes
éé $
>
éé% &
$num
éé' (
&&
éé) +
	tableType
éé, 5
.
éé5 6
RoundingMinutes
éé6 E
>
ééF G
$num
ééH I
)
ééI J
{
èè 	
if
ëë 
(
ëë 
	tableType
ëë 
.
ëë 
MinimumMinutes
ëë (
%
ëë) *
	tableType
ëë+ 4
.
ëë4 5
RoundingMinutes
ëë5 D
!=
ëëE G
$num
ëëH I
)
ëëI J
{
íí 
return
ìì 
false
ìì 
;
ìì 
}
îî 
}
ïï 	
return
óó 
true
óó 
;
óó 
}
òò 
private
¢¢ 
Money
¢¢ ,
CalculateTimeChargeForDuration
¢¢ 0
(
¢¢0 1
TimeSpan
¢¢1 9
duration
¢¢: B
,
¢¢B C
decimal
¢¢D K

hourlyRate
¢¢L V
)
¢¢V W
{
££ 
if
§§ 

(
§§ 
duration
§§ 
<=
§§ 
TimeSpan
§§  
.
§§  !
Zero
§§! %
)
§§% &
{
•• 	
return
¶¶ 
Money
¶¶ 
.
¶¶ 
Zero
¶¶ 
(
¶¶ 
)
¶¶ 
;
¶¶  
}
ßß 	
var
©© 
hours
©© 
=
©© 
(
©© 
decimal
©© 
)
©© 
duration
©© %
.
©©% &

TotalHours
©©& 0
;
©©0 1
return
™™ 
new
™™ 
Money
™™ 
(
™™ 
hours
™™ 
*
™™  

hourlyRate
™™! +
)
™™+ ,
;
™™, -
}
´´ 
private
≥≥ 
int
≥≥  
RoundUpToIncrement
≥≥ "
(
≥≥" #
int
≥≥# &
minutes
≥≥' .
,
≥≥. /
int
≥≥0 3
	increment
≥≥4 =
)
≥≥= >
{
¥¥ 
if
µµ 

(
µµ 
	increment
µµ 
<=
µµ 
$num
µµ 
)
µµ 
return
∂∂ 
minutes
∂∂ 
;
∂∂ 
var
∏∏ 
	intervals
∏∏ 
=
∏∏ 
(
∏∏ 
int
∏∏ 
)
∏∏ 
Math
∏∏ !
.
∏∏! "
Ceiling
∏∏" )
(
∏∏) *
(
∏∏* +
double
∏∏+ 1
)
∏∏1 2
minutes
∏∏2 9
/
∏∏: ;
	increment
∏∏< E
)
∏∏E F
;
∏∏F G
return
ππ 
	intervals
ππ 
*
ππ 
	increment
ππ $
;
ππ$ %
}
∫∫ 
}ΩΩ †
C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Interfaces\Printing\IPrintLayoutEngine.cs
	namespace 	
Magidesk
 
. 
Domain 
. 

Interfaces $
.$ %
Printing% -
{ 
public 

	interface 
IPrintLayoutEngine '
{ 
Task 
< 
string 
> %
GenerateTicketLayoutAsync .
(. /
object/ 5

ticketData6 @
,@ A
PrinterFormatB O
formatP V
)V W
;W X
}		 
}

  
uC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Exceptions\NotFoundException.cs
	namespace 	
Magidesk
 
. 
Domain 
. 

Exceptions $
;$ %
public 
class 
NotFoundException 
:  
DomainException! 0
{		 
public

 

NotFoundException

 
(

 
string

 #
message

$ +
)

+ ,
:

- .
base

/ 3
(

3 4
message

4 ;
)

; <
{ 
} 
public 

NotFoundException 
( 
string #
message$ +
,+ ,
	Exception- 6
innerException7 E
)E F
:G H
baseI M
(M N
messageN U
,U V
innerExceptionW e
)e f
{ 
} 
} ˛
}C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Exceptions\InvalidOperationException.cs
	namespace 	
Magidesk
 
. 
Domain 
. 

Exceptions $
;$ %
public 
sealed 
class %
InvalidOperationException -
:. /
DomainException0 ?
{		 
public

 
%
InvalidOperationException

 $
(

$ %
string

% +
message

, 3
)

3 4
:

5 6
base

7 ;
(

; <
message

< C
)

C D
{ 
} 
public 
%
InvalidOperationException $
($ %
string% +
message, 3
,3 4
	Exception5 >
innerException? M
)M N
:O P
baseQ U
(U V
messageV ]
,] ^
innerException_ m
)m n
{ 
} 
} ÿ
sC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Exceptions\DomainException.cs
	namespace 	
Magidesk
 
. 
Domain 
. 

Exceptions $
;$ %
public 
abstract 
class 
DomainException %
:& '
	Exception( 1
{		 
	protected

 
DomainException

 
(

 
string

 $
message

% ,
)

, -
:

. /
base

0 4
(

4 5
message

5 <
)

< =
{ 
} 
	protected 
DomainException 
( 
string $
message% ,
,, -
	Exception. 7
innerException8 F
)F G
:H I
baseJ N
(N O
messageO V
,V W
innerExceptionX f
)f g
{ 
} 
} ì
ÇC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Exceptions\BusinessRuleViolationException.cs
	namespace 	
Magidesk
 
. 
Domain 
. 

Exceptions $
;$ %
public 
sealed 
class *
BusinessRuleViolationException 2
:3 4
DomainException5 D
{ 
public 
*
BusinessRuleViolationException )
() *
string* 0
message1 8
)8 9
:: ;
base< @
(@ A
messageA H
)H I
{		 
}

 
public 
*
BusinessRuleViolationException )
() *
string* 0
message1 8
,8 9
	Exception: C
innerExceptionD R
)R S
:T U
baseV Z
(Z [
message[ b
,b c
innerExceptiond r
)r s
{ 
} 
} °
oC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Events\TicketHeldEvent.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Events  
{ 
public

 

sealed

 
class

 

TicketHeld

 "
:

# $
DomainEventBase

% 4
{ 
public 
Guid 
TicketId 
{ 
get "
;" #
}$ %
public 
string 
Reason 
{ 
get "
;" #
}$ %
public 
UserId 
HeldBy 
{ 
get "
;" #
}$ %
public 

TicketHeld 
( 
Guid 
ticketId '
,' (
string) /
reason0 6
,6 7
UserId8 >
heldBy? E
,E F
GuidG K
?K L
correlationIdM Z
=[ \
null] a
)a b
: 
base 
( 
correlationId  
)  !
{ 	
TicketId 
= 
ticketId 
;  
Reason 
= 
reason 
; 
HeldBy 
= 
heldBy 
; 
} 	
} 
} ∑
pC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Events\GroupSettleEvent.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Events  
{ 
public 

sealed 
class 
GroupSettled $
:% &
DomainEventBase' 6
{ 
public 
List 
< 
Guid 
> 
	TicketIds #
{$ %
get& )
;) *
}+ ,
public 
Money 
TotalAmount  
{! "
get# &
;& '
}( )
public 
PaymentType 
PaymentType &
{' (
get) ,
;, -
}. /
public 
UserId 
ProcessedBy !
{" #
get$ '
;' (
}) *
public 
List 
< 
Guid 
> 

PaymentIds $
{% &
get' *
;* +
}, -
public 
GroupSettled 
( 
List  
<  !
Guid! %
>% &
	ticketIds' 0
,0 1
Money2 7
totalAmount8 C
,C D
PaymentTypeE P
paymentTypeQ \
,\ ]
UserId^ d
processedBye p
,p q
Listr v
<v w
Guidw {
>{ |

paymentIds	} á
,
á à
Guid
â ç
?
ç é
correlationId
è ú
=
ù û
null
ü £
)
£ §
: 
base 
( 
correlationId  
)  !
{ 	
	TicketIds 
= 
	ticketIds !
;! "
TotalAmount 
= 
totalAmount %
;% &
PaymentType 
= 
paymentType %
;% &
ProcessedBy 
= 
processedBy %
;% &

PaymentIds 
= 

paymentIds #
;# $
} 	
} 
} œ
tC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Events\DiscountRemovedEvent.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Events  
{ 
public

 

sealed

 
class

 
DiscountRemoved

 '
:

( )
DomainEventBase

* 9
{ 
public 
Guid 
TicketId 
{ 
get "
;" #
}$ %
public 
Guid 

DiscountId 
{  
get! $
;$ %
}& '
public 
UserId 
	RemovedBy 
{  !
get" %
;% &
}' (
public 
DateTime 
	RemovedAt !
{" #
get$ '
;' (
}) *
public 
DiscountRemoved 
( 
Guid 
ticketId 
, 
Guid 

discountId 
, 
UserId 
	removedBy 
, 
DateTime 
	removedAt 
, 
Guid 
? 
correlationId 
=  !
null" &
)& '
: 
base 
( 
correlationId  
)  !
{ 	
TicketId 
= 
ticketId 
;  

DiscountId 
= 

discountId #
;# $
	RemovedBy 
= 
	removedBy !
;! "
	RemovedAt 
= 
	removedAt !
;! "
} 	
} 
} Í
xC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Exceptions\ConcurrencyException.cs
	namespace 	
Magidesk
 
. 
Domain 
. 

Exceptions $
;$ %
public 
sealed 
class  
ConcurrencyException (
:) *
DomainException+ :
{ 
public 
 
ConcurrencyException 
(  
string  &
message' .
). /
:0 1
base2 6
(6 7
message7 >
)> ?
{		 
}

 
public 
 
ConcurrencyException 
(  
string  &
message' .
,. /
	Exception0 9
innerException: H
)H I
:J K
baseL P
(P Q
messageQ X
,X Y
innerExceptionZ h
)h i
{ 
} 
} û
vC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Events\TicketSplitBySeatEvent.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Events  
{ 
public 

sealed 
class "
TicketSplitBySeatEvent .
:/ 0
DomainEventBase1 @
{ 
public 
Guid 
OriginalTicketId $
{% &
get' *
;* +
}, -
public 
List 
< 
Guid 
> 
NewTicketIds &
{' (
get) ,
;, -
}. /
public 
int 

SeatsCount 
{ 
get  #
;# $
}% &
public 

Dictionary 
< 
int 
, 
int "
>" #
ItemsPerSeat$ 0
{1 2
get3 6
;6 7
}8 9
public 
UserId 
ProcessedBy !
{" #
get$ '
;' (
}) *
public "
TicketSplitBySeatEvent %
(% &
Guid 
originalTicketId !
,! "
List 
< 
Guid 
> 
newTicketIds #
,# $
int 

seatsCount 
, 

Dictionary 
< 
int 
, 
int 
>  
itemsPerSeat! -
,- .
UserId 
processedBy 
, 
Guid 
? 
correlationId 
=  !
null" &
)& '
: 
base 
( 
correlationId  
)  !
{ 	
OriginalTicketId 
= 
originalTicketId /
;/ 0
NewTicketIds 
= 
newTicketIds '
;' (

SeatsCount 
= 

seatsCount #
;# $
ItemsPerSeat 
= 
itemsPerSeat '
;' (
ProcessedBy   
=   
processedBy   %
;  % &
}!! 	
}"" 
}## ø
sC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Events\TicketReleasedEvent.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Events  
{ 
public		 

sealed		 
class		 
TicketReleased		 &
:		' (
DomainEventBase		) 8
{

 
public 
Guid 
TicketId 
{ 
get "
;" #
}$ %
public 
TicketReleased 
( 
Guid "
ticketId# +
,+ ,
Guid- 1
?1 2
correlationId3 @
=A B
nullC G
)G H
: 
base 
( 
correlationId  
)  !
{ 	
TicketId 
= 
ticketId 
;  
} 	
} 
} ·
uC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Events\PaymentProcessedEvent.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Events  
{ 
public 

sealed 
class 
PaymentProcessed (
:) *
DomainEventBase+ :
{ 
public 
Guid 
	PaymentId 
{ 
get  #
;# $
}% &
public 
Guid 
TicketId 
{ 
get "
;" #
}$ %
public 
Money 
Amount 
{ 
get !
;! "
}# $
public 
PaymentType 
PaymentType &
{' (
get) ,
;, -
}. /
public 
UserId 
ProcessedBy !
{" #
get$ '
;' (
}) *
public 
PaymentProcessed 
(  
Guid  $
	paymentId% .
,. /
Guid0 4
ticketId5 =
,= >
Money? D
amountE K
,K L
PaymentTypeM X
paymentTypeY d
,d e
UserIdf l
processedBym x
,x y
Guidz ~
?~ 
correlationId
Ä ç
=
é è
null
ê î
)
î ï
: 
base 
( 
correlationId  
)  !
{ 	
	PaymentId 
= 
	paymentId !
;! "
TicketId 
= 
ticketId 
;  
Amount 
= 
amount 
; 
PaymentType 
= 
paymentType %
;% &
ProcessedBy 
= 
processedBy %
;% &
} 	
} 
} Ò
tC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Events\DiscountAppliedEvent.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Events  
{ 
public

 

sealed

 
class

 
DiscountApplied

 '
:

( )
DomainEventBase

* 9
{ 
public 
Guid 
TicketId 
{ 
get "
;" #
}$ %
public 
Guid 

DiscountId 
{  
get! $
;$ %
}& '
public 
Money 
Amount 
{ 
get !
;! "
}# $
public 
UserId 
	AppliedBy 
{  !
get" %
;% &
}' (
public 
UserId 
? 
AuthorizedBy #
{$ %
get& )
;) *
}+ ,
public 
DateTime 
	AppliedAt !
{" #
get$ '
;' (
}) *
public 
DiscountApplied 
( 
Guid 
ticketId 
, 
Guid 

discountId 
, 
Money 
amount 
, 
UserId 
	appliedBy 
, 
UserId 
? 
authorizedBy  
,  !
DateTime 
	appliedAt 
, 
Guid 
? 
correlationId 
=  !
null" &
)& '
: 
base 
( 
correlationId  
)  !
{ 	
TicketId 
= 
ticketId 
;  

DiscountId 
= 

discountId #
;# $
Amount 
= 
amount 
; 
	AppliedBy   
=   
	appliedBy   !
;  ! "
AuthorizedBy!! 
=!! 
authorizedBy!! '
;!!' (
	AppliedAt"" 
="" 
	appliedAt"" !
;""! "
}## 	
}$$ 
}%% Ã
mC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enums\TableShapeType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
TableShapeType 
{ 
	Rectangle 
, 
Square 

,
 
Round 	
,	 

Oval 
} í
tC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\UserPermission.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
[ 
Flags 
] 
public 
enum 
UserPermission 
{ 
None 
=	 

$num 
, 
CreateTicket 
= 
$num 
<< 
$num 
, 

EditTicket 
= 
$num 
<< 
$num 
, 
TakePayment 
= 
$num 
<< 
$num 
, 

VoidTicket 
= 
$num 
<< 
$num 
, 
RefundPayment 
= 
$num 
<< 
$num 
, 

OpenDrawer 
= 
$num 
<< 
$num 
, 

CloseBatch 
= 
$num 
<< 
$num 
, 
ApplyDiscount 
= 
$num 
<< 
$num 
, 
ManageUsers 
= 
$num 
<< 
$num 
, 
ManageTableLayout 
= 
$num 
<< 
$num 
, 

ManageMenu 
= 
$num 
<< 
$num 
, 
ViewReports 
= 
$num 
<< 
$num 
, 
SystemConfiguration 
= 
$num 
<< 
$num !
,! "
AdjustSessionTime 
= 
$num 
<< 
$num 
,  
RefundTicket"" 
="" 
$num"" 
<<"" 
$num"" 
}%% é
uC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\TransactionType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
TransactionType 
{ 
Credit 

,
 
Debit 	
} Í
vC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\TimeRoundingRule.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
TimeRoundingRule 
{ 
None 
, 	
FifteenMinutes 
, 
ThirtyMinutes 
, 
SixtyMinutes 
} ·
rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\TicketStatus.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
TicketStatus 
{ 
Draft 	
=
 
$num 
, 
Open 
=	 

$num 
, 
Held 
=	 

$num 
, 
Paid 
=	 

$num 
, 
Closed   

=   
$num   
,   
Voided%% 

=%% 
$num%% 
,%% 
Refunded** 
=** 
$num** 
,** 
	Scheduled// 
=// 
$num// 
}00 Ã
}C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\TerminalTransactionType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum #
TerminalTransactionType #
{ 
Sale 
, 	
Refund 

,
 
Drop 
, 	
Payout 

,
 
Bleed		 	
,			 

	OpenFloat

 
,

 
NoSale 

} ë
rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\TemplateType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
TemplateType 
{ 
Receipt 
= 
$num 
, 
Kitchen 
= 
$num 
, 
Report 

= 
$num 
} ñ
qC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\TableStatus.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
TableStatus 
{ 
	Available 
= 
$num 
, 
Seat 
=	 

$num 
, 
Booked 

= 
$num 
, 
Dirty 	
=
 
$num 
, 
Disable 
= 
$num 
}   ∑
xC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\TableSessionStatus.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
TableSessionStatus 
{ 
Active 

,
 
Paused 

,
 
Ended 	
} •
wC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\StockMovementType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
StockMovementType 
{ 
Sale 
=	 

$num 
, 

Adjustment 
= 
$num 
, 
	Receiving 
= 
$num 
, 
Return 

= 
$num 
, 
Waste		 	
=		
 
$num		 
}

 “
wC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\QualificationType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
QualificationType 
{ 
Item 
=	 

$num 
, 
Order		 	
=		
 
$num		 
}

 ê
yC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\PurchaseOrderStatus.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
PurchaseOrderStatus 
{ 
Draft 	
,	 

Ordered 
, 
Received 
, 
PartiallyReceived 
, 
	Cancelled		 
}

 ï
qC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\PrinterType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
PrinterType 
{ 
Report 

= 
$num 
, 
Receipt 
= 
$num 
, 
Kitchen		 
=		 
$num		 
,		 
Packing

 
=

 
$num

 
,

 
Kds 
= 	
$num
 
} °
sC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\PrinterFormat.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
PrinterFormat 
{ 
Thermal80mm 
= 
$num 
, 
Thermal58mm 
= 
$num 
, 
StandardPage 
= 
$num 
} Ê
sC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\PriceStrategy.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
PriceStrategy 
{ 
SumOfHalves 
, 
AverageOfHalves 
, 
HighestHalf 
, 
WholePie 
}		 Ë
qC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\PaymentType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
PaymentType 
{ 
Cash 
=	 

$num 
, 

CreditCard		 
=		 
$num		 
,		 

CreditVisa

 
=

 
$num

 
,

 
CreditMasterCard 
= 
$num 
, 

CreditAmex 
= 
$num 
, 
CreditDiscover 
= 
$num 
, 
	DebitCard 
= 
$num 
, 
	DebitVisa 
= 
$num 
, 
DebitMasterCard 
= 
$num 
, 
GiftCertificate 
= 
$num 
, 
CustomPayment 
= 
$num 
} Ä
xC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\PaymentBatchStatus.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
PaymentBatchStatus 
{ 
Open 
, 	
Closed 

,
 
	Submitted 
, 
Settled 
, 
Failed		 

}

 û
rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\OverrideType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
OverrideType 
{ 
TimeAdjustment 
, 
PricingOverride 
, 
ForceEndSession 
, 
GuestCountOverride 
, 
RateOverride 
}   ”
rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\ModifierType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
ModifierType 
{ 
Normal 

= 
$num 
, 
Extra		 	
=		
 
$num		 
,		 
InfoOnly

 
=

 
$num

 
,

 
AddOn 	
=
 
$num 
}  
uC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\ModifierPortion.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
ModifierPortion 
{ 
Whole 	
,	 

LeftHalf 
, 
	RightHalf 
, 
Quarter1 
, 
Quarter2		 
,		 
Quarter3

 
,

 
Quarter4 
} À
sC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\KitchenStatus.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
KitchenStatus 
{ 
New 
, 
Cooking 
, 
Done 
, 	
Void 
}		 ≥
nC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\GameType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
GameType 
{ 
	EightBall 
, 
NineBall 
, 
StraightPool 
, 
Snooker 
, 
ThreeCushion 
, 
	OnePocket$$ 
,$$ 
BankPool)) 
,)) 
Rotation.. 
,.. 
Practice33 
,33 
Other88 	
}99 ‚
sC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\EquipmentType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
EquipmentType 
{ 
Cue 
, 
BallSet 
, 
Rack 
, 	
Chalk 	
,	 

BridgeStick 
, 

TableCover$$ 
,$$ 
Lighting)) 
,)) 
Other.. 	
}// é
uC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\EquipmentStatus.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
EquipmentStatus 
{ 
	Available 
, 
InUse 	
,	 

MaintenanceRequired 
, 
OutOfService 
, 
Missing 
}   Ö
rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\DiscountType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
DiscountType 
{ 
Amount 

= 
$num 
, 

Percentage		 
=		 
$num		 
,		 
RePrice

 
=

 
$num

 
,

 
AltPrice 
= 
$num 
, 
FixedAmount 
= 
$num 
, 
MemberDiscount 
= 
$num 
, 
ManagerOverride 
= 
$num 
, 
Promotional 
= 
$num 
} ä
qC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\CutBehavior.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
CutBehavior 
{ 
Auto 
=	 

$num 
, 
Always 

= 
$num 
, 
Never 	
=
 
$num 
} ”
wC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\CashSessionStatus.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
CashSessionStatus 
{ 
Open 
=	 

$num 
, 
Closed		 

=		 
$num		 
}

 Î
tC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\AuditEventType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
AuditEventType 
{ 
Created 
= 
$num 
, 
Modified		 
=		 
$num		 
,		 
Deleted

 
=

 
$num

 
,

 
StatusChanged 
= 
$num 
, 
PaymentProcessed 
= 
$num 
, 
RefundProcessed 
= 
$num 
, 
Voided 

= 
$num 
, 
SystemShutdown 
= 
$num 
, 
Printed 
= 
$num 
, 
TicketTransferred 
= 
$num 
, 
TicketMerged 
= 
$num 
} Ê
uC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Enumerations\ApplicationType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Enumerations &
;& '
public 
enum 
ApplicationType 
{ 

FreeAmount 
= 
$num 
, 
FixedPerCategory		 
=		 
$num		 
,		 
FixedPerItem

 
=

 
$num

 
,

 
FixedPerOrder 
= 
$num 
, !
PercentagePerCategory 
= 
$num 
, 
PercentagePerItem 
= 
$num 
, 
PercentagePerOrder 
= 
$num 
} æ#
hC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Vendor.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
Vendor 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
null/ 3
!3 4
;4 5
public		 

string		 
?		 
ContactPerson		  
{		! "
get		# &
;		& '
private		( /
set		0 3
;		3 4
}		5 6
public

 

string

 
?

 
Email

 
{

 
get

 
;

 
private

  '
set

( +
;

+ ,
}

- .
public 

string 
? 
PhoneNumber 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

string 
? 
Address 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
private 
Vendor 
( 
) 
{ 
} 
public 

static 
Vendor 
Create 
(  
string  &
name' +
,+ ,
string- 3
?3 4
contactPerson5 B
=C D
nullE I
,I J
stringK Q
?Q R
emailS X
=Y Z
null[ _
)_ `
{ 
if 

( 
string 
. 
IsNullOrWhiteSpace %
(% &
name& *
)* +
)+ ,
throw- 2
new3 6
ArgumentException7 H
(H I
$strI b
)b c
;c d
return 
new 
Vendor 
{ 	
Id 
= 
Guid 
. 
NewGuid 
( 
) 
,  
Name 
= 
name 
, 
ContactPerson 
= 
contactPerson )
,) *
Email 
= 
email 
, 
IsActive 
= 
true 
} 	
;	 

} 
public 

void 
UpdateDetails 
( 
string $
name% )
,) *
string+ 1
?1 2
contact3 :
=; <
null= A
,A B
stringC I
?I J
emailK P
=Q R
nullS W
,W X
stringY _
?_ `
phonea f
=g h
nulli m
,m n
stringo u
?u v
addressw ~
=	 Ä
null
Å Ö
)
Ö Ü
{   
if!! 

(!! 
string!! 
.!! 
IsNullOrWhiteSpace!! %
(!!% &
name!!& *
)!!* +
)!!+ ,
throw!!- 2
new!!3 6
ArgumentException!!7 H
(!!H I
$str!!I b
)!!b c
;!!c d
Name"" 
="" 
name"" 
;"" 
ContactPerson## 
=## 
contact## 
;##  
Email$$ 
=$$ 
email$$ 
;$$ 
PhoneNumber%% 
=%% 
phone%% 
;%% 
Address&& 
=&& 
address&& 
;&& 
}'' 
public)) 

void)) 

Deactivate)) 
()) 
))) 
=>)) 
IsActive))  (
=))) *
false))+ 0
;))0 1
public** 

void** 
Activate** 
(** 
)** 
=>** 
IsActive** &
=**' (
true**) -
;**- .
}++ ¢8
fC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\User.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
User 
{ 
public		 

Guid		 
Id		 
{		 
get		 
;		 
private		 !
set		" %
;		% &
}		' (
public

 

string

 
Username

 
{

 
get

  
;

  !
private

" )
set

* -
;

- .
}

/ 0
=

1 2
string

3 9
.

9 :
Empty

: ?
;

? @
public 

string 
	FirstName 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
=2 3
string4 :
.: ;
Empty; @
;@ A
public 

string 
LastName 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
=1 2
string3 9
.9 :
Empty: ?
;? @
public 

string 
? 
EncryptedPin 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

string 
? 
EncryptedPassword $
{% &
get' *
;* +
private, 3
set4 7
;7 8
}9 :
public 

Guid 
RoleId 
{ 
get 
; 
private %
set& )
;) *
}+ ,
public 

virtual 
Role 
? 
Role 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

Money 

HourlyRate 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
private 
User 
( 
) 
{ 

HourlyRate 
= 
Money 
. 
Zero 
(  
)  !
;! "
} 
public 

static 
User 
Create 
( 
string 
username 
, 
string 
	firstName 
, 
string 
lastName 
, 
Guid 
roleId 
, 
string 
? 
encryptedPin 
= 
null #
,# $
string   
?   
encryptedPassword   !
=  " #
null  $ (
,  ( )
decimal!! 
?!! 

hourlyRate!! 
=!! 
null!! "
)!!" #
{"" 
if## 

(## 
string## 
.## 
IsNullOrWhiteSpace## %
(##% &
username##& .
)##. /
)##/ 0
throw$$ 
new$$ 
ArgumentException$$ '
($$' (
$str$$( C
,$$C D
nameof$$E K
($$K L
username$$L T
)$$T U
)$$U V
;$$V W
return&& 
new&& 
User&& 
{'' 	
Id(( 
=(( 
Guid(( 
.(( 
NewGuid(( 
((( 
)(( 
,((  
Username)) 
=)) 
username)) 
,))  
	FirstName** 
=** 
	firstName** !
,**! "
LastName++ 
=++ 
lastName++ 
,++  
RoleId,, 
=,, 
roleId,, 
,,, 
EncryptedPin-- 
=-- 
encryptedPin-- '
,--' (
EncryptedPassword.. 
=.. 
encryptedPassword..  1
,..1 2

HourlyRate// 
=// 

hourlyRate// #
.//# $
HasValue//$ ,
?//- .
new/// 2
Money//3 8
(//8 9

hourlyRate//9 C
.//C D
Value//D I
)//I J
://K L
Money//M R
.//R S
Zero//S W
(//W X
)//X Y
,//Y Z
PreferredLanguage00 
=00 
$str00  '
,00' (
IsActive11 
=11 
true11 
}22 	
;22	 

}33 
public55 

string55 
PreferredLanguage55 #
{55$ %
get55& )
;55) *
private55+ 2
set553 6
;556 7
}558 9
=55: ;
$str55< C
;55C D
public77 

void77  
SetPreferredLanguage77 $
(77$ %
string77% +
languageCode77, 8
)778 9
{88 
if99 

(99 
string99 
.99 
IsNullOrWhiteSpace99 %
(99% &
languageCode99& 2
)992 3
)993 4
throw995 :
new99; >
ArgumentException99? P
(99P Q
$str99Q p
)99p q
;99q r
PreferredLanguage:: 
=:: 
languageCode:: (
;::( )
};; 
public== 

void== 
SetRole== 
(== 
Guid== 
roleId== #
)==# $
{>> 
RoleId?? 
=?? 
roleId?? 
;?? 
}@@ 
publicBB 

voidBB 

DeactivateBB 
(BB 
)BB 
{CC 
IsActiveDD 
=DD 
falseDD 
;DD 
}EE 
publicGG 

voidGG 
ActivateGG 
(GG 
)GG 
{HH 
IsActiveII 
=II 
trueII 
;II 
}JJ 
publicLL 

voidLL 
UpdateDetailsLL 
(LL 
stringLL $
	firstNameLL% .
,LL. /
stringLL0 6
lastNameLL7 ?
)LL? @
{MM 
ifNN 

(NN 
stringNN 
.NN 
IsNullOrWhiteSpaceNN %
(NN% &
	firstNameNN& /
)NN/ 0
)NN0 1
throwNN2 7
newNN8 ;
ArgumentExceptionNN< M
(NNM N
$strNNN c
)NNc d
;NNd e
	FirstNameOO 
=OO 
	firstNameOO 
;OO 
LastNamePP 
=PP 
lastNamePP 
;PP 
}SS 
publicUU 

voidUU 
SetPinUU 
(UU 
stringUU 
encryptedPinUU *
)UU* +
{VV 
EncryptedPinWW 
=WW 
encryptedPinWW #
;WW# $
}XX 
}YY ≥"
pC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\TicketDiscount.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 
TicketDiscount

 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 
TicketId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

Guid 

DiscountId 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

DiscountType 
Type 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

decimal 
Value 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

Money 
? 
MinimumAmount 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

Money 
Amount 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

DateTime 
	AppliedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

UserId 
? 
	AppliedBy 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

UserId 
? 
AuthorizedBy 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
private 
TicketDiscount 
( 
) 
{ 
Amount 
= 
Money 
. 
Zero 
( 
) 
; 
} 
public 

static 
TicketDiscount  
Create! '
(' (
Guid 
ticketId 
, 
Guid 

discountId 
, 
string   
name   
,   
DiscountType!! 
type!! 
,!! 
decimal"" 
value"" 
,"" 
Money## 
amount## 
,## 
UserId$$ 
?$$ 
	appliedBy$$ 
,$$ 
UserId%% 
?%% 
authorizedBy%% 
=%% 
null%% #
,%%# $
Money&& 
?&& 
minimumAmount&& 
=&& 
null&& #
)&&# $
{'' 
return(( 
new(( 
TicketDiscount(( !
{)) 	
Id** 
=** 
Guid** 
.** 
NewGuid** 
(** 
)** 
,**  
TicketId++ 
=++ 
ticketId++ 
,++  

DiscountId,, 
=,, 

discountId,, #
,,,# $
Name-- 
=-- 
name-- 
,-- 
Type.. 
=.. 
type.. 
,.. 
Value// 
=// 
value// 
,// 
Amount00 
=00 
amount00 
,00 
MinimumAmount11 
=11 
minimumAmount11 )
,11) *
	AppliedAt22 
=22 
DateTime22  
.22  !
UtcNow22! '
,22' (
	AppliedBy33 
=33 
	appliedBy33 !
,33! "
AuthorizedBy44 
=44 
authorizedBy44 '
}55 	
;55	 

}66 
}77 Üé
hC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Ticket.cs
	namespace		 	
Magidesk		
 
.		 
Domain		 
.		 
Entities		 "
;		" #
public 
class 
Ticket 
{ 
private 
readonly 
List 
< 
	OrderLine #
># $
_orderLines% 0
=1 2
new3 6
(6 7
)7 8
;8 9
private 
readonly 
List 
< 
Payment !
>! "
	_payments# ,
=- .
new/ 2
(2 3
)3 4
;4 5
private 
readonly 
List 
< 
TicketDiscount (
>( )

_discounts* 4
=5 6
new7 :
(: ;
); <
;< =
private 
readonly 
List 
< 
int 
> 
_tableNumbers ,
=- .
new/ 2
(2 3
)3 4
;4 5
private 
readonly 

Dictionary 
<  
string  &
,& '
string( .
>. /
_properties0 ;
=< =
new> A
(A B
)B C
;C D
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

int 
TicketNumber 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

string 
? 
GlobalId 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
? 
OpenedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
? 
ClosedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 

ActiveDate 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

DateTime 
? 
DeliveryDate !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public   

TicketStatus   
Status   
{    
get  ! $
;  $ %
private  & -
set  . 1
;  1 2
}  3 4
public## 

UserId## 
	CreatedBy## 
{## 
get## !
;##! "
private### *
set##+ .
;##. /
}##0 1
=##2 3
null##4 8
!##8 9
;##9 :
public$$ 

UserId$$ 
?$$ 
ClosedBy$$ 
{$$ 
get$$ !
;$$! "
private$$# *
set$$+ .
;$$. /
}$$0 1
public%% 

UserId%% 
?%% 
VoidedBy%% 
{%% 
get%% !
;%%! "
private%%# *
set%%+ .
;%%. /
}%%0 1
public(( 

Guid(( 

TerminalId(( 
{(( 
get((  
;((  !
private((" )
set((* -
;((- .
}((/ 0
public)) 

Guid)) 
ShiftId)) 
{)) 
get)) 
;)) 
private)) &
set))' *
;))* +
})), -
public** 

Guid** 
OrderTypeId** 
{** 
get** !
;**! "
private**# *
set**+ .
;**. /
}**0 1
public++ 

Guid++ 
?++ 

CustomerId++ 
{++ 
get++ !
;++! "
private++# *
set+++ .
;++. /
}++0 1
public,, 

Guid,, 
?,, 
	SessionId,, 
{,, 
get,,  
;,,  !
private,," )
set,,* -
;,,- .
},,/ 0
public-- 

Guid-- 
?-- 
AssignedDriverId-- !
{--" #
get--$ '
;--' (
private--) 0
set--1 4
;--4 5
}--6 7
public00 

IReadOnlyList00 
<00 
int00 
>00 
TableNumbers00 *
=>00+ -
_tableNumbers00. ;
.00; <

AsReadOnly00< F
(00F G
)00G H
;00H I
public11 

int11 
NumberOfGuests11 
{11 
get11  #
;11# $
private11% ,
set11- 0
;110 1
}112 3
public44 

Money44 
SubtotalAmount44 
{44  !
get44" %
;44% &
private44' .
set44/ 2
;442 3
}444 5
public55 

Money55 
DiscountAmount55 
{55  !
get55" %
;55% &
private55' .
set55/ 2
;552 3
}554 5
public66 

Money66 
	TaxAmount66 
{66 
get66  
;66  !
private66" )
set66* -
;66- .
}66/ 0
public77 

Money77 
ServiceChargeAmount77 $
{77% &
get77' *
;77* +
private77, 3
set774 7
;777 8
}779 :
public88 

Money88  
DeliveryChargeAmount88 %
{88& '
get88( +
;88+ ,
private88- 4
set885 8
;888 9
}88: ;
public99 

Money99 
AdjustmentAmount99 !
{99" #
get99$ '
;99' (
private99) 0
set991 4
;994 5
}996 7
public:: 

Money:: 
TotalAmount:: 
{:: 
get:: "
;::" #
private::$ +
set::, /
;::/ 0
}::1 2
public;; 

Money;; 

PaidAmount;; 
{;; 
get;; !
;;;! "
private;;# *
set;;+ .
;;;. /
};;0 1
public<< 

Money<< 
	DueAmount<< 
{<< 
get<<  
;<<  !
private<<" )
set<<* -
;<<- .
}<</ 0
public== 

Money== 
AdvanceAmount== 
{==  
get==! $
;==$ %
private==& -
set==. 1
;==1 2
}==3 4
public@@ 

bool@@ 
IsTaxExempt@@ 
{@@ 
get@@ !
;@@! "
private@@# *
set@@+ .
;@@. /
}@@0 1
publicAA 

boolAA 
PriceIncludesTaxAA  
{AA! "
getAA# &
;AA& '
privateAA( /
setAA0 3
;AA3 4
}AA5 6
publicBB 

boolBB 
IsBarTabBB 
{BB 
getBB 
;BB 
privateBB  '
setBB( +
;BB+ ,
}BB- .
publicCC 

boolCC 

IsReOpenedCC 
{CC 
getCC  
;CC  !
privateCC" )
setCC* -
;CC- .
}CC/ 0
publicFF 

stringFF 
?FF 
DeliveryAddressFF "
{FF# $
getFF% (
;FF( )
privateFF* 1
setFF2 5
;FF5 6
}FF7 8
publicGG 

stringGG 
?GG 
ExtraDeliveryInfoGG $
{GG% &
getGG' *
;GG* +
privateGG, 3
setGG4 7
;GG7 8
}GG9 :
publicHH 

boolHH 
CustomerWillPickupHH "
{HH# $
getHH% (
;HH( )
privateHH* 1
setHH2 5
;HH5 6
}HH7 8
publicII 

DateTimeII 
?II 
DispatchedTimeII #
{II$ %
getII& )
;II) *
privateII+ 2
setII3 6
;II6 7
}II8 9
publicJJ 

DateTimeJJ 
?JJ 
	ReadyTimeJJ 
{JJ  
getJJ! $
;JJ$ %
privateJJ& -
setJJ. 1
;JJ1 2
}JJ3 4
publicMM 

IReadOnlyCollectionMM 
<MM 
	OrderLineMM (
>MM( )

OrderLinesMM* 4
=>MM5 7
_orderLinesMM8 C
.MMC D

AsReadOnlyMMD N
(MMN O
)MMO P
;MMP Q
publicNN 

IReadOnlyCollectionNN 
<NN 
PaymentNN &
>NN& '
PaymentsNN( 0
=>NN1 3
	_paymentsNN4 =
.NN= >

AsReadOnlyNN> H
(NNH I
)NNI J
;NNJ K
publicOO 

IReadOnlyCollectionOO 
<OO 
TicketDiscountOO -
>OO- .
	DiscountsOO/ 8
=>OO9 ;

_discountsOO< F
.OOF G

AsReadOnlyOOG Q
(OOQ R
)OOR S
;OOS T
publicPP 

GratuityPP 
?PP 
GratuityPP 
{PP 
getPP  #
;PP# $
privatePP% ,
setPP- 0
;PP0 1
}PP2 3
publicSS 

intSS 
VersionSS 
{SS 
getSS 
;SS 
privateSS %
setSS& )
;SS) *
}SS+ ,
publicWW 

IReadOnlyDictionaryWW 
<WW 
stringWW %
,WW% &
stringWW' -
>WW- .

PropertiesWW/ 9
=>WW: <
_propertiesWW= H
.WWH I

AsReadOnlyWWI S
(WWS T
)WWT U
;WWU V
publicZZ 

stringZZ 
?ZZ 
NoteZZ 
{ZZ 
getZZ 
;ZZ 
privateZZ &
setZZ' *
;ZZ* +
}ZZ, -
public`` 

DateTime`` 
?`` 
HeldAt`` 
{`` 
get`` !
;``! "
private``# *
set``+ .
;``. /
}``0 1
publicee 

stringee 
?ee 

HoldReasonee 
{ee 
getee  #
;ee# $
privateee% ,
setee- 0
;ee0 1
}ee2 3
publicjj 

UserIdjj 
?jj 
HeldByjj 
{jj 
getjj 
;jj  
privatejj! (
setjj) ,
;jj, -
}jj. /
privatemm 
Ticketmm 
(mm 
)mm 
{nn 
SubtotalAmountoo 
=oo 
Moneyoo 
.oo 
Zerooo #
(oo# $
)oo$ %
;oo% &
DiscountAmountpp 
=pp 
Moneypp 
.pp 
Zeropp #
(pp# $
)pp$ %
;pp% &
	TaxAmountqq 
=qq 
Moneyqq 
.qq 
Zeroqq 
(qq 
)qq  
;qq  !
ServiceChargeAmountrr 
=rr 
Moneyrr #
.rr# $
Zerorr$ (
(rr( )
)rr) *
;rr* + 
DeliveryChargeAmountss 
=ss 
Moneyss $
.ss$ %
Zeross% )
(ss) *
)ss* +
;ss+ ,
AdjustmentAmounttt 
=tt 
Moneytt  
.tt  !
Zerott! %
(tt% &
)tt& '
;tt' (
TotalAmountuu 
=uu 
Moneyuu 
.uu 
Zerouu  
(uu  !
)uu! "
;uu" #

PaidAmountvv 
=vv 
Moneyvv 
.vv 
Zerovv 
(vv  
)vv  !
;vv! "
	DueAmountww 
=ww 
Moneyww 
.ww 
Zeroww 
(ww 
)ww  
;ww  !
AdvanceAmountxx 
=xx 
Moneyxx 
.xx 
Zeroxx "
(xx" #
)xx# $
;xx$ %
NumberOfGuestsyy 
=yy 
$numyy 
;yy 
}zz 
public 

static 
Ticket 
Create 
(  
int
ÄÄ 
ticketNumber
ÄÄ 
,
ÄÄ 
UserId
ÅÅ 
	createdBy
ÅÅ 
,
ÅÅ 
Guid
ÇÇ 

terminalId
ÇÇ 
,
ÇÇ 
Guid
ÉÉ 
shiftId
ÉÉ 
,
ÉÉ 
Guid
ÑÑ 
orderTypeId
ÑÑ 
,
ÑÑ 
string
ÖÖ 
?
ÖÖ 
globalId
ÖÖ 
=
ÖÖ 
null
ÖÖ 
)
ÖÖ  
{
ÜÜ 
return
áá 
new
áá 
Ticket
áá 
{
àà 	
Id
ââ 
=
ââ 
Guid
ââ 
.
ââ 
NewGuid
ââ 
(
ââ 
)
ââ 
,
ââ  
TicketNumber
ää 
=
ää 
ticketNumber
ää '
,
ää' (
GlobalId
ãã 
=
ãã 
globalId
ãã 
,
ãã  
	CreatedBy
åå 
=
åå 
	createdBy
åå !
,
åå! "

TerminalId
çç 
=
çç 

terminalId
çç #
,
çç# $
ShiftId
éé 
=
éé 
shiftId
éé 
,
éé 
OrderTypeId
èè 
=
èè 
orderTypeId
èè %
,
èè% &
Status
êê 
=
êê 
TicketStatus
êê !
.
êê! "
Draft
êê" '
,
êê' (
	CreatedAt
ëë 
=
ëë 
DateTime
ëë  
.
ëë  !
UtcNow
ëë! '
,
ëë' (

ActiveDate
íí 
=
íí 
DateTime
íí !
.
íí! "
UtcNow
íí" (
,
íí( )
Version
ìì 
=
ìì 
$num
ìì 
}
îî 	
;
îî	 

}
ïï 
public
öö 

void
öö 
Open
öö 
(
öö 
)
öö 
{
õõ 
if
úú 

(
úú 
Status
úú 
!=
úú 
TicketStatus
úú "
.
úú" #
Draft
úú# (
)
úú( )
{
ùù 	
throw
ûû 
new
ûû -
DomainInvalidOperationException
ûû 5
(
ûû5 6
$"
ûû6 8
$str
ûû8 N
{
ûûN O
Status
ûûO U
}
ûûU V
$str
ûûV ^
"
ûû^ _
)
ûû_ `
;
ûû` a
}
üü 	
Status
°° 
=
°° 
TicketStatus
°° 
.
°° 
Open
°° "
;
°°" #
OpenedAt
¢¢ 
=
¢¢ 
DateTime
¢¢ 
.
¢¢ 
UtcNow
¢¢ "
;
¢¢" #

ActiveDate
££ 
=
££ 
DateTime
££ 
.
££ 
UtcNow
££ $
;
££$ %
IncrementVersion
§§ 
(
§§ 
)
§§ 
;
§§ 
}
•• 
public
™™ 

void
™™ 
AddOrderLine
™™ 
(
™™ 
	OrderLine
™™ &
	orderLine
™™' 0
)
™™0 1
{
´´ 
if
¨¨ 

(
¨¨ 
	orderLine
¨¨ 
==
¨¨ 
null
¨¨ 
)
¨¨ 
{
≠≠ 	
throw
ÆÆ 
new
ÆÆ #
ArgumentNullException
ÆÆ +
(
ÆÆ+ ,
nameof
ÆÆ, 2
(
ÆÆ2 3
	orderLine
ÆÆ3 <
)
ÆÆ< =
)
ÆÆ= >
;
ÆÆ> ?
}
ØØ 	
if
±± 

(
±± 
Status
±± 
==
±± 
TicketStatus
±± "
.
±±" #
Closed
±±# )
||
±±* ,
Status
±±- 3
==
±±4 6
TicketStatus
±±7 C
.
±±C D
Voided
±±D J
||
±±K M
Status
±±N T
==
±±U W
TicketStatus
±±X d
.
±±d e
Refunded
±±e m
)
±±m n
{
≤≤ 	
throw
≥≥ 
new
≥≥ -
DomainInvalidOperationException
≥≥ 5
(
≥≥5 6
$"
≥≥6 8
$str
≥≥8 V
{
≥≥V W
Status
≥≥W ]
}
≥≥] ^
$str
≥≥^ f
"
≥≥f g
)
≥≥g h
;
≥≥h i
}
¥¥ 	
if
∂∂ 

(
∂∂ 
	orderLine
∂∂ 
.
∂∂ 
TicketId
∂∂ 
!=
∂∂ !
Id
∂∂" $
)
∂∂$ %
{
∑∑ 	
throw
∏∏ 
new
∏∏ ,
BusinessRuleViolationException
∏∏ 4
(
∏∏4 5
$str
∏∏5 `
)
∏∏` a
;
∏∏a b
}
ππ 	
_orderLines
ªª 
.
ªª 
Add
ªª 
(
ªª 
	orderLine
ªª !
)
ªª! "
;
ªª" #

ActiveDate
ºº 
=
ºº 
DateTime
ºº 
.
ºº 
UtcNow
ºº $
;
ºº$ %
IncrementVersion
ΩΩ 
(
ΩΩ 
)
ΩΩ 
;
ΩΩ 
if
¿¿ 

(
¿¿ 
Status
¿¿ 
==
¿¿ 
TicketStatus
¿¿ "
.
¿¿" #
Draft
¿¿# (
)
¿¿( )
{
¡¡ 	
Open
¬¬ 
(
¬¬ 
)
¬¬ 
;
¬¬ 
}
√√ 	
CalculateTotals
≈≈ 
(
≈≈ 
)
≈≈ 
;
≈≈ 
}
∆∆ 
public
ÀÀ 

void
ÀÀ 
RemoveOrderLine
ÀÀ 
(
ÀÀ  
Guid
ÀÀ  $
orderLineId
ÀÀ% 0
)
ÀÀ0 1
{
ÃÃ 
if
ÕÕ 

(
ÕÕ 
Status
ÕÕ 
==
ÕÕ 
TicketStatus
ÕÕ "
.
ÕÕ" #
Closed
ÕÕ# )
||
ÕÕ* ,
Status
ÕÕ- 3
==
ÕÕ4 6
TicketStatus
ÕÕ7 C
.
ÕÕC D
Voided
ÕÕD J
||
ÕÕK M
Status
ÕÕN T
==
ÕÕU W
TicketStatus
ÕÕX d
.
ÕÕd e
Refunded
ÕÕe m
)
ÕÕm n
{
ŒŒ 	
throw
œœ 
new
œœ -
DomainInvalidOperationException
œœ 5
(
œœ5 6
$"
œœ6 8
$str
œœ8 [
{
œœ[ \
Status
œœ\ b
}
œœb c
$str
œœc k
"
œœk l
)
œœl m
;
œœm n
}
–– 	
var
““ 
	orderLine
““ 
=
““ 
_orderLines
““ #
.
““# $
FirstOrDefault
““$ 2
(
““2 3
ol
““3 5
=>
““6 8
ol
““9 ;
.
““; <
Id
““< >
==
““? A
orderLineId
““B M
)
““M N
;
““N O
if
”” 

(
”” 
	orderLine
”” 
==
”” 
null
”” 
)
”” 
{
‘‘ 	
throw
’’ 
new
’’ ,
BusinessRuleViolationException
’’ 4
(
’’4 5
$"
’’5 7
$str
’’7 A
{
’’A B
orderLineId
’’B M
}
’’M N
$str
’’N Y
"
’’Y Z
)
’’Z [
;
’’[ \
}
÷÷ 	
_orderLines
ÿÿ 
.
ÿÿ 
Remove
ÿÿ 
(
ÿÿ 
	orderLine
ÿÿ $
)
ÿÿ$ %
;
ÿÿ% &

ActiveDate
ŸŸ 
=
ŸŸ 
DateTime
ŸŸ 
.
ŸŸ 
UtcNow
ŸŸ $
;
ŸŸ$ %
IncrementVersion
⁄⁄ 
(
⁄⁄ 
)
⁄⁄ 
;
⁄⁄ 
CalculateTotals
€€ 
(
€€ 
)
€€ 
;
€€ 
}
‹‹ 
public
·· 

void
·· 

AddPayment
·· 
(
·· 
Payment
·· "
payment
··# *
)
··* +
{
‚‚ 
if
„„ 

(
„„ 
payment
„„ 
==
„„ 
null
„„ 
)
„„ 
{
‰‰ 	
throw
ÂÂ 
new
ÂÂ #
ArgumentNullException
ÂÂ +
(
ÂÂ+ ,
nameof
ÂÂ, 2
(
ÂÂ2 3
payment
ÂÂ3 :
)
ÂÂ: ;
)
ÂÂ; <
;
ÂÂ< =
}
ÊÊ 	
if
ËË 

(
ËË 
Status
ËË 
==
ËË 
TicketStatus
ËË "
.
ËË" #
Closed
ËË# )
||
ËË* ,
Status
ËË- 3
==
ËË4 6
TicketStatus
ËË7 C
.
ËËC D
Voided
ËËD J
||
ËËK M
Status
ËËN T
==
ËËU W
TicketStatus
ËËX d
.
ËËd e
Refunded
ËËe m
)
ËËm n
{
ÈÈ 	
throw
ÍÍ 
new
ÍÍ -
DomainInvalidOperationException
ÍÍ 5
(
ÍÍ5 6
$"
ÍÍ6 8
$str
ÍÍ8 X
{
ÍÍX Y
Status
ÍÍY _
}
ÍÍ_ `
$str
ÍÍ` h
"
ÍÍh i
)
ÍÍi j
;
ÍÍj k
}
ÎÎ 	
if
ÌÌ 

(
ÌÌ 
payment
ÌÌ 
.
ÌÌ 
TicketId
ÌÌ 
!=
ÌÌ 
Id
ÌÌ  "
)
ÌÌ" #
{
ÓÓ 	
throw
ÔÔ 
new
ÔÔ ,
BusinessRuleViolationException
ÔÔ 4
(
ÔÔ4 5
$str
ÔÔ5 ^
)
ÔÔ^ _
;
ÔÔ_ `
}
 	
	_payments
ÚÚ 
.
ÚÚ 
Add
ÚÚ 
(
ÚÚ 
payment
ÚÚ 
)
ÚÚ 
;
ÚÚ 

ActiveDate
ÛÛ 
=
ÛÛ 
DateTime
ÛÛ 
.
ÛÛ 
UtcNow
ÛÛ $
;
ÛÛ$ %
IncrementVersion
ÙÙ 
(
ÙÙ 
)
ÙÙ 
;
ÙÙ 
if
˜˜ 

(
˜˜ 
Status
˜˜ 
==
˜˜ 
TicketStatus
˜˜ "
.
˜˜" #
Draft
˜˜# (
)
˜˜( )
{
¯¯ 	
Open
˘˘ 
(
˘˘ 
)
˘˘ 
;
˘˘ 
}
˙˙ 	#
RecalculatePaidAmount
¸¸ 
(
¸¸ 
)
¸¸ 
;
¸¸  
	DueAmount
ÄÄ 
=
ÄÄ 

PaidAmount
ÄÄ 
>=
ÄÄ !
TotalAmount
ÄÄ" -
?
ÅÅ 
Money
ÅÅ 
.
ÅÅ 
Zero
ÅÅ 
(
ÅÅ 
TotalAmount
ÅÅ $
.
ÅÅ$ %
Currency
ÅÅ% -
)
ÅÅ- .
:
ÇÇ 
TotalAmount
ÇÇ 
-
ÇÇ 

PaidAmount
ÇÇ &
;
ÇÇ& '
if
ÖÖ 

(
ÖÖ 

PaidAmount
ÖÖ 
>=
ÖÖ 
TotalAmount
ÖÖ %
&&
ÖÖ& (
Status
ÖÖ) /
==
ÖÖ0 2
TicketStatus
ÖÖ3 ?
.
ÖÖ? @
Open
ÖÖ@ D
)
ÖÖD E
{
ÜÜ 	
Status
áá 
=
áá 
TicketStatus
áá !
.
áá! "
Paid
áá" &
;
áá& '
}
àà 	
}
ââ 
public
éé 

bool
éé 
CanAddPayment
éé 
(
éé 
Payment
éé %
payment
éé& -
)
éé- .
{
èè 
if
êê 

(
êê 
payment
êê 
==
êê 
null
êê 
)
êê 
{
ëë 	
throw
íí 
new
íí #
ArgumentNullException
íí +
(
íí+ ,
nameof
íí, 2
(
íí2 3
payment
íí3 :
)
íí: ;
)
íí; <
;
íí< =
}
ìì 	
if
ïï 

(
ïï 
Status
ïï 
==
ïï 
TicketStatus
ïï "
.
ïï" #
Closed
ïï# )
||
ïï* ,
Status
ïï- 3
==
ïï4 6
TicketStatus
ïï7 C
.
ïïC D
Voided
ïïD J
||
ïïK M
Status
ïïN T
==
ïïU W
TicketStatus
ïïX d
.
ïïd e
Refunded
ïïe m
)
ïïm n
{
ññ 	
return
óó 
false
óó 
;
óó 
}
òò 	
if
öö 

(
öö 
payment
öö 
.
öö 
TicketId
öö 
!=
öö 
Id
öö  "
)
öö" #
{
õõ 	
return
úú 
false
úú 
;
úú 
}
ùù 	
return
üü 
true
üü 
;
üü 
}
†† 
public
•• 

void
•• 
Close
•• 
(
•• 
UserId
•• 
closedBy
•• %
)
••% &
{
¶¶ 
if
ßß 

(
ßß 
!
ßß 
CanClose
ßß 
(
ßß 
)
ßß 
)
ßß 
{
®® 	
throw
©© 
new
©© -
DomainInvalidOperationException
©© 5
(
©©5 6
$"
©©6 8
$str
©©8 O
{
©©O P
Status
©©P V
}
©©V W
$str
©©W _
"
©©_ `
)
©©` a
;
©©a b
}
™™ 	
Status
¨¨ 
=
¨¨ 
TicketStatus
¨¨ 
.
¨¨ 
Closed
¨¨ $
;
¨¨$ %
ClosedAt
≠≠ 
=
≠≠ 
DateTime
≠≠ 
.
≠≠ 
UtcNow
≠≠ "
;
≠≠" #
ClosedBy
ÆÆ 
=
ÆÆ 
closedBy
ÆÆ 
;
ÆÆ 

ActiveDate
ØØ 
=
ØØ 
DateTime
ØØ 
.
ØØ 
UtcNow
ØØ $
;
ØØ$ %
IncrementVersion
∞∞ 
(
∞∞ 
)
∞∞ 
;
∞∞ 
}
±± 
public
∂∂ 

bool
∂∂ 
CanClose
∂∂ 
(
∂∂ 
)
∂∂ 
{
∑∑ 
if
∏∏ 

(
∏∏ 
Status
∏∏ 
!=
∏∏ 
TicketStatus
∏∏ "
.
∏∏" #
Paid
∏∏# '
)
∏∏' (
{
ππ 	
return
∫∫ 
false
∫∫ 
;
∫∫ 
}
ªª 	
if
ΩΩ 

(
ΩΩ 
	DueAmount
ΩΩ 
>
ΩΩ 
Money
ΩΩ 
.
ΩΩ 
Zero
ΩΩ "
(
ΩΩ" #
)
ΩΩ# $
)
ΩΩ$ %
{
ææ 	
return
øø 
false
øø 
;
øø 
}
¿¿ 	
return
¬¬ 
true
¬¬ 
;
¬¬ 
}
√√ 
public
»» 

void
»» 
Void
»» 
(
»» 
UserId
»» 
voidedBy
»» $
,
»»$ %
string
»»& ,
reason
»»- 3
,
»»3 4
bool
»»5 9
waste
»»: ?
)
»»? @
{
…… 
if
   

(
   
!
   
CanVoid
   
(
   
)
   
)
   
{
ÀÀ 	
throw
ÃÃ 
new
ÃÃ -
DomainInvalidOperationException
ÃÃ 5
(
ÃÃ5 6
$"
ÃÃ6 8
$str
ÃÃ8 N
{
ÃÃN O
Status
ÃÃO U
}
ÃÃU V
$str
ÃÃV ^
"
ÃÃ^ _
)
ÃÃ_ `
;
ÃÃ` a
}
ÕÕ 	
Status
œœ 
=
œœ 
TicketStatus
œœ 
.
œœ 
Voided
œœ $
;
œœ$ %
VoidedBy
–– 
=
–– 
voidedBy
–– 
;
–– 
_properties
—— 
[
—— 
$str
——  
]
——  !
=
——" #
reason
——$ *
;
——* +
_properties
““ 
[
““ 
$str
““ 
]
““ 
=
““  !
waste
““" '
.
““' (
ToString
““( 0
(
““0 1
)
““1 2
;
““2 3

ActiveDate
”” 
=
”” 
DateTime
”” 
.
”” 
UtcNow
”” $
;
””$ %
IncrementVersion
‘‘ 
(
‘‘ 
)
‘‘ 
;
‘‘ 
}
’’ 
public
⁄⁄ 

bool
⁄⁄ 
CanVoid
⁄⁄ 
(
⁄⁄ 
)
⁄⁄ 
{
€€ 
if
‹‹ 

(
‹‹ 
Status
‹‹ 
==
‹‹ 
TicketStatus
‹‹ "
.
‹‹" #
Closed
‹‹# )
||
‹‹* ,
Status
‹‹- 3
==
‹‹4 6
TicketStatus
‹‹7 C
.
‹‹C D
Refunded
‹‹D L
)
‹‹L M
{
›› 	
return
ﬁﬁ 
false
ﬁﬁ 
;
ﬁﬁ 
}
ﬂﬂ 	
if
‚‚ 

(
‚‚ 
	_payments
‚‚ 
.
‚‚ 
Any
‚‚ 
(
‚‚ 
p
‚‚ 
=>
‚‚ 
!
‚‚  
p
‚‚  !
.
‚‚! "
IsVoided
‚‚" *
)
‚‚* +
)
‚‚+ ,
{
„„ 	
return
‰‰ 
false
‰‰ 
;
‰‰ 
}
ÂÂ 	
return
ÁÁ 
true
ÁÁ 
;
ÁÁ 
}
ËË 
public
ÌÌ 

bool
ÌÌ 
	CanRefund
ÌÌ 
(
ÌÌ 
)
ÌÌ 
{
ÓÓ 
return
 
Status
 
==
 
TicketStatus
 %
.
% &
Closed
& ,
;
, -
}
ÒÒ 
public
˜˜ 

void
˜˜ 
ProcessRefund
˜˜ 
(
˜˜ 
Payment
˜˜ %
refundPayment
˜˜& 3
)
˜˜3 4
{
¯¯ 
if
˘˘ 

(
˘˘ 
refundPayment
˘˘ 
==
˘˘ 
null
˘˘ !
)
˘˘! "
{
˙˙ 	
throw
˚˚ 
new
˚˚ #
ArgumentNullException
˚˚ +
(
˚˚+ ,
nameof
˚˚, 2
(
˚˚2 3
refundPayment
˚˚3 @
)
˚˚@ A
)
˚˚A B
;
˚˚B C
}
¸¸ 	
if
˛˛ 

(
˛˛ 
!
˛˛ 
	CanRefund
˛˛ 
(
˛˛ 
)
˛˛ 
)
˛˛ 
{
ˇˇ 	
throw
ÄÄ 
new
ÄÄ -
DomainInvalidOperationException
ÄÄ 5
(
ÄÄ5 6
$"
ÄÄ6 8
$str
ÄÄ8 P
{
ÄÄP Q
Status
ÄÄQ W
}
ÄÄW X
$str
ÄÄX `
"
ÄÄ` a
)
ÄÄa b
;
ÄÄb c
}
ÅÅ 	
if
ÉÉ 

(
ÉÉ 
refundPayment
ÉÉ 
.
ÉÉ 
TicketId
ÉÉ "
!=
ÉÉ# %
Id
ÉÉ& (
)
ÉÉ( )
{
ÑÑ 	
throw
ÖÖ 
new
ÖÖ ,
BusinessRuleViolationException
ÖÖ 4
(
ÖÖ4 5
$str
ÖÖ5 e
)
ÖÖe f
;
ÖÖf g
}
ÜÜ 	
if
àà 

(
àà 
refundPayment
àà 
.
àà 
TransactionType
àà )
!=
àà* ,
TransactionType
àà- <
.
àà< =
Debit
àà= B
)
ààB C
{
ââ 	
throw
ää 
new
ää ,
BusinessRuleViolationException
ää 4
(
ää4 5
$str
ää5 f
)
ääf g
;
ääg h
}
ãã 	
	_payments
éé 
.
éé 
Add
éé 
(
éé 
refundPayment
éé #
)
éé# $
;
éé$ %

ActiveDate
èè 
=
èè 
DateTime
èè 
.
èè 
UtcNow
èè $
;
èè$ %
IncrementVersion
êê 
(
êê 
)
êê 
;
êê #
RecalculatePaidAmount
ëë 
(
ëë 
)
ëë 
;
ëë  
	DueAmount
îî 
=
îî 

PaidAmount
îî 
>=
îî !
TotalAmount
îî" -
?
ïï 
Money
ïï 
.
ïï 
Zero
ïï 
(
ïï 
TotalAmount
ïï $
.
ïï$ %
Currency
ïï% -
)
ïï- .
:
ññ 
TotalAmount
ññ 
-
ññ 

PaidAmount
ññ &
;
ññ& '
if
ôô 

(
ôô 

PaidAmount
ôô 
<=
ôô 
Money
ôô 
.
ôô  
Zero
ôô  $
(
ôô$ %
)
ôô% &
)
ôô& '
{
öö 	
Status
õõ 
=
õõ 
TicketStatus
õõ !
.
õõ! "
Refunded
õõ" *
;
õõ* +
}
úú 	
}
ùù 
public
¢¢ 

bool
¢¢ 
CanSplit
¢¢ 
(
¢¢ 
)
¢¢ 
{
££ 
return
•• 
Status
•• 
==
•• 
TicketStatus
•• %
.
••% &
Open
••& *
;
••* +
}
¶¶ 
public
´´ 

Money
´´ 
GetRemainingDue
´´  
(
´´  !
)
´´! "
{
¨¨ 
return
≠≠ 
TotalAmount
≠≠ 
-
≠≠ 

PaidAmount
≠≠ '
;
≠≠' (
}
ÆÆ 
public
≥≥ 

void
≥≥ 
Reopen
≥≥ 
(
≥≥ 
)
≥≥ 
{
¥¥ 
if
µµ 

(
µµ 
Status
µµ 
!=
µµ 
TicketStatus
µµ "
.
µµ" #
Closed
µµ# )
)
µµ) *
{
∂∂ 	
throw
∑∑ 
new
∑∑ -
DomainInvalidOperationException
∑∑ 5
(
∑∑5 6
$"
∑∑6 8
$str
∑∑8 P
{
∑∑P Q
Status
∑∑Q W
}
∑∑W X
$str
∑∑X `
"
∑∑` a
)
∑∑a b
;
∑∑b c
}
∏∏ 	
Status
∫∫ 
=
∫∫ 
TicketStatus
∫∫ 
.
∫∫ 
Open
∫∫ "
;
∫∫" #

IsReOpened
ªª 
=
ªª 
true
ªª 
;
ªª 
ClosedAt
ºº 
=
ºº 
null
ºº 
;
ºº 
ClosedBy
ΩΩ 
=
ΩΩ 
null
ΩΩ 
;
ΩΩ 

ActiveDate
ææ 
=
ææ 
DateTime
ææ 
.
ææ 
UtcNow
ææ $
;
ææ$ %
IncrementVersion
øø 
(
øø 
)
øø 
;
øø 
}
¿¿ 
public
≈≈ 

void
≈≈ 
ApplyDiscount
≈≈ 
(
≈≈ 
TicketDiscount
≈≈ ,
discount
≈≈- 5
)
≈≈5 6
{
∆∆ 
if
«« 

(
«« 
discount
«« 
==
«« 
null
«« 
)
«« 
{
»» 	
throw
…… 
new
…… #
ArgumentNullException
…… +
(
……+ ,
nameof
……, 2
(
……2 3
discount
……3 ;
)
……; <
)
……< =
;
……= >
}
   	
if
ÃÃ 

(
ÃÃ 
Status
ÃÃ 
==
ÃÃ 
TicketStatus
ÃÃ "
.
ÃÃ" #
Closed
ÃÃ# )
||
ÃÃ* ,
Status
ÃÃ- 3
==
ÃÃ4 6
TicketStatus
ÃÃ7 C
.
ÃÃC D
Voided
ÃÃD J
||
ÃÃK M
Status
ÃÃN T
==
ÃÃU W
TicketStatus
ÃÃX d
.
ÃÃd e
Refunded
ÃÃe m
)
ÃÃm n
{
ÕÕ 	
throw
ŒŒ 
new
ŒŒ -
DomainInvalidOperationException
ŒŒ 5
(
ŒŒ5 6
$"
ŒŒ6 8
$str
ŒŒ8 [
{
ŒŒ[ \
Status
ŒŒ\ b
}
ŒŒb c
$str
ŒŒc k
"
ŒŒk l
)
ŒŒl m
;
ŒŒm n
}
œœ 	
if
—— 

(
—— 
discount
—— 
.
—— 
TicketId
—— 
!=
——  
Id
——! #
)
——# $
{
““ 	
throw
”” 
new
”” ,
BusinessRuleViolationException
”” 4
(
””4 5
$str
””5 _
)
””_ `
;
””` a
}
‘‘ 	

_discounts
÷÷ 
.
÷÷ 
Add
÷÷ 
(
÷÷ 
discount
÷÷ 
)
÷÷  
;
÷÷  !

ActiveDate
◊◊ 
=
◊◊ 
DateTime
◊◊ 
.
◊◊ 
UtcNow
◊◊ $
;
◊◊$ %
IncrementVersion
ÿÿ 
(
ÿÿ 
)
ÿÿ 
;
ÿÿ 
CalculateTotals
ŸŸ 
(
ŸŸ 
)
ŸŸ 
;
ŸŸ 
}
⁄⁄ 
public
‰‰ 

void
‰‰ 
ApplyDiscount
‰‰ 
(
‰‰ 
Discount
‰‰ &
discount
‰‰' /
,
‰‰/ 0
UserId
‰‰1 7
	appliedBy
‰‰8 A
,
‰‰A B
UserId
‰‰C I
?
‰‰I J
authorizedBy
‰‰K W
=
‰‰X Y
null
‰‰Z ^
)
‰‰^ _
{
ÂÂ 
if
ÊÊ 

(
ÊÊ 
discount
ÊÊ 
==
ÊÊ 
null
ÊÊ 
)
ÊÊ 
{
ÁÁ 	
throw
ËË 
new
ËË #
ArgumentNullException
ËË +
(
ËË+ ,
nameof
ËË, 2
(
ËË2 3
discount
ËË3 ;
)
ËË; <
)
ËË< =
;
ËË= >
}
ÈÈ 	
if
ÎÎ 

(
ÎÎ 
	appliedBy
ÎÎ 
==
ÎÎ 
null
ÎÎ 
)
ÎÎ 
{
ÏÏ 	
throw
ÌÌ 
new
ÌÌ #
ArgumentNullException
ÌÌ +
(
ÌÌ+ ,
nameof
ÌÌ, 2
(
ÌÌ2 3
	appliedBy
ÌÌ3 <
)
ÌÌ< =
)
ÌÌ= >
;
ÌÌ> ?
}
ÓÓ 	
if
 

(
 
Status
 
==
 
TicketStatus
 "
.
" #
Closed
# )
||
* ,
Status
- 3
==
4 6
TicketStatus
7 C
.
C D
Voided
D J
||
K M
Status
N T
==
U W
TicketStatus
X d
.
d e
Refunded
e m
)
m n
{
ÒÒ 	
throw
ÚÚ 
new
ÚÚ -
DomainInvalidOperationException
ÚÚ 5
(
ÚÚ5 6
$"
ÚÚ6 8
$str
ÚÚ8 [
{
ÚÚ[ \
Status
ÚÚ\ b
}
ÚÚb c
$str
ÚÚc k
"
ÚÚk l
)
ÚÚl m
;
ÚÚm n
}
ÛÛ 	
if
ıı 

(
ıı 
!
ıı 
discount
ıı 
.
ıı 
IsActive
ıı 
)
ıı 
{
ˆˆ 	
throw
˜˜ 
new
˜˜ ,
BusinessRuleViolationException
˜˜ 4
(
˜˜4 5
$str
˜˜5 V
)
˜˜V W
;
˜˜W X
}
¯¯ 	
var
˚˚ 
discountAmount
˚˚ 
=
˚˚ 
discount
˚˚ %
.
˚˚% &
CalculateDiscount
˚˚& 7
(
˚˚7 8
SubtotalAmount
˚˚8 F
)
˚˚F G
;
˚˚G H
var
˛˛ 
newTotal
˛˛ 
=
˛˛ 
TotalAmount
˛˛ "
-
˛˛# $
discountAmount
˛˛% 3
;
˛˛3 4
if
ˇˇ 

(
ˇˇ 
newTotal
ˇˇ 
<
ˇˇ 
Money
ˇˇ 
.
ˇˇ 
Zero
ˇˇ !
(
ˇˇ! "
)
ˇˇ" #
)
ˇˇ# $
{
ÄÄ 	
throw
ÅÅ 
new
ÅÅ ,
BusinessRuleViolationException
ÅÅ 4
(
ÅÅ4 5
$str
ÅÅ5 _
)
ÅÅ_ `
;
ÅÅ` a
}
ÇÇ 	
var
ÖÖ  
discountPercentage
ÖÖ 
=
ÖÖ  
SubtotalAmount
ÖÖ! /
.
ÖÖ/ 0
Amount
ÖÖ0 6
>
ÖÖ7 8
$num
ÖÖ9 :
?
ÜÜ 
(
ÜÜ 
discountAmount
ÜÜ 
.
ÜÜ 
Amount
ÜÜ $
/
ÜÜ% &
SubtotalAmount
ÜÜ' 5
.
ÜÜ5 6
Amount
ÜÜ6 <
)
ÜÜ< =
*
ÜÜ> ?
$num
ÜÜ@ D
:
áá 
$num
áá 
;
áá 
if
ââ 

(
ââ  
discountPercentage
ââ 
>
ââ  
$num
ââ! $
&&
ââ% '
authorizedBy
ââ( 4
==
ââ5 7
null
ââ8 <
)
ââ< =
{
ää 	
throw
ãã 
new
ãã ,
BusinessRuleViolationException
ãã 4
(
ãã4 5
$str
ãã5 p
)
ããp q
;
ããq r
}
åå 	
var
èè 
ticketDiscount
èè 
=
èè 
TicketDiscount
èè +
.
èè+ ,
Create
èè, 2
(
èè2 3
ticketId
êê 
:
êê 
Id
êê 
,
êê 

discountId
ëë 
:
ëë 
discount
ëë  
.
ëë  !
Id
ëë! #
,
ëë# $
name
íí 
:
íí 
discount
íí 
.
íí 
Name
íí 
,
íí  
type
ìì 
:
ìì 
discount
ìì 
.
ìì 
Type
ìì 
,
ìì  
value
îî 
:
îî 
discount
îî 
.
îî 
Value
îî !
,
îî! "
amount
ïï 
:
ïï 
discountAmount
ïï "
,
ïï" #
	appliedBy
ññ 
:
ññ 
	appliedBy
ññ  
,
ññ  !
authorizedBy
óó 
:
óó 
authorizedBy
óó &
,
óó& '
minimumAmount
òò 
:
òò 
discount
òò #
.
òò# $

MinimumBuy
òò$ .
)
ôô 	
;
ôô	 


_discounts
õõ 
.
õõ 
Add
õõ 
(
õõ 
ticketDiscount
õõ %
)
õõ% &
;
õõ& '

ActiveDate
úú 
=
úú 
DateTime
úú 
.
úú 
UtcNow
úú $
;
úú$ %
IncrementVersion
ùù 
(
ùù 
)
ùù 
;
ùù 
CalculateTotals
ûû 
(
ûû 
)
ûû 
;
ûû 
}
°° 
public
©© 

void
©© 
RemoveDiscount
©© 
(
©© 
Guid
©© #

discountId
©©$ .
)
©©. /
{
™™ 
if
´´ 

(
´´ 
Status
´´ 
==
´´ 
TicketStatus
´´ "
.
´´" #
Closed
´´# )
||
´´* ,
Status
´´- 3
==
´´4 6
TicketStatus
´´7 C
.
´´C D
Voided
´´D J
||
´´K M
Status
´´N T
==
´´U W
TicketStatus
´´X d
.
´´d e
Refunded
´´e m
)
´´m n
{
¨¨ 	
throw
≠≠ 
new
≠≠ -
DomainInvalidOperationException
≠≠ 5
(
≠≠5 6
$"
≠≠6 8
$str
≠≠8 ^
{
≠≠^ _
Status
≠≠_ e
}
≠≠e f
$str
≠≠f n
"
≠≠n o
)
≠≠o p
;
≠≠p q
}
ÆÆ 	
var
∞∞ 
discount
∞∞ 
=
∞∞ 

_discounts
∞∞ !
.
∞∞! "
FirstOrDefault
∞∞" 0
(
∞∞0 1
d
∞∞1 2
=>
∞∞3 5
d
∞∞6 7
.
∞∞7 8
Id
∞∞8 :
==
∞∞; =

discountId
∞∞> H
)
∞∞H I
;
∞∞I J
if
±± 

(
±± 
discount
±± 
==
±± 
null
±± 
)
±± 
{
≤≤ 	
throw
≥≥ 
new
≥≥ ,
BusinessRuleViolationException
≥≥ 4
(
≥≥4 5
$"
≥≥5 7
$str
≥≥7 @
{
≥≥@ A

discountId
≥≥A K
}
≥≥K L
$str
≥≥L f
"
≥≥f g
)
≥≥g h
;
≥≥h i
}
¥¥ 	

_discounts
∂∂ 
.
∂∂ 
Remove
∂∂ 
(
∂∂ 
discount
∂∂ "
)
∂∂" #
;
∂∂# $

ActiveDate
∑∑ 
=
∑∑ 
DateTime
∑∑ 
.
∑∑ 
UtcNow
∑∑ $
;
∑∑$ %
IncrementVersion
∏∏ 
(
∏∏ 
)
∏∏ 
;
∏∏ 
CalculateTotals
ππ 
(
ππ 
)
ππ 
;
ππ 
}
ºº 
public
¡¡ 

void
¡¡ 
ApplyLineDiscount
¡¡ !
(
¡¡! "
Guid
¡¡" &
orderLineId
¡¡' 2
,
¡¡2 3
OrderLineDiscount
¡¡4 E
discount
¡¡F N
)
¡¡N O
{
¬¬ 
if
√√ 

(
√√ 
discount
√√ 
==
√√ 
null
√√ 
)
√√ 
throw
√√ #
new
√√$ '#
ArgumentNullException
√√( =
(
√√= >
nameof
√√> D
(
√√D E
discount
√√E M
)
√√M N
)
√√N O
;
√√O P
if
≈≈ 

(
≈≈ 
Status
≈≈ 
==
≈≈ 
TicketStatus
≈≈ "
.
≈≈" #
Closed
≈≈# )
||
≈≈* ,
Status
≈≈- 3
==
≈≈4 6
TicketStatus
≈≈7 C
.
≈≈C D
Voided
≈≈D J
||
≈≈K M
Status
≈≈N T
==
≈≈U W
TicketStatus
≈≈X d
.
≈≈d e
Refunded
≈≈e m
)
≈≈m n
{
∆∆ 	
throw
«« 
new
«« -
DomainInvalidOperationException
«« 5
(
««5 6
$"
««6 8
$str
««8 [
{
««[ \
Status
««\ b
}
««b c
$str
««c k
"
««k l
)
««l m
;
««m n
}
»» 	
var
   
line
   
=
   
_orderLines
   
.
   
FirstOrDefault
   -
(
  - .
x
  . /
=>
  0 2
x
  3 4
.
  4 5
Id
  5 7
==
  8 :
orderLineId
  ; F
)
  F G
;
  G H
if
ÀÀ 

(
ÀÀ 
line
ÀÀ 
==
ÀÀ 
null
ÀÀ 
)
ÀÀ 
throw
ÃÃ 
new
ÃÃ ,
BusinessRuleViolationException
ÃÃ 4
(
ÃÃ4 5
$"
ÃÃ5 7
$str
ÃÃ7 A
{
ÃÃA B
orderLineId
ÃÃB M
}
ÃÃM N
$str
ÃÃN h
"
ÃÃh i
)
ÃÃi j
;
ÃÃj k
line
ŒŒ 
.
ŒŒ 
ApplyDiscount
ŒŒ 
(
ŒŒ 
discount
ŒŒ #
)
ŒŒ# $
;
ŒŒ$ %

ActiveDate
œœ 
=
œœ 
DateTime
œœ 
.
œœ 
UtcNow
œœ $
;
œœ$ %
IncrementVersion
–– 
(
–– 
)
–– 
;
–– 
CalculateTotals
—— 
(
—— 
)
—— 
;
—— 
}
““ 
public
◊◊ 

void
◊◊ 
Schedule
◊◊ 
(
◊◊ 
DateTime
◊◊ !
deliveryDate
◊◊" .
)
◊◊. /
{
ÿÿ 
if
ŸŸ 

(
ŸŸ 
deliveryDate
ŸŸ 
<=
ŸŸ 
DateTime
ŸŸ $
.
ŸŸ$ %
UtcNow
ŸŸ% +
)
ŸŸ+ ,
{
⁄⁄ 	
throw
€€ 
new
€€ ,
BusinessRuleViolationException
€€ 4
(
€€4 5
$str
€€5 g
)
€€g h
;
€€h i
}
‹‹ 	
if
ﬁﬁ 

(
ﬁﬁ 
Status
ﬁﬁ 
!=
ﬁﬁ 
TicketStatus
ﬁﬁ "
.
ﬁﬁ" #
Draft
ﬁﬁ# (
&&
ﬁﬁ) +
Status
ﬁﬁ, 2
!=
ﬁﬁ3 5
TicketStatus
ﬁﬁ6 B
.
ﬁﬁB C
Open
ﬁﬁC G
)
ﬁﬁG H
{
ﬂﬂ 	
throw
‡‡ 
new
‡‡ -
DomainInvalidOperationException
‡‡ 6
(
‡‡6 7
$"
‡‡7 9
$str
‡‡9 S
{
‡‡S T
Status
‡‡T Z
}
‡‡Z [
$str
‡‡[ c
"
‡‡c d
)
‡‡d e
;
‡‡e f
}
·· 	
DeliveryDate
„„ 
=
„„ 
deliveryDate
„„ #
;
„„# $
Status
‰‰ 
=
‰‰ 
TicketStatus
‰‰ 
.
‰‰ 
	Scheduled
‰‰ '
;
‰‰' (

ActiveDate
ÂÂ 
=
ÂÂ 
DateTime
ÂÂ 
.
ÂÂ 
UtcNow
ÂÂ $
;
ÂÂ$ %
IncrementVersion
ÊÊ 
(
ÊÊ 
)
ÊÊ 
;
ÊÊ 
}
ÁÁ 
public
ÏÏ 

void
ÏÏ 
Fire
ÏÏ 
(
ÏÏ 
)
ÏÏ 
{
ÌÌ 
if
ÓÓ 

(
ÓÓ 
Status
ÓÓ 
!=
ÓÓ 
TicketStatus
ÓÓ "
.
ÓÓ" #
	Scheduled
ÓÓ# ,
)
ÓÓ, -
{
ÔÔ 	
throw
 
new
 -
DomainInvalidOperationException
 5
(
5 6
$"
6 8
$str
8 o
{
o p
Status
p v
}
v w
$str
w x
"
x y
)
y z
;
z {
}
ÒÒ 	
Status
ÛÛ 
=
ÛÛ 
TicketStatus
ÛÛ 
.
ÛÛ 
Open
ÛÛ "
;
ÛÛ" #

ActiveDate
ÙÙ 
=
ÙÙ 
DateTime
ÙÙ 
.
ÙÙ 
UtcNow
ÙÙ $
;
ÙÙ$ %
IncrementVersion
ˆˆ 
(
ˆˆ 
)
ˆˆ 
;
ˆˆ 
}
˜˜ 
public
¸¸ 

void
¸¸ 
ChangeOrderType
¸¸ 
(
¸¸  
	OrderType
¸¸  )
	orderType
¸¸* 3
)
¸¸3 4
{
˝˝ 
if
˛˛ 

(
˛˛ 
	orderType
˛˛ 
==
˛˛ 
null
˛˛ 
)
˛˛ 
throw
˛˛ $
new
˛˛% (#
ArgumentNullException
˛˛) >
(
˛˛> ?
nameof
˛˛? E
(
˛˛E F
	orderType
˛˛F O
)
˛˛O P
)
˛˛P Q
;
˛˛Q R
if
ˇˇ 

(
ˇˇ 
	orderType
ˇˇ 
.
ˇˇ 
Id
ˇˇ 
==
ˇˇ 
OrderTypeId
ˇˇ '
)
ˇˇ' (
return
ˇˇ) /
;
ˇˇ/ 0
if
ÇÇ 

(
ÇÇ 
	orderType
ÇÇ 
.
ÇÇ 
Name
ÇÇ 
.
ÇÇ 
Contains
ÇÇ #
(
ÇÇ# $
$str
ÇÇ$ .
,
ÇÇ. /
StringComparison
ÇÇ0 @
.
ÇÇ@ A
OrdinalIgnoreCase
ÇÇA R
)
ÇÇR S
)
ÇÇS T
{
ÉÉ 	
if
ÑÑ 
(
ÑÑ 

CustomerId
ÑÑ 
==
ÑÑ 
null
ÑÑ #
)
ÑÑ# $
throw
ÖÖ 
new
ÖÖ ,
BusinessRuleViolationException
ÖÖ 9
(
ÖÖ9 :
$str
ÖÖ: _
)
ÖÖ_ `
;
ÖÖ` a
if
ÜÜ 
(
ÜÜ 
string
ÜÜ 
.
ÜÜ  
IsNullOrWhiteSpace
ÜÜ *
(
ÜÜ* +
DeliveryAddress
ÜÜ+ :
)
ÜÜ: ;
)
ÜÜ; <
throw
áá 
new
áá ,
BusinessRuleViolationException
áá 9
(
áá9 :
$str
áá: g
)
áág h
;
ááh i
}
àà 	
OrderTypeId
ää 
=
ää 
	orderType
ää 
.
ää  
Id
ää  "
;
ää" #
IsBarTab
ãã 
=
ãã 
	orderType
ãã 
.
ãã 
IsBarTab
ãã %
;
ãã% &

ActiveDate
åå 
=
åå 
DateTime
åå 
.
åå 
UtcNow
åå $
;
åå$ %
IncrementVersion
çç 
(
çç 
)
çç 
;
çç 
}
éé 
public
ìì 

void
ìì 
SetCustomer
ìì 
(
ìì 
Guid
ìì  
?
ìì  !

customerId
ìì" ,
,
ìì, -
string
ìì. 4
?
ìì4 5
address
ìì6 =
=
ìì> ?
null
ìì@ D
,
ììD E
string
ììF L
?
ììL M
	extraInfo
ììN W
=
ììX Y
null
ììZ ^
)
ìì^ _
{
îî 

CustomerId
ïï 
=
ïï 

customerId
ïï 
;
ïï  
DeliveryAddress
ññ 
=
ññ 
address
ññ !
;
ññ! "
ExtraDeliveryInfo
óó 
=
óó 
	extraInfo
óó %
;
óó% &

ActiveDate
òò 
=
òò 
DateTime
òò 
.
òò 
UtcNow
òò $
;
òò$ %
IncrementVersion
ôô 
(
ôô 
)
ôô 
;
ôô 
if
úú 

(
úú 

CustomerId
úú 
==
úú 
null
úú 
&&
úú !
!
úú" #
string
úú# )
.
úú) * 
IsNullOrWhiteSpace
úú* <
(
úú< =
address
úú= D
)
úúD E
)
úúE F
{
ùù 	
}
†† 	
}
°° 
public
®® 

void
®® 
CalculateTotals
®® 
(
®®  
)
®®  !
{
©© 
SubtotalAmount
´´ 
=
´´ 
_orderLines
´´ $
.
´´$ %
	Aggregate
´´% .
(
´´. /
Money
¨¨ 
.
¨¨ 
Zero
¨¨ 
(
¨¨ 
)
¨¨ 
,
¨¨ 
(
≠≠ 
sum
≠≠ 
,
≠≠ 
line
≠≠ 
)
≠≠ 
=>
≠≠ 
sum
≠≠ 
+
≠≠  
line
≠≠! %
.
≠≠% &
TotalAmount
≠≠& 1
)
≠≠1 2
;
≠≠2 3
	TaxAmount
±± 
=
±± 
IsTaxExempt
±± 
?
≤≤ 
Money
≤≤ 
.
≤≤ 
Zero
≤≤ 
(
≤≤ 
)
≤≤ 
:
≥≥ 
SubtotalAmount
≥≥ 
*
≥≥ 
$num
≥≥ $
;
≥≥$ %
DiscountAmount
∂∂ 
=
∂∂ 

_discounts
∂∂ #
.
∂∂# $
	Aggregate
∂∂$ -
(
∂∂- .
Money
∑∑ 
.
∑∑ 
Zero
∑∑ 
(
∑∑ 
)
∑∑ 
,
∑∑ 
(
∏∏ 
sum
∏∏ 
,
∏∏ 
d
∏∏ 
)
∏∏ 
=>
∏∏ 
sum
∏∏ 
+
∏∏ 
d
∏∏ 
.
∏∏  
Amount
∏∏  &
)
∏∏& '
;
∏∏' (
if
ΩΩ 

(
ΩΩ 
PriceIncludesTax
ΩΩ 
)
ΩΩ 
{
ææ 	
TotalAmount
¿¿ 
=
¿¿ 
SubtotalAmount
¿¿ (
+
¡¡ !
ServiceChargeAmount
¡¡ %
+
¬¬ "
DeliveryChargeAmount
¬¬ &
+
√√ 
AdjustmentAmount
√√ "
-
ƒƒ 
DiscountAmount
ƒƒ  
;
ƒƒ  !
}
≈≈ 	
else
∆∆ 
{
«« 	
TotalAmount
…… 
=
…… 
SubtotalAmount
…… (
+
   
	TaxAmount
   
+
ÀÀ !
ServiceChargeAmount
ÀÀ %
+
ÃÃ "
DeliveryChargeAmount
ÃÃ &
+
ÕÕ 
AdjustmentAmount
ÕÕ "
-
ŒŒ 
DiscountAmount
ŒŒ  
;
ŒŒ  !
}
œœ 	
if
““ 

(
““ 
Gratuity
““ 
!=
““ 
null
““ 
)
““ 
{
”” 	
TotalAmount
‘‘ 
=
‘‘ 
TotalAmount
‘‘ %
+
‘‘& '
Gratuity
‘‘( 0
.
‘‘0 1
Amount
‘‘1 7
;
‘‘7 8
}
’’ 	#
RecalculatePaidAmount
ÿÿ 
(
ÿÿ 
)
ÿÿ 
;
ÿÿ  
if
€€ 

(
€€ 

PaidAmount
€€ 
>=
€€ 
TotalAmount
€€ %
)
€€% &
{
‹‹ 	
	DueAmount
›› 
=
›› 
Money
›› 
.
›› 
Zero
›› #
(
››# $
TotalAmount
››$ /
.
››/ 0
Currency
››0 8
)
››8 9
;
››9 :
}
ﬁﬁ 	
else
ﬂﬂ 
{
‡‡ 	
	DueAmount
·· 
=
·· 
TotalAmount
·· $
-
··% &

PaidAmount
··' 1
;
··1 2
}
‚‚ 	
}
„„ 
internal
ÈÈ 
void
ÈÈ $
CalculateTotalsWithTax
ÈÈ (
(
ÈÈ( )
Money
ÈÈ) .
	taxAmount
ÈÈ/ 8
)
ÈÈ8 9
{
ÍÍ 
SubtotalAmount
ÏÏ 
=
ÏÏ 
_orderLines
ÏÏ $
.
ÏÏ$ %
	Aggregate
ÏÏ% .
(
ÏÏ. /
Money
ÌÌ 
.
ÌÌ 
Zero
ÌÌ 
(
ÌÌ 
)
ÌÌ 
,
ÌÌ 
(
ÓÓ 
sum
ÓÓ 
,
ÓÓ 
line
ÓÓ 
)
ÓÓ 
=>
ÓÓ 
sum
ÓÓ 
+
ÓÓ  
line
ÓÓ! %
.
ÓÓ% &
TotalAmount
ÓÓ& 1
)
ÓÓ1 2
;
ÓÓ2 3
	TaxAmount
ÒÒ 
=
ÒÒ 
	taxAmount
ÒÒ 
;
ÒÒ 
DiscountAmount
ÙÙ 
=
ÙÙ 

_discounts
ÙÙ #
.
ÙÙ# $
	Aggregate
ÙÙ$ -
(
ÙÙ- .
Money
ıı 
.
ıı 
Zero
ıı 
(
ıı 
)
ıı 
,
ıı 
(
ˆˆ 
sum
ˆˆ 
,
ˆˆ 
d
ˆˆ 
)
ˆˆ 
=>
ˆˆ 
sum
ˆˆ 
+
ˆˆ 
d
ˆˆ 
.
ˆˆ  
Amount
ˆˆ  &
)
ˆˆ& '
;
ˆˆ' (
if
˚˚ 

(
˚˚ 
PriceIncludesTax
˚˚ 
)
˚˚ 
{
¸¸ 	
TotalAmount
˛˛ 
=
˛˛ 
SubtotalAmount
˛˛ (
+
ˇˇ !
ServiceChargeAmount
ˇˇ %
+
ÄÄ "
DeliveryChargeAmount
ÄÄ &
+
ÅÅ 
AdjustmentAmount
ÅÅ "
-
ÇÇ 
DiscountAmount
ÇÇ  
;
ÇÇ  !
}
ÉÉ 	
else
ÑÑ 
{
ÖÖ 	
TotalAmount
áá 
=
áá 
SubtotalAmount
áá (
+
àà 
	TaxAmount
àà 
+
ââ !
ServiceChargeAmount
ââ %
+
ää "
DeliveryChargeAmount
ää &
+
ãã 
AdjustmentAmount
ãã "
-
åå 
DiscountAmount
åå  
;
åå  !
}
çç 	
if
êê 

(
êê 
Gratuity
êê 
!=
êê 
null
êê 
)
êê 
{
ëë 	
TotalAmount
íí 
=
íí 
TotalAmount
íí %
+
íí& '
Gratuity
íí( 0
.
íí0 1
Amount
íí1 7
;
íí7 8
}
ìì 	#
RecalculatePaidAmount
ññ 
(
ññ 
)
ññ 
;
ññ  
	DueAmount
óó 
=
óó 
TotalAmount
óó 
-
óó  !

PaidAmount
óó" ,
;
óó, -
}
òò 
private
ûû 
void
ûû #
RecalculatePaidAmount
ûû &
(
ûû& '
)
ûû' (
{
üü 
var
†† 
validPayments
†† 
=
†† 
	_payments
†† %
.
††% &
Where
††& +
(
††+ ,
p
††, -
=>
††. 0
!
††1 2
p
††2 3
.
††3 4
IsVoided
††4 <
)
††< =
.
††= >
ToList
††> D
(
††D E
)
††E F
;
††F G
var
¢¢ 
totalCredits
¢¢ 
=
¢¢ 
validPayments
¢¢ (
.
££ 
Where
££ 
(
££ 
p
££ 
=>
££ 
p
££ 
.
££ 
TransactionType
££ )
==
££* ,
TransactionType
££- <
.
££< =
Credit
££= C
)
££C D
.
§§ 
	Aggregate
§§ 
(
§§ 
Money
§§ 
.
§§ 
Zero
§§ !
(
§§! "
)
§§" #
,
§§# $
(
§§% &
sum
§§& )
,
§§) *
p
§§+ ,
)
§§, -
=>
§§. 0
sum
§§1 4
+
§§5 6
p
§§7 8
.
§§8 9
Amount
§§9 ?
)
§§? @
;
§§@ A
var
¶¶ 
totalDebits
¶¶ 
=
¶¶ 
validPayments
¶¶ '
.
ßß 
Where
ßß 
(
ßß 
p
ßß 
=>
ßß 
p
ßß 
.
ßß 
TransactionType
ßß )
==
ßß* ,
TransactionType
ßß- <
.
ßß< =
Debit
ßß= B
)
ßßB C
.
®® 
	Aggregate
®® 
(
®® 
Money
®® 
.
®® 
Zero
®® !
(
®®! "
)
®®" #
,
®®# $
(
®®% &
sum
®®& )
,
®®) *
p
®®+ ,
)
®®, -
=>
®®. 0
sum
®®1 4
+
®®5 6
p
®®7 8
.
®®8 9
Amount
®®9 ?
)
®®? @
;
®®@ A
if
´´ 

(
´´ 
totalDebits
´´ 
>
´´ 
totalCredits
´´ &
)
´´& '
{
¨¨ 	

PaidAmount
ØØ 
=
ØØ 
Money
ØØ 
.
ØØ  
Zero
ØØ  $
(
ØØ$ %
totalCredits
ØØ% 1
.
ØØ1 2
Currency
ØØ2 :
)
ØØ: ;
;
ØØ; <
}
∞∞ 	
else
±± 
{
≤≤ 	

PaidAmount
≥≥ 
=
≥≥ 
totalCredits
≥≥ &
-
≥≥' (
totalDebits
≥≥) 4
;
≥≥4 5
}
¥¥ 	
}
µµ 
public
∫∫ 

void
∫∫ 
AddTableNumber
∫∫ 
(
∫∫ 
int
∫∫ "
tableNumber
∫∫# .
)
∫∫. /
{
ªª 
if
ºº 

(
ºº 
tableNumber
ºº 
<=
ºº 
$num
ºº 
)
ºº 
{
ΩΩ 	
throw
ææ 
new
ææ ,
BusinessRuleViolationException
ææ 4
(
ææ4 5
$str
ææ5 ^
)
ææ^ _
;
ææ_ `
}
øø 	
if
¡¡ 

(
¡¡ 
!
¡¡ 
_tableNumbers
¡¡ 
.
¡¡ 
Contains
¡¡ #
(
¡¡# $
tableNumber
¡¡$ /
)
¡¡/ 0
)
¡¡0 1
{
¬¬ 	
_tableNumbers
√√ 
.
√√ 
Add
√√ 
(
√√ 
tableNumber
√√ )
)
√√) *
;
√√* +
IncrementVersion
ƒƒ 
(
ƒƒ 
)
ƒƒ 
;
ƒƒ 
}
≈≈ 	
}
∆∆ 
public
ÀÀ 

void
ÀÀ 
RemoveTableNumber
ÀÀ !
(
ÀÀ! "
int
ÀÀ" %
tableNumber
ÀÀ& 1
)
ÀÀ1 2
{
ÃÃ 
if
ÕÕ 

(
ÕÕ 
_tableNumbers
ÕÕ 
.
ÕÕ 
Remove
ÕÕ  
(
ÕÕ  !
tableNumber
ÕÕ! ,
)
ÕÕ, -
)
ÕÕ- .
{
ŒŒ 	
IncrementVersion
œœ 
(
œœ 
)
œœ 
;
œœ 
}
–– 	
}
—— 
public
÷÷ 

void
÷÷ 
AssignTable
÷÷ 
(
÷÷ 
int
÷÷ 
tableNumber
÷÷  +
)
÷÷+ ,
{
◊◊ 
if
ÿÿ 

(
ÿÿ 
tableNumber
ÿÿ 
<=
ÿÿ 
$num
ÿÿ 
)
ÿÿ 
{
ŸŸ 	
throw
⁄⁄ 
new
⁄⁄ ,
BusinessRuleViolationException
⁄⁄ 4
(
⁄⁄4 5
$str
⁄⁄5 ^
)
⁄⁄^ _
;
⁄⁄_ `
}
€€ 	
if
›› 

(
›› 
_tableNumbers
›› 
.
›› 
Count
›› 
==
››  "
$num
››# $
&&
››% '
_tableNumbers
››( 5
[
››5 6
$num
››6 7
]
››7 8
==
››9 ;
tableNumber
››< G
)
››G H
{
ﬁﬁ 	
return
ﬂﬂ 
;
ﬂﬂ 
}
‡‡ 	
_tableNumbers
‚‚ 
.
‚‚ 
Clear
‚‚ 
(
‚‚ 
)
‚‚ 
;
‚‚ 
_tableNumbers
„„ 
.
„„ 
Add
„„ 
(
„„ 
tableNumber
„„ %
)
„„% &
;
„„& '
IncrementVersion
‰‰ 
(
‰‰ 
)
‰‰ 
;
‰‰ 
}
ÂÂ 
public
ÍÍ 

void
ÍÍ 
AddGratuity
ÍÍ 
(
ÍÍ 
Gratuity
ÍÍ $
gratuity
ÍÍ% -
)
ÍÍ- .
{
ÎÎ 
if
ÏÏ 

(
ÏÏ 
gratuity
ÏÏ 
==
ÏÏ 
null
ÏÏ 
)
ÏÏ 
{
ÌÌ 	
throw
ÓÓ 
new
ÓÓ #
ArgumentNullException
ÓÓ +
(
ÓÓ+ ,
nameof
ÓÓ, 2
(
ÓÓ2 3
gratuity
ÓÓ3 ;
)
ÓÓ; <
)
ÓÓ< =
;
ÓÓ= >
}
ÔÔ 	
if
ÒÒ 

(
ÒÒ 
gratuity
ÒÒ 
.
ÒÒ 
TicketId
ÒÒ 
!=
ÒÒ  
Id
ÒÒ! #
)
ÒÒ# $
{
ÚÚ 	
throw
ÛÛ 
new
ÛÛ ,
BusinessRuleViolationException
ÛÛ 4
(
ÛÛ4 5
$str
ÛÛ5 _
)
ÛÛ_ `
;
ÛÛ` a
}
ÙÙ 	
Gratuity
ˆˆ 
=
ˆˆ 
gratuity
ˆˆ 
;
ˆˆ 
IncrementVersion
˜˜ 
(
˜˜ 
)
˜˜ 
;
˜˜ 
CalculateTotals
¯¯ 
(
¯¯ 
)
¯¯ 
;
¯¯ 
}
˘˘ 
public
˛˛ 

void
˛˛  
MarkGratuityAsPaid
˛˛ "
(
˛˛" #
)
˛˛# $
{
ˇˇ 
if
ÄÄ 

(
ÄÄ 
Gratuity
ÄÄ 
==
ÄÄ 
null
ÄÄ 
)
ÄÄ 
{
ÅÅ 	
throw
ÇÇ 
new
ÇÇ -
DomainInvalidOperationException
ÇÇ 5
(
ÇÇ5 6
$str
ÇÇ6 T
)
ÇÇT U
;
ÇÇU V
}
ÉÉ 	
Gratuity
ÖÖ 
.
ÖÖ 

MarkAsPaid
ÖÖ 
(
ÖÖ 
)
ÖÖ 
;
ÖÖ 
CalculateTotals
ÜÜ 
(
ÜÜ 
)
ÜÜ 
;
ÜÜ 
}
áá 
public
åå 

void
åå $
MarkGratuityAsRefunded
åå &
(
åå& '
)
åå' (
{
çç 
if
éé 

(
éé 
Gratuity
éé 
==
éé 
null
éé 
)
éé 
{
èè 	
throw
êê 
new
êê -
DomainInvalidOperationException
êê 5
(
êê5 6
$str
êê6 X
)
êêX Y
;
êêY Z
}
ëë 	
Gratuity
ìì 
.
ìì 
MarkAsRefunded
ìì 
(
ìì  
)
ìì  !
;
ìì! "
CalculateTotals
îî 
(
îî 
)
îî 
;
îî 
}
ïï 
public
öö 

void
öö 

SetSession
öö 
(
öö 
Guid
öö 
	sessionId
öö  )
)
öö) *
{
õõ 
if
úú 

(
úú 
	sessionId
úú 
==
úú 
Guid
úú 
.
úú 
Empty
úú #
)
úú# $
{
ùù 	
throw
ûû 
new
ûû 
ArgumentException
ûû (
(
ûû( )
$str
ûû) F
,
ûûF G
nameof
ûûH N
(
ûûN O
	sessionId
ûûO X
)
ûûX Y
)
ûûY Z
;
ûûZ [
}
üü 	
if
°° 

(
°° 
Status
°° 
==
°° 
TicketStatus
°° "
.
°°" #
Closed
°°# )
||
°°* ,
Status
°°- 3
==
°°4 6
TicketStatus
°°7 C
.
°°C D
Voided
°°D J
||
°°K M
Status
°°N T
==
°°U W
TicketStatus
°°X d
.
°°d e
Refunded
°°e m
)
°°m n
{
¢¢ 	
throw
££ 
new
££ -
DomainInvalidOperationException
££ 5
(
££5 6
$"
££6 8
$str
££8 Y
{
££Y Z
Status
££Z `
}
££` a
$str
££a i
"
££i j
)
££j k
;
££k l
}
§§ 	
	SessionId
¶¶ 
=
¶¶ 
	sessionId
¶¶ 
;
¶¶ 

ActiveDate
ßß 
=
ßß 
DateTime
ßß 
.
ßß 
UtcNow
ßß $
;
ßß$ %
IncrementVersion
®® 
(
®® 
)
®® 
;
®® 
}
©© 
public
ØØ 

void
ØØ 
SetServiceCharge
ØØ  
(
ØØ  !
Money
ØØ! &
amount
ØØ' -
)
ØØ- .
{
∞∞ 
if
±± 

(
±± 
amount
±± 
<
±± 
Money
±± 
.
±± 
Zero
±± 
(
±±  
)
±±  !
)
±±! "
{
≤≤ 	
throw
≥≥ 
new
≥≥ ,
BusinessRuleViolationException
≥≥ 4
(
≥≥4 5
$str
≥≥5 Y
)
≥≥Y Z
;
≥≥Z [
}
¥¥ 	
if
∂∂ 

(
∂∂ 
Status
∂∂ 
==
∂∂ 
TicketStatus
∂∂ "
.
∂∂" #
Closed
∂∂# )
||
∂∂* ,
Status
∂∂- 3
==
∂∂4 6
TicketStatus
∂∂7 C
.
∂∂C D
Voided
∂∂D J
||
∂∂K M
Status
∂∂N T
==
∂∂U W
TicketStatus
∂∂X d
.
∂∂d e
Refunded
∂∂e m
)
∂∂m n
{
∑∑ 	
throw
∏∏ 
new
∏∏ -
DomainInvalidOperationException
∏∏ 5
(
∏∏5 6
$"
∏∏6 8
$str
∏∏8 b
{
∏∏b c
Status
∏∏c i
}
∏∏i j
$str
∏∏j r
"
∏∏r s
)
∏∏s t
;
∏∏t u
}
ππ 	!
ServiceChargeAmount
ªª 
=
ªª 
amount
ªª $
;
ªª$ %

ActiveDate
ºº 
=
ºº 
DateTime
ºº 
.
ºº 
UtcNow
ºº $
;
ºº$ %
CalculateTotals
ΩΩ 
(
ΩΩ 
)
ΩΩ 
;
ΩΩ 
}
ææ 
public
√√ 

void
√√ 
SetDeliveryCharge
√√ !
(
√√! "
Money
√√" '
amount
√√( .
)
√√. /
{
ƒƒ 
if
≈≈ 

(
≈≈ 
amount
≈≈ 
<
≈≈ 
Money
≈≈ 
.
≈≈ 
Zero
≈≈ 
(
≈≈  
)
≈≈  !
)
≈≈! "
{
∆∆ 	
throw
«« 
new
«« ,
BusinessRuleViolationException
«« 4
(
««4 5
$str
««5 Z
)
««Z [
;
««[ \
}
»» 	
if
   

(
   
Status
   
==
   
TicketStatus
   "
.
  " #
Closed
  # )
||
  * ,
Status
  - 3
==
  4 6
TicketStatus
  7 C
.
  C D
Voided
  D J
||
  K M
Status
  N T
==
  U W
TicketStatus
  X d
.
  d e
Refunded
  e m
)
  m n
{
ÀÀ 	
throw
ÃÃ 
new
ÃÃ -
DomainInvalidOperationException
ÃÃ 5
(
ÃÃ5 6
$"
ÃÃ6 8
$str
ÃÃ8 c
{
ÃÃc d
Status
ÃÃd j
}
ÃÃj k
$str
ÃÃk s
"
ÃÃs t
)
ÃÃt u
;
ÃÃu v
}
ÕÕ 	"
DeliveryChargeAmount
œœ 
=
œœ 
amount
œœ %
;
œœ% &

ActiveDate
–– 
=
–– 
DateTime
–– 
.
–– 
UtcNow
–– $
;
––$ %
CalculateTotals
—— 
(
—— 
)
—— 
;
—— 
}
““ 
public
ÿÿ 

void
ÿÿ 
SetTaxExempt
ÿÿ 
(
ÿÿ 
bool
ÿÿ !
isTaxExempt
ÿÿ" -
)
ÿÿ- .
{
ŸŸ 
if
⁄⁄ 

(
⁄⁄ 
Status
⁄⁄ 
==
⁄⁄ 
TicketStatus
⁄⁄ "
.
⁄⁄" #
Closed
⁄⁄# )
||
⁄⁄* ,
Status
⁄⁄- 3
==
⁄⁄4 6
TicketStatus
⁄⁄7 C
.
⁄⁄C D
Voided
⁄⁄D J
||
⁄⁄K M
Status
⁄⁄N T
==
⁄⁄U W
TicketStatus
⁄⁄X d
.
⁄⁄d e
Refunded
⁄⁄e m
)
⁄⁄m n
{
€€ 	
throw
‹‹ 
new
‹‹ -
DomainInvalidOperationException
‹‹ 5
(
‹‹5 6
$"
‹‹6 8
$str
‹‹8 e
{
‹‹e f
Status
‹‹f l
}
‹‹l m
$str
‹‹m u
"
‹‹u v
)
‹‹v w
;
‹‹w x
}
›› 	
IsTaxExempt
ﬂﬂ 
=
ﬂﬂ 
isTaxExempt
ﬂﬂ !
;
ﬂﬂ! "

ActiveDate
‡‡ 
=
‡‡ 
DateTime
‡‡ 
.
‡‡ 
UtcNow
‡‡ $
;
‡‡$ %
CalculateTotals
·· 
(
·· 
)
·· 
;
·· 
}
‚‚ 
public
ÁÁ 

void
ÁÁ 
SetNote
ÁÁ 
(
ÁÁ 
string
ÁÁ 
?
ÁÁ 
note
ÁÁ  $
)
ÁÁ$ %
{
ËË 
if
ÈÈ 

(
ÈÈ 
Status
ÈÈ 
==
ÈÈ 
TicketStatus
ÈÈ "
.
ÈÈ" #
Closed
ÈÈ# )
||
ÈÈ* ,
Status
ÈÈ- 3
==
ÈÈ4 6
TicketStatus
ÈÈ7 C
.
ÈÈC D
Voided
ÈÈD J
||
ÈÈK M
Status
ÈÈN T
==
ÈÈU W
TicketStatus
ÈÈX d
.
ÈÈd e
Refunded
ÈÈe m
)
ÈÈm n
{
ÍÍ 	
throw
ÎÎ 
new
ÎÎ -
DomainInvalidOperationException
ÎÎ 6
(
ÎÎ6 7
$"
ÎÎ7 9
$str
ÎÎ9 Y
{
ÎÎY Z
Status
ÎÎZ `
}
ÎÎ` a
$str
ÎÎa i
"
ÎÎi j
)
ÎÎj k
;
ÎÎk l
}
ÏÏ 	
Note
ÓÓ 
=
ÓÓ 
note
ÓÓ 
;
ÓÓ 

ActiveDate
ÔÔ 
=
ÔÔ 
DateTime
ÔÔ 
.
ÔÔ 
UtcNow
ÔÔ $
;
ÔÔ$ %
IncrementVersion
 
(
 
)
 
;
 
}
ÒÒ 
public
¯¯ 

void
¯¯ 
SetNumberOfGuests
¯¯ !
(
¯¯! "
int
¯¯" %
numberOfGuests
¯¯& 4
)
¯¯4 5
{
˘˘ 
if
˙˙ 

(
˙˙ 
numberOfGuests
˙˙ 
<
˙˙ 
$num
˙˙ 
)
˙˙ 
{
˚˚ 	
throw
¸¸ 
new
¸¸ ,
BusinessRuleViolationException
¸¸ 5
(
¸¸5 6
$str
¸¸6 \
)
¸¸\ ]
;
¸¸] ^
}
˝˝ 	
NumberOfGuests
ÉÉ 
=
ÉÉ 
numberOfGuests
ÉÉ '
;
ÉÉ' (

ActiveDate
ÑÑ 
=
ÑÑ 
DateTime
ÑÑ 
.
ÑÑ 
UtcNow
ÑÑ $
;
ÑÑ$ %
IncrementVersion
ÖÖ 
(
ÖÖ 
)
ÖÖ 
;
ÖÖ 
}
ÜÜ 
public
èè 

void
èè 
SetAdjustment
èè 
(
èè 
Money
èè #
amount
èè$ *
)
èè* +
{
êê 
if
ëë 

(
ëë 
amount
ëë 
<
ëë 
Money
ëë 
.
ëë 
Zero
ëë 
(
ëë  
)
ëë  !
)
ëë! "
{
íí 	
throw
ìì 
new
ìì ,
BusinessRuleViolationException
ìì 4
(
ìì4 5
$strìì5 Ä
)ììÄ Å
;ììÅ Ç
}
îî 	
if
ññ 

(
ññ 
Status
ññ 
==
ññ 
TicketStatus
ññ "
.
ññ" #
Closed
ññ# )
||
ññ* ,
Status
ññ- 3
==
ññ4 6
TicketStatus
ññ7 C
.
ññC D
Voided
ññD J
||
ññK M
Status
ññN T
==
ññU W
TicketStatus
ññX d
.
ññd e
Refunded
ññe m
)
ññm n
{
óó 	
throw
òò 
new
òò -
DomainInvalidOperationException
òò 5
(
òò5 6
$"
òò6 8
$str
òò8 ^
{
òò^ _
Status
òò_ e
}
òòe f
$str
òòf n
"
òòn o
)
òòo p
;
òòp q
}
ôô 	
AdjustmentAmount
õõ 
=
õõ 
amount
õõ !
;
õõ! "

ActiveDate
úú 
=
úú 
DateTime
úú 
.
úú 
UtcNow
úú $
;
úú$ %
CalculateTotals
ùù 
(
ùù 
)
ùù 
;
ùù 
}
ûû 
public
££ 

void
££ 
SetAdvancePayment
££ !
(
££! "
Money
££" '
amount
££( .
)
££. /
{
§§ 
if
•• 

(
•• 
amount
•• 
<
•• 
Money
•• 
.
•• 
Zero
•• 
(
••  
)
••  !
)
••! "
{
¶¶ 	
throw
ßß 
new
ßß ,
BusinessRuleViolationException
ßß 4
(
ßß4 5
$str
ßß5 Z
)
ßßZ [
;
ßß[ \
}
®® 	
if
™™ 

(
™™ 
Status
™™ 
==
™™ 
TicketStatus
™™ "
.
™™" #
Closed
™™# )
||
™™* ,
Status
™™- 3
==
™™4 6
TicketStatus
™™7 C
.
™™C D
Voided
™™D J
||
™™K M
Status
™™N T
==
™™U W
TicketStatus
™™X d
.
™™d e
Refunded
™™e m
)
™™m n
{
´´ 	
throw
¨¨ 
new
¨¨ -
DomainInvalidOperationException
¨¨ 5
(
¨¨5 6
$"
¨¨6 8
$str
¨¨8 c
{
¨¨c d
Status
¨¨d j
}
¨¨j k
$str
¨¨k s
"
¨¨s t
)
¨¨t u
;
¨¨u v
}
≠≠ 	
AdvanceAmount
ØØ 
=
ØØ 
amount
ØØ 
;
ØØ 

ActiveDate
∞∞ 
=
∞∞ 
DateTime
∞∞ 
.
∞∞ 
UtcNow
∞∞ $
;
∞∞$ %
CalculateTotals
±± 
(
±± 
)
±± 
;
±± 
}
≤≤ 
public
∑∑ 

void
∑∑ 
MarkAsReady
∑∑ 
(
∑∑ 
)
∑∑ 
{
∏∏ 
if
ππ 

(
ππ 
Status
ππ 
==
ππ 
TicketStatus
ππ "
.
ππ" #
Closed
ππ# )
||
ππ* ,
Status
ππ- 3
==
ππ4 6
TicketStatus
ππ7 C
.
ππC D
Voided
ππD J
||
ππK M
Status
ππN T
==
ππU W
TicketStatus
ππX d
.
ππd e
Refunded
ππe m
)
ππm n
{
∫∫ 	
throw
ªª 
new
ªª -
DomainInvalidOperationException
ªª 5
(
ªª5 6
$"
ªª6 8
$str
ªª8 W
{
ªªW X
Status
ªªX ^
}
ªª^ _
$str
ªª_ g
"
ªªg h
)
ªªh i
;
ªªi j
}
ºº 	
	ReadyTime
ææ 
=
ææ 
DateTime
ææ 
.
ææ 
UtcNow
ææ #
;
ææ# $

ActiveDate
øø 
=
øø 
DateTime
øø 
.
øø 
UtcNow
øø $
;
øø$ %
IncrementVersion
¿¿ 
(
¿¿ 
)
¿¿ 
;
¿¿ 
}
¡¡ 
public
∆∆ 

void
∆∆ 
MarkAsDispatched
∆∆  
(
∆∆  !
Guid
∆∆! %
?
∆∆% &
driverId
∆∆' /
)
∆∆/ 0
{
«« 
if
»» 

(
»» 
Status
»» 
==
»» 
TicketStatus
»» "
.
»»" #
Closed
»»# )
||
»»* ,
Status
»»- 3
==
»»4 6
TicketStatus
»»7 C
.
»»C D
Voided
»»D J
||
»»K M
Status
»»N T
==
»»U W
TicketStatus
»»X d
.
»»d e
Refunded
»»e m
)
»»m n
{
…… 	
throw
   
new
   -
DomainInvalidOperationException
   5
(
  5 6
$"
  6 8
$str
  8 \
{
  \ ]
Status
  ] c
}
  c d
$str
  d l
"
  l m
)
  m n
;
  n o
}
ÀÀ 	
if
ÕÕ 

(
ÕÕ  
CustomerWillPickup
ÕÕ 
)
ÕÕ 
{
ŒŒ 	
throw
œœ 
new
œœ -
DomainInvalidOperationException
œœ 6
(
œœ6 7
$str
œœ7 Y
)
œœY Z
;
œœZ [
}
–– 	
DispatchedTime
““ 
=
““ 
DateTime
““ !
.
““! "
UtcNow
““" (
;
““( )
AssignedDriverId
”” 
=
”” 
driverId
”” #
;
””# $

ActiveDate
‘‘ 
=
‘‘ 
DateTime
‘‘ 
.
‘‘ 
UtcNow
‘‘ $
;
‘‘$ %
IncrementVersion
’’ 
(
’’ 
)
’’ 
;
’’ 
}
÷÷ 
public
€€ 

void
€€ 
Transfer
€€ 
(
€€ 
UserId
€€ 
newOwner
€€  (
)
€€( )
{
‹‹ 
if
›› 

(
›› 
Status
›› 
==
›› 
TicketStatus
›› "
.
››" #
Closed
››# )
||
››* ,
Status
››- 3
==
››4 6
TicketStatus
››7 C
.
››C D
Voided
››D J
||
››K M
Status
››N T
==
››U W
TicketStatus
››X d
.
››d e
Refunded
››e m
)
››m n
{
ﬁﬁ 	
throw
ﬂﬂ 
new
ﬂﬂ -
DomainInvalidOperationException
ﬂﬂ 5
(
ﬂﬂ5 6
$"
ﬂﬂ6 8
$str
ﬂﬂ8 R
{
ﬂﬂR S
Status
ﬂﬂS Y
}
ﬂﬂY Z
$str
ﬂﬂZ b
"
ﬂﬂb c
)
ﬂﬂc d
;
ﬂﬂd e
}
‡‡ 	
if
‚‚ 

(
‚‚ 
newOwner
‚‚ 
==
‚‚ 
null
‚‚ 
)
‚‚ 
{
„„ 	
throw
‰‰ 
new
‰‰ #
ArgumentNullException
‰‰ +
(
‰‰+ ,
nameof
‰‰, 2
(
‰‰2 3
newOwner
‰‰3 ;
)
‰‰; <
)
‰‰< =
;
‰‰= >
}
ÂÂ 	
	CreatedBy
ÁÁ 
=
ÁÁ 
newOwner
ÁÁ 
;
ÁÁ 

ActiveDate
ËË 
=
ËË 
DateTime
ËË 
.
ËË 
UtcNow
ËË $
;
ËË$ %
IncrementVersion
ÈÈ 
(
ÈÈ 
)
ÈÈ 
;
ÈÈ 
}
ÍÍ 
public
ÙÙ 

void
ÙÙ 
Hold
ÙÙ 
(
ÙÙ 
string
ÙÙ 
reason
ÙÙ "
,
ÙÙ" #
UserId
ÙÙ$ *
userId
ÙÙ+ 1
)
ÙÙ1 2
{
ıı 
if
ˆˆ 

(
ˆˆ 
Status
ˆˆ 
==
ˆˆ 
TicketStatus
ˆˆ "
.
ˆˆ" #
Closed
ˆˆ# )
)
ˆˆ) *
{
˜˜ 	
throw
¯¯ 
new
¯¯ -
DomainInvalidOperationException
¯¯ 5
(
¯¯5 6
$str
¯¯6 T
)
¯¯T U
;
¯¯U V
}
˘˘ 	
if
˚˚ 

(
˚˚ 
Status
˚˚ 
==
˚˚ 
TicketStatus
˚˚ "
.
˚˚" #
Voided
˚˚# )
)
˚˚) *
{
¸¸ 	
throw
˝˝ 
new
˝˝ -
DomainInvalidOperationException
˝˝ 5
(
˝˝5 6
$str
˝˝6 T
)
˝˝T U
;
˝˝U V
}
˛˛ 	
if
Ä	Ä	 

(
Ä	Ä	 
Status
Ä	Ä	 
==
Ä	Ä	 
TicketStatus
Ä	Ä	 "
.
Ä	Ä	" #
Refunded
Ä	Ä	# +
)
Ä	Ä	+ ,
{
Å	Å	 	
throw
Ç	Ç	 
new
Ç	Ç	 -
DomainInvalidOperationException
Ç	Ç	 5
(
Ç	Ç	5 6
$str
Ç	Ç	6 V
)
Ç	Ç	V W
;
Ç	Ç	W X
}
É	É	 	
if
Ö	Ö	 

(
Ö	Ö	 
Status
Ö	Ö	 
==
Ö	Ö	 
TicketStatus
Ö	Ö	 "
.
Ö	Ö	" #
Held
Ö	Ö	# '
)
Ö	Ö	' (
{
Ü	Ü	 	
throw
á	á	 
new
á	á	 -
DomainInvalidOperationException
á	á	 5
(
á	á	5 6
$str
á	á	6 O
)
á	á	O P
;
á	á	P Q
}
à	à	 	
if
ä	ä	 

(
ä	ä	 
string
ä	ä	 
.
ä	ä	  
IsNullOrWhiteSpace
ä	ä	 %
(
ä	ä	% &
reason
ä	ä	& ,
)
ä	ä	, -
)
ä	ä	- .
{
ã	ã	 	
throw
å	å	 
new
å	å	 
ArgumentException
å	å	 '
(
å	å	' (
$str
å	å	( B
,
å	å	B C
nameof
å	å	D J
(
å	å	J K
reason
å	å	K Q
)
å	å	Q R
)
å	å	R S
;
å	å	S T
}
ç	ç	 	
if
è	è	 

(
è	è	 
userId
è	è	 
==
è	è	 
null
è	è	 
)
è	è	 
{
ê	ê	 	
throw
ë	ë	 
new
ë	ë	 #
ArgumentNullException
ë	ë	 +
(
ë	ë	+ ,
nameof
ë	ë	, 2
(
ë	ë	2 3
userId
ë	ë	3 9
)
ë	ë	9 :
)
ë	ë	: ;
;
ë	ë	; <
}
í	í	 	
Status
î	î	 
=
î	î	 
TicketStatus
î	î	 
.
î	î	 
Held
î	î	 "
;
î	î	" #
HeldAt
ï	ï	 
=
ï	ï	 
DateTime
ï	ï	 
.
ï	ï	 
UtcNow
ï	ï	  
;
ï	ï	  !

HoldReason
ñ	ñ	 
=
ñ	ñ	 
reason
ñ	ñ	 
;
ñ	ñ	 
HeldBy
ó	ó	 
=
ó	ó	 
userId
ó	ó	 
;
ó	ó	 

ActiveDate
ò	ò	 
=
ò	ò	 
DateTime
ò	ò	 
.
ò	ò	 
UtcNow
ò	ò	 $
;
ò	ò	$ %
IncrementVersion
ô	ô	 
(
ô	ô	 
)
ô	ô	 
;
ô	ô	 
}
ö	ö	 
public
†	†	 

void
†	†	 
Release
†	†	 
(
†	†	 
)
†	†	 
{
°	°	 
if
¢	¢	 

(
¢	¢	 
Status
¢	¢	 
!=
¢	¢	 
TicketStatus
¢	¢	 "
.
¢	¢	" #
Held
¢	¢	# '
)
¢	¢	' (
{
£	£	 	
throw
§	§	 
new
§	§	 -
DomainInvalidOperationException
§	§	 5
(
§	§	5 6
$str
§	§	6 Z
)
§	§	Z [
;
§	§	[ \
}
•	•	 	
Status
ß	ß	 
=
ß	ß	 
TicketStatus
ß	ß	 
.
ß	ß	 
Open
ß	ß	 "
;
ß	ß	" #

ActiveDate
®	®	 
=
®	®	 
DateTime
®	®	 
.
®	®	 
UtcNow
®	®	 $
;
®	®	$ %
IncrementVersion
©	©	 
(
©	©	 
)
©	©	 
;
©	©	 
}
™	™	 
private
≤	≤	 
void
≤	≤	 
IncrementVersion
≤	≤	 !
(
≤	≤	! "
)
≤	≤	" #
{
≥	≥	 
Version
¥	¥	 
++
¥	¥	 
;
¥	¥	 
}
µ	µ	 
}∂	∂	 Ü
uC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\TerminalTransaction.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
TerminalTransaction  
{ 
public		 

Guid		 
Id		 
{		 
get		 
;		 
private		 !
set		" %
;		% &
}		' (
public

 

Guid

 
CashSessionId

 
{

 
get

  #
;

# $
private

% ,
set

- 0
;

0 1
}

2 3
public 
#
TerminalTransactionType "
Type# '
{( )
get* -
;- .
private/ 6
set7 :
;: ;
}< =
public 

Money 
Amount 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

string 
	Reference 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
=2 3
string4 :
.: ;
Empty; @
;@ A
public 

DateTime 
	Timestamp 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
	protected 
TerminalTransaction !
(! "
)" #
{ 
Amount 
= 
Money 
. 
Zero 
( 
) 
; 
} 
public 

TerminalTransaction 
( 
Guid #
cashSessionId$ 1
,1 2#
TerminalTransactionType3 J
typeK O
,O P
MoneyQ V
amountW ]
,] ^
string_ e
	referencef o
)o p
{ 
Id 

= 
Guid 
. 
NewGuid 
( 
) 
; 
CashSessionId 
= 
cashSessionId %
;% &
Type 
= 
type 
; 
Amount 
= 
amount 
; 
	Reference 
= 
	reference 
??  
string! '
.' (
Empty( -
;- .
	Timestamp   
=   
DateTime   
.   
UtcNow   #
;  # $
}!! 
}"" ¬'
jC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Terminal.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
Terminal		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public 

string 
TerminalKey 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
string6 <
.< =
Empty= B
;B C
public 

string 
Location 
{ 
get  
;  !
set" %
;% &
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
public 

Guid 
? 
FloorId 
{ 
get 
; 
set  #
;# $
}% &
public 

bool 
HasCashDrawer 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
true. 2
;2 3
public 

decimal 
OpeningBalance !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 

decimal 
CurrentBalance !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 

bool 

AutoLogOut 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
AutoLogOutTime 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
$num. 0
;0 1
public 

bool 
ShowGuestSelection "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
true3 7
;7 8
public 

bool 
ShowTableSelection "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
true3 7
;7 8
public 

bool 
KitchenMode 
{ 
get !
;! "
set# &
;& '
}( )
public 

string 
DefaultFontSize !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
$str2 6
;6 7
public 

string 
DefaultFontFamily #
{$ %
get& )
;) *
set+ .
;. /
}0 1
=2 3
$str4 >
;> ?
private   
Terminal   
(   
)   
{!! 
}"" 
public'' 

static'' 
Terminal'' 
Create'' !
(''! "
string''" (
name'') -
,''- .
string''/ 5
terminalKey''6 A
)''A B
{(( 
if)) 

()) 
string)) 
.)) 
IsNullOrWhiteSpace)) %
())% &
terminalKey))& 1
)))1 2
)))2 3
{** 	
throw++ 
new++ 
ArgumentException++ '
(++' (
$str++( C
)++C D
;++D E
},, 	
return.. 
new.. 
Terminal.. 
{// 	
Id00 
=00 
Guid00 
.00 
NewGuid00 
(00 
)00 
,00  
Name11 
=11 
name11 
??11 
terminalKey11 &
,11& '
TerminalKey22 
=22 
terminalKey22 %
,22% &
Location33 
=33 
$str33  
,33  !
HasCashDrawer44 
=44 
true44  
,44  !
OpeningBalance55 
=55 
$num55 
,55 
CurrentBalance66 
=66 
$num66 
,66 

AutoLogOut77 
=77 
false77 
,77 
AutoLogOutTime88 
=88 
$num88 
,88  
ShowGuestSelection99 
=99  
true99! %
,99% &
ShowTableSelection:: 
=::  
true::! %
,::% &
KitchenMode;; 
=;; 
false;; 
}<< 	
;<<	 

}== 
}>> õe
kC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\TableType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
	TableType 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

string 
Description 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
string6 <
.< =
Empty= B
;B C
public 

decimal 

HourlyRate 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

decimal 
? 
FirstHourRate !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 

int 
MinimumMinutes 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

int 
RoundingMinutes 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

Money 
MinimumCharge 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
=5 6
Money7 <
.< =
Zero= A
(A B
)B C
;C D
public 

TimeRoundingRule 
RoundingRule (
{) *
get+ .
;. /
private0 7
set8 ;
;; <
}= >
=? @
TimeRoundingRuleA Q
.Q R
NoneR V
;V W
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
private 
	TableType 
( 
) 
{ 
} 
public(( 

static(( 
	TableType(( 
Create(( "
(((" #
string)) 
name)) 
,)) 
decimal** 

hourlyRate** 
,** 
string++ 
description++ 
=++ 
$str++ 
)++  
{,, 
if-- 

(-- 
string-- 
.-- 
IsNullOrWhiteSpace-- %
(--% &
name--& *
)--* +
)--+ ,
{.. 	
throw// 
new// 
ArgumentException// '
(//' (
$str//( J
,//J K
nameof//L R
(//R S
name//S W
)//W X
)//X Y
;//Y Z
}00 	
if22 

(22 

hourlyRate22 
<=22 
$num22 
)22 
{33 	
throw44 
new44 
ArgumentException44 '
(44' (
$str44( P
,44P Q
nameof44R X
(44X Y

hourlyRate44Y c
)44c d
)44d e
;44e f
}55 	
var77 
now77 
=77 
DateTime77 
.77 
UtcNow77 !
;77! "
return99 
new99 
	TableType99 
{:: 	
Id;; 
=;; 
Guid;; 
.;; 
NewGuid;; 
(;; 
);; 
,;;  
Name<< 
=<< 
name<< 
.<< 
Trim<< 
(<< 
)<< 
,<< 
Description== 
=== 
description== %
?==% &
.==& '
Trim==' +
(==+ ,
)==, -
??==. 0
string==1 7
.==7 8
Empty==8 =
,=== >

HourlyRate>> 
=>> 

hourlyRate>> #
,>># $
FirstHourRate?? 
=?? 
null??  
,??  !
MinimumMinutes@@ 
=@@ 
$num@@ 
,@@ 
RoundingMinutesAA 
=AA 
$numAA 
,AA  
MinimumChargeBB 
=BB 
MoneyBB !
.BB! "
ZeroBB" &
(BB& '
)BB' (
,BB( )
RoundingRuleCC 
=CC 
TimeRoundingRuleCC +
.CC+ ,
NoneCC, 0
,CC0 1
IsActiveDD 
=DD 
trueDD 
,DD 
	CreatedAtEE 
=EE 
nowEE 
,EE 
	UpdatedAtFF 
=FF 
nowFF 
}GG 	
;GG	 

}HH 
publicPP 

voidPP 
UpdateRatesPP 
(PP 
decimalPP #

hourlyRatePP$ .
,PP. /
decimalPP0 7
?PP7 8
firstHourRatePP9 F
=PPG H
nullPPI M
)PPM N
{QQ 
ifRR 

(RR 

hourlyRateRR 
<=RR 
$numRR 
)RR 
{SS 	
throwTT 
newTT 
ArgumentExceptionTT '
(TT' (
$strTT( P
,TTP Q
nameofTTR X
(TTX Y

hourlyRateTTY c
)TTc d
)TTd e
;TTe f
}UU 	
ifWW 

(WW 
firstHourRateWW 
.WW 
HasValueWW "
&&WW# %
firstHourRateWW& 3
.WW3 4
ValueWW4 9
<=WW: <
$numWW= >
)WW> ?
{XX 	
throwYY 
newYY 
ArgumentExceptionYY '
(YY' (
$strYY( a
,YYa b
nameofYYc i
(YYi j
firstHourRateYYj w
)YYw x
)YYx y
;YYy z
}ZZ 	

HourlyRate\\ 
=\\ 

hourlyRate\\ 
;\\  
FirstHourRate]] 
=]] 
firstHourRate]] %
;]]% &
	UpdatedAt^^ 
=^^ 
DateTime^^ 
.^^ 
UtcNow^^ #
;^^# $
}__ 
publicgg 

voidgg 
SetRoundinggg 
(gg 
intgg 
minimumMinutesgg  .
,gg. /
intgg0 3
roundingMinutesgg4 C
)ggC D
{hh 
ifii 

(ii 
minimumMinutesii 
<ii 
$numii 
)ii 
{jj 	
throwkk 
newkk 
ArgumentExceptionkk '
(kk' (
$strkk( M
,kkM N
nameofkkO U
(kkU V
minimumMinuteskkV d
)kkd e
)kke f
;kkf g
}ll 	
ifnn 

(nn 
roundingMinutesnn 
<nn 
$numnn 
)nn  
{oo 	
throwpp 
newpp 
ArgumentExceptionpp '
(pp' (
$strpp( N
,ppN O
nameofppP V
(ppV W
roundingMinutesppW f
)ppf g
)ppg h
;pph i
}qq 	
MinimumMinutesss 
=ss 
minimumMinutesss '
;ss' (
RoundingMinutestt 
=tt 
roundingMinutestt )
;tt) *
	UpdatedAtuu 
=uu 
DateTimeuu 
.uu 
UtcNowuu #
;uu# $
}vv 
public~~ 

void~~ 
UpdateDetails~~ 
(~~ 
string~~ $
name~~% )
,~~) *
string~~+ 1
description~~2 =
=~~> ?
$str~~@ B
)~~B C
{ 
if
ÄÄ 

(
ÄÄ 
string
ÄÄ 
.
ÄÄ  
IsNullOrWhiteSpace
ÄÄ %
(
ÄÄ% &
name
ÄÄ& *
)
ÄÄ* +
)
ÄÄ+ ,
{
ÅÅ 	
throw
ÇÇ 
new
ÇÇ 
ArgumentException
ÇÇ '
(
ÇÇ' (
$str
ÇÇ( J
,
ÇÇJ K
nameof
ÇÇL R
(
ÇÇR S
name
ÇÇS W
)
ÇÇW X
)
ÇÇX Y
;
ÇÇY Z
}
ÉÉ 	
Name
ÖÖ 
=
ÖÖ 
name
ÖÖ 
.
ÖÖ 
Trim
ÖÖ 
(
ÖÖ 
)
ÖÖ 
;
ÖÖ 
Description
ÜÜ 
=
ÜÜ 
description
ÜÜ !
?
ÜÜ! "
.
ÜÜ" #
Trim
ÜÜ# '
(
ÜÜ' (
)
ÜÜ( )
??
ÜÜ* ,
string
ÜÜ- 3
.
ÜÜ3 4
Empty
ÜÜ4 9
;
ÜÜ9 :
	UpdatedAt
áá 
=
áá 
DateTime
áá 
.
áá 
UtcNow
áá #
;
áá# $
}
àà 
public
çç 

void
çç 

Deactivate
çç 
(
çç 
)
çç 
{
éé 
IsActive
èè 
=
èè 
false
èè 
;
èè 
	UpdatedAt
êê 
=
êê 
DateTime
êê 
.
êê 
UtcNow
êê #
;
êê# $
}
ëë 
public
ññ 

void
ññ 
Activate
ññ 
(
ññ 
)
ññ 
{
óó 
IsActive
òò 
=
òò 
true
òò 
;
òò 
	UpdatedAt
ôô 
=
ôô 
DateTime
ôô 
.
ôô 
UtcNow
ôô #
;
ôô# $
}
öö 
public
°° 

void
°° 
SetMinimumCharge
°°  
(
°°  !
Money
°°! &
minimumCharge
°°' 4
)
°°4 5
{
¢¢ 
MinimumCharge
££ 
=
££ 
minimumCharge
££ %
??
££& (
throw
££) .
new
££/ 2#
ArgumentNullException
££3 H
(
££H I
nameof
££I O
(
££O P
minimumCharge
££P ]
)
££] ^
)
££^ _
;
££_ `
	UpdatedAt
§§ 
=
§§ 
DateTime
§§ 
.
§§ 
UtcNow
§§ #
;
§§# $
}
•• 
public
´´ 

void
´´ 
SetRoundingRule
´´ 
(
´´  
TimeRoundingRule
´´  0
roundingRule
´´1 =
)
´´= >
{
¨¨ 
RoundingRule
≠≠ 
=
≠≠ 
roundingRule
≠≠ #
;
≠≠# $
RoundingMinutes
∞∞ 
=
∞∞ 
roundingRule
∞∞ &
switch
∞∞' -
{
±± 	
TimeRoundingRule
≤≤ 
.
≤≤ 
None
≤≤ !
=>
≤≤" $
$num
≤≤% &
,
≤≤& '
TimeRoundingRule
≥≥ 
.
≥≥ 
FifteenMinutes
≥≥ +
=>
≥≥, .
$num
≥≥/ 1
,
≥≥1 2
TimeRoundingRule
¥¥ 
.
¥¥ 
ThirtyMinutes
¥¥ *
=>
¥¥+ -
$num
¥¥. 0
,
¥¥0 1
TimeRoundingRule
µµ 
.
µµ 
SixtyMinutes
µµ )
=>
µµ* ,
$num
µµ- /
,
µµ/ 0
_
∂∂ 
=>
∂∂ 
$num
∂∂ 
}
∑∑ 	
;
∑∑	 

	UpdatedAt
ππ 
=
ππ 
DateTime
ππ 
.
ππ 
UtcNow
ππ #
;
ππ# $
}
∫∫ 
public
¿¿ 

bool
¿¿ *
ValidatePricingConfiguration
¿¿ ,
(
¿¿, -
)
¿¿- .
{
¡¡ 
if
√√ 

(
√√ 
FirstHourRate
√√ 
.
√√ 
HasValue
√√ "
&&
√√# %
MinimumCharge
√√& 3
.
√√3 4
Amount
√√4 :
>
√√; <
$num
√√= >
)
√√> ?
{
ƒƒ 	
var
≈≈ 
firstHourMoney
≈≈ 
=
≈≈  
new
≈≈! $
Money
≈≈% *
(
≈≈* +
FirstHourRate
≈≈+ 8
.
≈≈8 9
Value
≈≈9 >
)
≈≈> ?
;
≈≈? @
if
∆∆ 
(
∆∆ 
firstHourMoney
∆∆ 
<
∆∆  
MinimumCharge
∆∆! .
)
∆∆. /
{
«« 
return
»» 
false
»» 
;
»» 
}
…… 
}
   	
if
ÕÕ 

(
ÕÕ 

HourlyRate
ÕÕ 
<=
ÕÕ 
$num
ÕÕ 
)
ÕÕ 
{
ŒŒ 	
return
œœ 
false
œœ 
;
œœ 
}
–– 	
return
““ 
true
““ 
;
““ 
}
”” 
}‘‘ ¬g
lC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\TableShape.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 

TableShape

 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

TableShapeType 
	ShapeType #
{$ %
get& )
;) *
private+ 2
set3 6
;6 7
}8 9
public 

int 
Width 
{ 
get 
; 
private #
set$ '
;' (
}) *
=+ ,
$num- 0
;0 1
public 

int 
Height 
{ 
get 
; 
private $
set% (
;( )
}* +
=, -
$num. 1
;1 2
public 

string 
BackgroundColor !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
=8 9
$str: C
;C D
public 

string 
BorderColor 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
$str6 ?
;? @
public 

int 
BorderThickness 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
=5 6
$num7 8
;8 9
public 

int 
CornerRadius 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
=2 3
$num4 5
;5 6
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
=/ 0
true1 5
;5 6
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
private 

TableShape 
( 
) 
{ 
} 
public!! 

static!! 

TableShape!! 
Create!! #
(!!# $
string!!$ *
name!!+ /
,!!/ 0
TableShapeType!!1 ?
	shapeType!!@ I
,!!I J
int!!K N
width!!O T
=!!U V
$num!!W Z
,!!Z [
int!!\ _
height!!` f
=!!g h
$num!!i l
)!!l m
{"" 
if## 

(## 
string## 
.## 
IsNullOrWhiteSpace## %
(##% &
name##& *
)##* +
)##+ ,
{$$ 	
throw%% 
new%% 

Exceptions%%  
.%%  !*
BusinessRuleViolationException%%! ?
(%%? @
$str%%@ ]
)%%] ^
;%%^ _
}&& 	
if(( 

((( 
width(( 
<=(( 
$num(( 
||(( 
height((  
<=((! #
$num(($ %
)((% &
{)) 	
throw** 
new** 

Exceptions**  
.**  !*
BusinessRuleViolationException**! ?
(**? @
$str**@ m
)**m n
;**n o
}++ 	
var-- 
cornerRadius-- 
=-- 
	shapeType-- $
switch--% +
{.. 	
TableShapeType// 
.// 
	Rectangle// $
=>//% '
$num//( )
,//) *
TableShapeType00 
.00 
Square00 !
=>00" $
$num00% &
,00& '
TableShapeType11 
.11 
Round11  
=>11! #
width11$ )
/11* +
$num11, -
,11- .
TableShapeType22 
.22 
Oval22 
=>22  "
height22# )
/22* +
$num22, -
,22- .
_33 
=>33 
$num33 
}44 	
;44	 

return66 
new66 

TableShape66 
{77 	
Id88 
=88 
Guid88 
.88 
NewGuid88 
(88 
)88 
,88  
Name99 
=99 
name99 
,99 
	ShapeType:: 
=:: 
	shapeType:: !
,::! "
Width;; 
=;; 
width;; 
,;; 
Height<< 
=<< 
height<< 
,<< 
CornerRadius== 
=== 
cornerRadius== '
,==' (
	CreatedAt>> 
=>> 
DateTime>>  
.>>  !
UtcNow>>! '
,>>' (
	UpdatedAt?? 
=?? 
DateTime??  
.??  !
UtcNow??! '
}@@ 	
;@@	 

}AA 
publicFF 

voidFF 

UpdateNameFF 
(FF 
stringFF !
nameFF" &
)FF& '
{GG 
ifHH 

(HH 
stringHH 
.HH 
IsNullOrWhiteSpaceHH %
(HH% &
nameHH& *
)HH* +
)HH+ ,
{II 	
throwJJ 
newJJ 

ExceptionsJJ  
.JJ  !*
BusinessRuleViolationExceptionJJ! ?
(JJ? @
$strJJ@ ]
)JJ] ^
;JJ^ _
}KK 	
NameMM 
=MM 
nameMM 
;MM 
	UpdatedAtNN 
=NN 
DateTimeNN 
.NN 
UtcNowNN #
;NN# $
}OO 
publicTT 

voidTT 
UpdateDimensionsTT  
(TT  !
intTT! $
widthTT% *
,TT* +
intTT, /
heightTT0 6
)TT6 7
{UU 
ifVV 

(VV 
widthVV 
<=VV 
$numVV 
||VV 
heightVV  
<=VV! #
$numVV$ %
)VV% &
{WW 	
throwXX 
newXX 

ExceptionsXX  
.XX  !*
BusinessRuleViolationExceptionXX! ?
(XX? @
$strXX@ m
)XXm n
;XXn o
}YY 	
Width[[ 
=[[ 
width[[ 
;[[ 
Height\\ 
=\\ 
height\\ 
;\\ 
CornerRadius__ 
=__ 
	ShapeType__  
switch__! '
{`` 	
TableShapeTypeaa 
.aa 
	Rectangleaa $
=>aa% '
$numaa( )
,aa) *
TableShapeTypebb 
.bb 
Squarebb !
=>bb" $
$numbb% &
,bb& '
TableShapeTypecc 
.cc 
Roundcc  
=>cc! #
widthcc$ )
/cc* +
$numcc, -
,cc- .
TableShapeTypedd 
.dd 
Ovaldd 
=>dd  "
heightdd# )
/dd* +
$numdd, -
,dd- .
_ee 
=>ee 
$numee 
}ff 	
;ff	 

	UpdatedAthh 
=hh 
DateTimehh 
.hh 
UtcNowhh #
;hh# $
}ii 
publicnn 

voidnn 
UpdateColorsnn 
(nn 
stringnn #
backgroundColornn$ 3
,nn3 4
stringnn5 ;
borderColornn< G
)nnG H
{oo 
BackgroundColorpp 
=pp 
backgroundColorpp )
??pp* ,
$strpp- 6
;pp6 7
BorderColorqq 
=qq 
borderColorqq !
??qq" $
$strqq% .
;qq. /
	UpdatedAtrr 
=rr 
DateTimerr 
.rr 
UtcNowrr #
;rr# $
}ss 
publicxx 

voidxx !
UpdateBorderThicknessxx %
(xx% &
intxx& )
	thicknessxx* 3
)xx3 4
{yy 
ifzz 

(zz 
	thicknesszz 
<zz 
$numzz 
)zz 
{{{ 	
throw|| 
new|| 

Exceptions||  
.||  !*
BusinessRuleViolationException||! ?
(||? @
$str||@ f
)||f g
;||g h
}}} 	
BorderThickness 
= 
	thickness #
;# $
	UpdatedAt
ÄÄ 
=
ÄÄ 
DateTime
ÄÄ 
.
ÄÄ 
UtcNow
ÄÄ #
;
ÄÄ# $
}
ÅÅ 
public
ÜÜ 

void
ÜÜ 
Activate
ÜÜ 
(
ÜÜ 
)
ÜÜ 
{
áá 
IsActive
àà 
=
àà 
true
àà 
;
àà 
	UpdatedAt
ââ 
=
ââ 
DateTime
ââ 
.
ââ 
UtcNow
ââ #
;
ââ# $
}
ää 
public
èè 

void
èè 

Deactivate
èè 
(
èè 
)
èè 
{
êê 
IsActive
ëë 
=
ëë 
false
ëë 
;
ëë 
	UpdatedAt
íí 
=
íí 
DateTime
íí 
.
íí 
UtcNow
íí #
;
íí# $
}
ìì 
public
òò 

TableShapeDto
òò 
ToDto
òò 
(
òò 
)
òò  
{
ôô 
return
öö 
new
öö 
TableShapeDto
öö  
{
õõ 	
Id
úú 
=
úú 
Id
úú 
,
úú 
Name
ùù 
=
ùù 
Name
ùù 
,
ùù 
	ShapeType
ûû 
=
ûû 
	ShapeType
ûû !
,
ûû! "
Width
üü 
=
üü 
Width
üü 
,
üü 
Height
†† 
=
†† 
Height
†† 
,
†† 
BackgroundColor
°° 
=
°° 
BackgroundColor
°° -
,
°°- .
BorderColor
¢¢ 
=
¢¢ 
BorderColor
¢¢ %
,
¢¢% &
BorderThickness
££ 
=
££ 
BorderThickness
££ -
,
££- .
CornerRadius
§§ 
=
§§ 
CornerRadius
§§ '
,
§§' (
IsActive
•• 
=
•• 
IsActive
•• 
}
¶¶ 	
;
¶¶	 

}
ßß 
}®® 
public≠≠ 
class
≠≠ 
TableShapeDto
≠≠ 
{ÆÆ 
public
ØØ 

Guid
ØØ 
Id
ØØ 
{
ØØ 
get
ØØ 
;
ØØ 
set
ØØ 
;
ØØ 
}
ØØ  
public
∞∞ 

string
∞∞ 
Name
∞∞ 
{
∞∞ 
get
∞∞ 
;
∞∞ 
set
∞∞ !
;
∞∞! "
}
∞∞# $
=
∞∞% &
string
∞∞' -
.
∞∞- .
Empty
∞∞. 3
;
∞∞3 4
public
±± 

TableShapeType
±± 
	ShapeType
±± #
{
±±$ %
get
±±& )
;
±±) *
set
±±+ .
;
±±. /
}
±±0 1
public
≤≤ 

int
≤≤ 
Width
≤≤ 
{
≤≤ 
get
≤≤ 
;
≤≤ 
set
≤≤ 
;
≤≤  
}
≤≤! "
=
≤≤# $
$num
≤≤% (
;
≤≤( )
public
≥≥ 

int
≥≥ 
Height
≥≥ 
{
≥≥ 
get
≥≥ 
;
≥≥ 
set
≥≥  
;
≥≥  !
}
≥≥" #
=
≥≥$ %
$num
≥≥& )
;
≥≥) *
public
¥¥ 

string
¥¥ 
BackgroundColor
¥¥ !
{
¥¥" #
get
¥¥$ '
;
¥¥' (
set
¥¥) ,
;
¥¥, -
}
¥¥. /
=
¥¥0 1
$str
¥¥2 ;
;
¥¥; <
public
µµ 

string
µµ 
BorderColor
µµ 
{
µµ 
get
µµ  #
;
µµ# $
set
µµ% (
;
µµ( )
}
µµ* +
=
µµ, -
$str
µµ. 7
;
µµ7 8
public
∂∂ 

int
∂∂ 
BorderThickness
∂∂ 
{
∂∂  
get
∂∂! $
;
∂∂$ %
set
∂∂& )
;
∂∂) *
}
∂∂+ ,
=
∂∂- .
$num
∂∂/ 0
;
∂∂0 1
public
∑∑ 

int
∑∑ 
CornerRadius
∑∑ 
{
∑∑ 
get
∑∑ !
;
∑∑! "
set
∑∑# &
;
∑∑& '
}
∑∑( )
=
∑∑* +
$num
∑∑, -
;
∑∑- .
public
∏∏ 

bool
∏∏ 
IsActive
∏∏ 
{
∏∏ 
get
∏∏ 
;
∏∏ 
set
∏∏  #
;
∏∏# $
}
∏∏% &
=
∏∏' (
true
∏∏) -
;
∏∏- .
}ππ ﬁx
nC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\TableSession.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
TableSession 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 
TableId 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

Guid 
? 

CustomerId 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

Guid 
? 
TicketId 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

DateTime 
	StartTime 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
? 
EndTime 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

DateTime 
? 
PausedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

TimeSpan 
TotalPausedDuration '
{( )
get* -
;- .
private/ 6
set7 :
;: ;
}< =
public 

TableSessionStatus 
Status $
{% &
get' *
;* +
private, 3
set4 7
;7 8
}9 :
public 

Guid 
TableTypeId 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

decimal 

HourlyRate 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

Money 
TotalCharge 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
=3 4
Money5 :
.: ;
Zero; ?
(? @
)@ A
;A B
public 

int 

GuestCount 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
private 
TableSession 
( 
) 
{ 
}   
public-- 

static-- 
TableSession-- 
Start-- $
(--$ %
Guid.. 
tableId.. 
,.. 
Guid// 
tableTypeId// 
,// 
decimal00 

hourlyRate00 
,00 
int11 

guestCount11 
,11 
Guid22 
?22 

customerId22 
=22 
null22 
,22  
Guid33 
?33 
ticketId33 
=33 
null33 
)33 
{44 
if55 

(55 
tableId55 
==55 
Guid55 
.55 
Empty55 !
)55! "
{66 	
throw77 
new77 
ArgumentException77 '
(77' (
$str77( C
,77C D
nameof77E K
(77K L
tableId77L S
)77S T
)77T U
;77U V
}88 	
if:: 

(:: 
tableTypeId:: 
==:: 
Guid:: 
.::  
Empty::  %
)::% &
{;; 	
throw<< 
new<< 
ArgumentException<< '
(<<' (
$str<<( H
,<<H I
nameof<<J P
(<<P Q
tableTypeId<<Q \
)<<\ ]
)<<] ^
;<<^ _
}== 	
if?? 

(?? 

hourlyRate?? 
<=?? 
$num?? 
)?? 
{@@ 	
throwAA 
newAA 
ArgumentExceptionAA '
(AA' (
$strAA( P
,AAP Q
nameofAAR X
(AAX Y

hourlyRateAAY c
)AAc d
)AAd e
;AAe f
}BB 	
ifDD 

(DD 

guestCountDD 
<=DD 
$numDD 
)DD 
{EE 	
throwFF 
newFF 
ArgumentExceptionFF '
(FF' (
$strFF( P
,FFP Q
nameofFFR X
(FFX Y

guestCountFFY c
)FFc d
)FFd e
;FFe f
}GG 	
varII 
nowII 
=II 
DateTimeII 
.II 
UtcNowII !
;II! "
returnKK 
newKK 
TableSessionKK 
{LL 	
IdMM 
=MM 
GuidMM 
.MM 
NewGuidMM 
(MM 
)MM 
,MM  
TableIdNN 
=NN 
tableIdNN 
,NN 
TableTypeIdOO 
=OO 
tableTypeIdOO %
,OO% &

HourlyRatePP 
=PP 

hourlyRatePP #
,PP# $

GuestCountQQ 
=QQ 

guestCountQQ #
,QQ# $

CustomerIdRR 
=RR 

customerIdRR #
,RR# $
TicketIdSS 
=SS 
ticketIdSS 
,SS  
	StartTimeTT 
=TT 
nowTT 
,TT 
StatusUU 
=UU 
TableSessionStatusUU '
.UU' (
ActiveUU( .
,UU. /
TotalPausedDurationVV 
=VV  !
TimeSpanVV" *
.VV* +
ZeroVV+ /
,VV/ 0
TotalChargeWW 
=WW 
MoneyWW 
.WW  
ZeroWW  $
(WW$ %
)WW% &
,WW& '
	CreatedAtXX 
=XX 
nowXX 
,XX 
	UpdatedAtYY 
=YY 
nowYY 
}ZZ 	
;ZZ	 

}[[ 
publicaa 

voidaa 
Pauseaa 
(aa 
)aa 
{bb 
ifcc 

(cc 
Statuscc 
==cc 
TableSessionStatuscc (
.cc( )
Endedcc) .
)cc. /
{dd 	
throwee 
newee %
InvalidOperationExceptionee /
(ee/ 0
$stree0 P
)eeP Q
;eeQ R
}ff 	
ifhh 

(hh 
Statushh 
==hh 
TableSessionStatushh (
.hh( )
Pausedhh) /
)hh/ 0
{ii 	
throwjj 
newjj %
InvalidOperationExceptionjj /
(jj/ 0
$strjj0 L
)jjL M
;jjM N
}kk 	
PausedAtmm 
=mm 
DateTimemm 
.mm 
UtcNowmm "
;mm" #
Statusnn 
=nn 
TableSessionStatusnn #
.nn# $
Pausednn$ *
;nn* +
	UpdatedAtoo 
=oo 
DateTimeoo 
.oo 
UtcNowoo #
;oo# $
}pp 
publicvv 

voidvv 
Resumevv 
(vv 
)vv 
{ww 
ifxx 

(xx 
Statusxx 
!=xx 
TableSessionStatusxx (
.xx( )
Pausedxx) /
)xx/ 0
{yy 	
throwzz 
newzz %
InvalidOperationExceptionzz /
(zz/ 0
$strzz0 S
)zzS T
;zzT U
}{{ 	
if}} 

(}} 
!}} 
PausedAt}} 
.}} 
HasValue}} 
)}} 
{~~ 	
throw 
new %
InvalidOperationException /
(/ 0
$str0 U
)U V
;V W
}
ÄÄ 	
var
ÉÉ 
pauseDuration
ÉÉ 
=
ÉÉ 
DateTime
ÉÉ $
.
ÉÉ$ %
UtcNow
ÉÉ% +
-
ÉÉ, -
PausedAt
ÉÉ. 6
.
ÉÉ6 7
Value
ÉÉ7 <
;
ÉÉ< =!
TotalPausedDuration
ÑÑ 
+=
ÑÑ 
pauseDuration
ÑÑ ,
;
ÑÑ, -
PausedAt
ÜÜ 
=
ÜÜ 
null
ÜÜ 
;
ÜÜ 
Status
áá 
=
áá  
TableSessionStatus
áá #
.
áá# $
Active
áá$ *
;
áá* +
	UpdatedAt
àà 
=
àà 
DateTime
àà 
.
àà 
UtcNow
àà #
;
àà# $
}
ââ 
public
ëë 

void
ëë 
End
ëë 
(
ëë 
Money
ëë 
calculatedCharge
ëë *
)
ëë* +
{
íí 
if
ìì 

(
ìì 
calculatedCharge
ìì 
==
ìì 
null
ìì  $
)
ìì$ %
{
îî 	
throw
ïï 
new
ïï #
ArgumentNullException
ïï +
(
ïï+ ,
nameof
ïï, 2
(
ïï2 3
calculatedCharge
ïï3 C
)
ïïC D
)
ïïD E
;
ïïE F
}
ññ 	
if
òò 

(
òò 
Status
òò 
==
òò  
TableSessionStatus
òò (
.
òò( )
Ended
òò) .
)
òò. /
{
ôô 	
throw
öö 
new
öö '
InvalidOperationException
öö /
(
öö/ 0
$str
öö0 K
)
ööK L
;
ööL M
}
õõ 	
if
ùù 

(
ùù 
Status
ùù 
==
ùù  
TableSessionStatus
ùù (
.
ùù( )
Paused
ùù) /
)
ùù/ 0
{
ûû 	
throw
üü 
new
üü '
InvalidOperationException
üü /
(
üü/ 0
$str
üü0 _
)
üü_ `
;
üü` a
}
†† 	
EndTime
¢¢ 
=
¢¢ 
DateTime
¢¢ 
.
¢¢ 
UtcNow
¢¢ !
;
¢¢! "
TotalCharge
££ 
=
££ 
calculatedCharge
££ &
;
££& '
Status
§§ 
=
§§  
TableSessionStatus
§§ #
.
§§# $
Ended
§§$ )
;
§§) *
	UpdatedAt
•• 
=
•• 
DateTime
•• 
.
•• 
UtcNow
•• #
;
••# $
}
¶¶ 
public
®® 

TimeSpan
®® 
ManualAdjustment
®® $
{
®®% &
get
®®' *
;
®®* +
private
®®, 3
set
®®4 7
;
®®7 8
}
®®9 :
public
ÆÆ 

void
ÆÆ 

AdjustTime
ÆÆ 
(
ÆÆ 
TimeSpan
ÆÆ #

adjustment
ÆÆ$ .
)
ÆÆ. /
{
ØØ 
if
∞∞ 

(
∞∞ 
Status
∞∞ 
==
∞∞  
TableSessionStatus
∞∞ (
.
∞∞( )
Ended
∞∞) .
)
∞∞. /
{
±± 	
throw
≤≤ 
new
≤≤ '
InvalidOperationException
≤≤ /
(
≤≤/ 0
$str
≤≤0 Y
)
≤≤Y Z
;
≤≤Z [
}
≥≥ 	
ManualAdjustment
µµ 
+=
µµ 

adjustment
µµ &
;
µµ& '
	UpdatedAt
∂∂ 
=
∂∂ 
DateTime
∂∂ 
.
∂∂ 
UtcNow
∂∂ #
;
∂∂# $
}
∑∑ 
public
ΩΩ 

TimeSpan
ΩΩ 
GetBillableTime
ΩΩ #
(
ΩΩ# $
)
ΩΩ$ %
{
ææ 
var
øø 
endTime
øø 
=
øø 
EndTime
øø 
??
øø  
DateTime
øø! )
.
øø) *
UtcNow
øø* 0
;
øø0 1
var
¿¿ 
	totalTime
¿¿ 
=
¿¿ 
endTime
¿¿ 
-
¿¿  !
	StartTime
¿¿" +
;
¿¿+ ,
var
√√ "
currentPauseDuration
√√  
=
√√! "
TimeSpan
√√# +
.
√√+ ,
Zero
√√, 0
;
√√0 1
if
ƒƒ 

(
ƒƒ 
Status
ƒƒ 
==
ƒƒ  
TableSessionStatus
ƒƒ (
.
ƒƒ( )
Paused
ƒƒ) /
&&
ƒƒ0 2
PausedAt
ƒƒ3 ;
.
ƒƒ; <
HasValue
ƒƒ< D
)
ƒƒD E
{
≈≈ 	"
currentPauseDuration
∆∆  
=
∆∆! "
DateTime
∆∆# +
.
∆∆+ ,
UtcNow
∆∆, 2
-
∆∆3 4
PausedAt
∆∆5 =
.
∆∆= >
Value
∆∆> C
;
∆∆C D
}
«« 	
var
…… 
billableTime
…… 
=
…… 
	totalTime
…… $
-
……% &!
TotalPausedDuration
……' :
-
……; <"
currentPauseDuration
……= Q
+
……R S
ManualAdjustment
……T d
;
……d e
return
ÃÃ 
billableTime
ÃÃ 
<
ÃÃ 
TimeSpan
ÃÃ &
.
ÃÃ& '
Zero
ÃÃ' +
?
ÃÃ, -
TimeSpan
ÃÃ. 6
.
ÃÃ6 7
Zero
ÃÃ7 ;
:
ÃÃ< =
billableTime
ÃÃ> J
;
ÃÃJ K
}
ÕÕ 
public
”” 

void
”” 
LinkToTicket
”” 
(
”” 
Guid
”” !
ticketId
””" *
)
””* +
{
‘‘ 
if
’’ 

(
’’ 
ticketId
’’ 
==
’’ 
Guid
’’ 
.
’’ 
Empty
’’ "
)
’’" #
{
÷÷ 	
throw
◊◊ 
new
◊◊ 
ArgumentException
◊◊ '
(
◊◊' (
$str
◊◊( D
,
◊◊D E
nameof
◊◊F L
(
◊◊L M
ticketId
◊◊M U
)
◊◊U V
)
◊◊V W
;
◊◊W X
}
ÿÿ 	
TicketId
⁄⁄ 
=
⁄⁄ 
ticketId
⁄⁄ 
;
⁄⁄ 
	UpdatedAt
€€ 
=
€€ 
DateTime
€€ 
.
€€ 
UtcNow
€€ #
;
€€# $
}
‹‹ 
public
‚‚ 

void
‚‚ 
UpdateGuestCount
‚‚  
(
‚‚  !
int
‚‚! $

guestCount
‚‚% /
)
‚‚/ 0
{
„„ 
if
‰‰ 

(
‰‰ 

guestCount
‰‰ 
<=
‰‰ 
$num
‰‰ 
)
‰‰ 
{
ÂÂ 	
throw
ÊÊ 
new
ÊÊ 
ArgumentException
ÊÊ '
(
ÊÊ' (
$str
ÊÊ( P
,
ÊÊP Q
nameof
ÊÊR X
(
ÊÊX Y

guestCount
ÊÊY c
)
ÊÊc d
)
ÊÊd e
;
ÊÊe f
}
ÁÁ 	
if
ÈÈ 

(
ÈÈ 
Status
ÈÈ 
==
ÈÈ  
TableSessionStatus
ÈÈ (
.
ÈÈ( )
Ended
ÈÈ) .
)
ÈÈ. /
{
ÍÍ 	
throw
ÎÎ 
new
ÎÎ '
InvalidOperationException
ÎÎ /
(
ÎÎ/ 0
$str
ÎÎ0 `
)
ÎÎ` a
;
ÎÎa b
}
ÏÏ 	

GuestCount
ÓÓ 
=
ÓÓ 

guestCount
ÓÓ 
;
ÓÓ  
	UpdatedAt
ÔÔ 
=
ÔÔ 
DateTime
ÔÔ 
.
ÔÔ 
UtcNow
ÔÔ #
;
ÔÔ# $
}
 
}ÒÒ îb
mC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\TableLayout.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
TableLayout 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

Guid 
? 
FloorId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

Floor 
? 
Floor 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

List 
< 
Table 
> 
Tables 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
new6 9
(9 :
): ;
;; <
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
=/ 0
true1 5
;5 6
public 

bool 
IsDraft 
{ 
get 
; 
private &
set' *
;* +
}, -
=. /
false0 5
;5 6
public 

int 
Version 
{ 
get 
; 
private %
set& )
;) *
}+ ,
private 
TableLayout 
( 
) 
{ 
} 
public!! 

static!! 
TableLayout!! 
Create!! $
(!!$ %
string!!% +
name!!, 0
,!!0 1
Guid!!2 6
?!!6 7
floorId!!8 ?
=!!@ A
null!!B F
,!!F G
bool!!H L
isDraft!!M T
=!!U V
false!!W \
)!!\ ]
{"" 
if## 

(## 
string## 
.## 
IsNullOrWhiteSpace## %
(##% &
name##& *
)##* +
)##+ ,
{$$ 	
throw%% 
new%% 

Exceptions%%  
.%%  !*
BusinessRuleViolationException%%! ?
(%%? @
$str%%@ ^
)%%^ _
;%%_ `
}&& 	
return(( 
new(( 
TableLayout(( 
{)) 	
Id** 
=** 
Guid** 
.** 
NewGuid** 
(** 
)** 
,**  
Name++ 
=++ 
name++ 
,++ 
FloorId,, 
=,, 
floorId,, 
,,, 
IsDraft-- 
=-- 
isDraft-- 
,-- 
	CreatedAt.. 
=.. 
DateTime..  
...  !
UtcNow..! '
,..' (
	UpdatedAt// 
=// 
DateTime//  
.//  !
UtcNow//! '
,//' (
Version00 
=00 
$num00 
}11 	
;11	 

}22 
public77 

void77 

UpdateName77 
(77 
string77 !
name77" &
)77& '
{88 
if99 

(99 
string99 
.99 
IsNullOrWhiteSpace99 %
(99% &
name99& *
)99* +
)99+ ,
{:: 	
throw;; 
new;; 

Exceptions;;  
.;;  !*
BusinessRuleViolationException;;! ?
(;;? @
$str;;@ ^
);;^ _
;;;_ `
}<< 	
Name>> 
=>> 
name>> 
;>> 
	UpdatedAt?? 
=?? 
DateTime?? 
.?? 
UtcNow?? #
;??# $
Version@@ 
++@@ 
;@@ 
}AA 
publicFF 

voidFF 
UpdateFloorFF 
(FF 
GuidFF  
?FF  !
floorIdFF" )
)FF) *
{GG 
FloorIdHH 
=HH 
floorIdHH 
;HH 
	UpdatedAtII 
=II 
DateTimeII 
.II 
UtcNowII #
;II# $
VersionJJ 
++JJ 
;JJ 
}KK 
publicPP 

voidPP 
AddTablePP 
(PP 
TablePP 
tablePP $
)PP$ %
{QQ 
ifRR 

(RR 
tableRR 
==RR 
nullRR 
)RR 
{SS 	
throwTT 
newTT !
ArgumentNullExceptionTT +
(TT+ ,
nameofTT, 2
(TT2 3
tableTT3 8
)TT8 9
)TT9 :
;TT: ;
}UU 	
ifXX 

(XX 
TablesXX 
.XX 
AnyXX 
(XX 
tXX 
=>XX 
tXX 
.XX 
TableNumberXX )
==XX* ,
tableXX- 2
.XX2 3
TableNumberXX3 >
)XX> ?
)XX? @
{YY 	
throwZZ 
newZZ 

ExceptionsZZ  
.ZZ  !*
BusinessRuleViolationExceptionZZ! ?
(ZZ? @
$"ZZ@ B
$strZZB O
{ZZO P
tableZZP U
.ZZU V
TableNumberZZV a
}ZZa b
$str	ZZb Å
"
ZZÅ Ç
)
ZZÇ É
;
ZZÉ Ñ
}[[ 	
Tables]] 
.]] 
Add]] 
(]] 
table]] 
)]] 
;]] 
	UpdatedAt^^ 
=^^ 
DateTime^^ 
.^^ 
UtcNow^^ #
;^^# $
Version__ 
++__ 
;__ 
}`` 
publicee 

voidee 
RemoveTableee 
(ee 
Guidee  
tableIdee! (
)ee( )
{ff 
vargg 
tablegg 
=gg 
Tablesgg 
.gg 
FirstOrDefaultgg )
(gg) *
tgg* +
=>gg, .
tgg/ 0
.gg0 1
Idgg1 3
==gg4 6
tableIdgg7 >
)gg> ?
;gg? @
ifhh 

(hh 
tablehh 
!=hh 
nullhh 
)hh 
{ii 	
ifkk 
(kk 
tablekk 
.kk 
Statuskk 
!=kk 
TableStatuskk  +
.kk+ ,
	Availablekk, 5
)kk5 6
{ll 
throwmm 
newmm 

Exceptionsmm $
.mm$ %%
InvalidOperationExceptionmm% >
(mm> ?
$"mm? A
$strmmA U
{mmU V
tablemmV [
.mm[ \
TableNumbermm\ g
}mmg h
$strmmh u
{mmu v
tablemmv {
.mm{ |
Status	mm| Ç
}
mmÇ É
$str
mmÉ ù
"
mmù û
)
mmû ü
;
mmü †
}nn 
Tablespp 
.pp 
Removepp 
(pp 
tablepp 
)pp  
;pp  !
	UpdatedAtqq 
=qq 
DateTimeqq  
.qq  !
UtcNowqq! '
;qq' (
Versionrr 
++rr 
;rr 
}ss 	
}tt 
publicyy 

voidyy 
UpdateTablePositionyy #
(yy# $
Guidyy$ (
tableIdyy) 0
,yy0 1
doubleyy2 8
xyy9 :
,yy: ;
doubleyy< B
yyyC D
)yyD E
{zz 
var{{ 
table{{ 
={{ 
Tables{{ 
.{{ 
FirstOrDefault{{ )
({{) *
t{{* +
=>{{, .
t{{/ 0
.{{0 1
Id{{1 3
=={{4 6
tableId{{7 >
){{> ?
;{{? @
if|| 

(|| 
table|| 
!=|| 
null|| 
)|| 
{}} 	
table~~ 
.~~ 
UpdatePosition~~  
(~~  !
x~~! "
,~~" #
y~~$ %
)~~% &
;~~& '
	UpdatedAt 
= 
DateTime  
.  !
UtcNow! '
;' (
Version
ÄÄ 
++
ÄÄ 
;
ÄÄ 
}
ÅÅ 	
}
ÇÇ 
public
áá 

IReadOnlyList
áá 
<
áá 
Table
áá 
>
áá 
GetTablesByStatus
áá  1
(
áá1 2
TableStatus
áá2 =
status
áá> D
)
ááD E
{
àà 
return
ââ 
Tables
ââ 
.
ââ 
Where
ââ 
(
ââ 
t
ââ 
=>
ââ  
t
ââ! "
.
ââ" #
Status
ââ# )
==
ââ* ,
status
ââ- 3
)
ââ3 4
.
ââ4 5
ToList
ââ5 ;
(
ââ; <
)
ââ< =
.
ââ= >

AsReadOnly
ââ> H
(
ââH I
)
ââI J
;
ââJ K
}
ää 
public
èè 

int
èè  
GetNextTableNumber
èè !
(
èè! "
)
èè" #
{
êê 
if
ëë 

(
ëë 
!
ëë 
Tables
ëë 
.
ëë 
Any
ëë 
(
ëë 
)
ëë 
)
ëë 
{
íí 	
return
ìì 
$num
ìì 
;
ìì 
}
îî 	
return
ññ 
Tables
ññ 
.
ññ 
Max
ññ 
(
ññ 
t
ññ 
=>
ññ 
t
ññ  
.
ññ  !
TableNumber
ññ! ,
)
ññ, -
+
ññ. /
$num
ññ0 1
;
ññ1 2
}
óó 
public
úú 

void
úú 
Activate
úú 
(
úú 
)
úú 
{
ùù 
IsActive
ûû 
=
ûû 
true
ûû 
;
ûû 
	UpdatedAt
üü 
=
üü 
DateTime
üü 
.
üü 
UtcNow
üü #
;
üü# $
Version
†† 
++
†† 
;
†† 
}
°° 
public
¶¶ 

void
¶¶ 

Deactivate
¶¶ 
(
¶¶ 
)
¶¶ 
{
ßß 
if
©© 

(
©© 
Tables
©© 
.
©© 
Any
©© 
(
©© 
t
©© 
=>
©© 
t
©© 
.
©© 
Status
©© $
==
©©% '
TableStatus
©©( 3
.
©©3 4
Seat
©©4 8
)
©©8 9
)
©©9 :
{
™™ 	
throw
´´ 
new
´´ 

Exceptions
´´  
.
´´  !'
InvalidOperationException
´´! :
(
´´: ;
$str
´´; i
)
´´i j
;
´´j k
}
¨¨ 	
IsActive
ÆÆ 
=
ÆÆ 
false
ÆÆ 
;
ÆÆ 
	UpdatedAt
ØØ 
=
ØØ 
DateTime
ØØ 
.
ØØ 
UtcNow
ØØ #
;
ØØ# $
Version
∞∞ 
++
∞∞ 
;
∞∞ 
}
±± 
public
∂∂ 

void
∂∂ 
SetDraftStatus
∂∂ 
(
∂∂ 
bool
∂∂ #
isDraft
∂∂$ +
)
∂∂+ ,
{
∑∑ 
IsDraft
∏∏ 
=
∏∏ 
isDraft
∏∏ 
;
∏∏ 
	UpdatedAt
ππ 
=
ππ 
DateTime
ππ 
.
ππ 
UtcNow
ππ #
;
ππ# $
Version
∫∫ 
++
∫∫ 
;
∫∫ 
}
ªª 
public
¿¿ 

bool
¿¿ 
IsValid
¿¿ 
(
¿¿ 
)
¿¿ 
{
¡¡ 
return
¬¬ 
!
¬¬ 
string
¬¬ 
.
¬¬  
IsNullOrWhiteSpace
¬¬ )
(
¬¬) *
Name
¬¬* .
)
¬¬. /
&&
¬¬0 2
Tables
¬¬3 9
.
¬¬9 :
Any
¬¬: =
(
¬¬= >
)
¬¬> ?
;
¬¬? @
}
√√ 
}ƒƒ ˛ù
gC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Table.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 
Table

 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

int 
TableNumber 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

int 
Capacity 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

double 
X 
{ 
get 
; 
private "
set# &
;& '
}( )
=* +
$num, -
;- .
public 

double 
Y 
{ 
get 
; 
private "
set# &
;& '
}( )
=* +
$num, -
;- .
public 

double 
Width 
{ 
get 
; 
private &
set' *
;* +
}, -
=. /
$num0 3
;3 4
public 

double 
Height 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
=/ 0
$num1 4
;4 5
public 

TableShapeType 
Shape 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
=6 7
TableShapeType8 F
.F G
	RectangleG P
;P Q
public 

Guid 
? 
FloorId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

Guid 
? 
LayoutId 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

TableLayout 
? 
Layout 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

TableStatus 
Status 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
TableStatus6 A
.A B
	AvailableB K
;K L
public 

Guid 
? 
CurrentTicketId  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
public 

Guid 
? 
TableTypeId 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

	TableType 
? 
	TableType 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
=/ 0
true1 5
;5 6
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

int 
Version 
{ 
get 
; 
private %
set& )
;) *
}+ ,
private!! 
Table!! 
(!! 
)!! 
{"" 
}## 
public(( 

static(( 
Table(( 
Create(( 
((( 
int)) 
tableNumber)) 
,)) 
int** 
capacity** 
,** 
double++ 
x++ 
=++ 
$num++ 
,++ 
double,, 
y,, 
=,, 
$num,, 
,,, 
Guid-- 
?-- 
floorId-- 
=-- 
null-- 
,-- 
Guid.. 
?.. 
layoutId.. 
=.. 
null.. 
,.. 
Guid// 
?// 
tableTypeId// 
=// 
null//  
,//  !
bool00 
isActive00 
=00 
true00 
,00 
TableShapeType11 
shape11 
=11 
TableShapeType11 -
.11- .
	Rectangle11. 7
,117 8
double22 
width22 
=22 
$num22 
,22 
double33 
height33 
=33 
$num33 
)33 
{44 
if55 

(55 
tableNumber55 
<=55 
$num55 
)55 
{66 	
throw77 
new77 

Exceptions77  
.77  !*
BusinessRuleViolationException77! ?
(77? @
$str77@ i
)77i j
;77j k
}88 	
if:: 

(:: 
capacity:: 
<=:: 
$num:: 
):: 
{;; 	
throw<< 
new<< 

Exceptions<<  
.<<  !*
BusinessRuleViolationException<<! ?
(<<? @
$str<<@ k
)<<k l
;<<l m
}== 	
return?? 
new?? 
Table?? 
{@@ 	
IdAA 
=AA 
GuidAA 
.AA 
NewGuidAA 
(AA 
)AA 
,AA  
TableNumberBB 
=BB 
tableNumberBB %
,BB% &
FloorIdCC 
=CC 
floorIdCC 
,CC 
CapacityDD 
=DD 
capacityDD 
,DD  
XEE 
=EE 
xEE 
,EE 
YFF 
=FF 
yFF 
,FF 
WidthGG 
=GG 
widthGG 
,GG 
HeightHH 
=HH 
heightHH 
,HH 
ShapeII 
=II 
shapeII 
,II 
StatusJJ 
=JJ 
TableStatusJJ  
.JJ  !
	AvailableJJ! *
,JJ* +
CurrentTicketIdKK 
=KK 
nullKK "
,KK" #
TableTypeIdLL 
=LL 
tableTypeIdLL %
,LL% &
LayoutIdMM 
=MM 
layoutIdMM 
,MM  
IsActiveNN 
=NN 
isActiveNN 
,NN  
VersionOO 
=OO 
$numOO 
}PP 	
;PP	 

}QQ 
publicVV 

voidVV 
UpdateTableNumberVV !
(VV! "
intVV" %
tableNumberVV& 1
)VV1 2
{WW 
ifXX 

(XX 
tableNumberXX 
<=XX 
$numXX 
)XX 
{YY 	
throwZZ 
newZZ 

ExceptionsZZ  
.ZZ  !*
BusinessRuleViolationExceptionZZ! ?
(ZZ? @
$strZZ@ i
)ZZi j
;ZZj k
}[[ 	
TableNumber]] 
=]] 
tableNumber]] !
;]]! "
}^^ 
publiccc 

voidcc 
UpdateCapacitycc 
(cc 
intcc "
capacitycc# +
)cc+ ,
{dd 
ifee 

(ee 
capacityee 
<=ee 
$numee 
)ee 
{ff 	
throwgg 
newgg 

Exceptionsgg  
.gg  !*
BusinessRuleViolationExceptiongg! ?
(gg? @
$strgg@ k
)ggk l
;ggl m
}hh 	
Capacityjj 
=jj 
capacityjj 
;jj 
}kk 
publicpp 

voidpp 
UpdatePositionpp 
(pp 
doublepp %
xpp& '
,pp' (
doublepp) /
ypp0 1
)pp1 2
{qq 
Xrr 	
=rr
 
xrr 
;rr 
Yss 	
=ss
 
yss 
;ss 
}tt 
publicyy 

voidyy 
UpdateGeometryyy 
(yy 
doubleyy %
xyy& '
,yy' (
doubleyy) /
yyy0 1
,yy1 2
TableShapeTypeyy3 A
shapeyyB G
,yyG H
doubleyyI O
widthyyP U
,yyU V
doubleyyW ]
heightyy^ d
)yyd e
{zz 
X{{ 	
={{
 
x{{ 
;{{ 
Y|| 	
=||
 
y|| 
;|| 
Shape}} 
=}} 
shape}} 
;}} 
Width~~ 
=~~ 
width~~ 
;~~ 
Height 
= 
height 
; 
}
ÄÄ 
public
ÖÖ 

void
ÖÖ 
UpdateFloor
ÖÖ 
(
ÖÖ 
Guid
ÖÖ  
?
ÖÖ  !
floorId
ÖÖ" )
)
ÖÖ) *
{
ÜÜ 
FloorId
áá 
=
áá 
floorId
áá 
;
áá 
}
àà 
public
èè 

void
èè 
SetTableType
èè 
(
èè 
Guid
èè !
tableTypeId
èè" -
)
èè- .
{
êê 
if
ëë 

(
ëë 
tableTypeId
ëë 
==
ëë 
Guid
ëë 
.
ëë  
Empty
ëë  %
)
ëë% &
{
íí 	
throw
ìì 
new
ìì 
ArgumentException
ìì '
(
ìì' (
$str
ìì( H
,
ììH I
nameof
ììJ P
(
ììP Q
tableTypeId
ììQ \
)
ìì\ ]
)
ìì] ^
;
ìì^ _
}
îî 	
TableTypeId
ññ 
=
ññ 
tableTypeId
ññ !
;
ññ! "
	UpdatedAt
óó 
=
óó 
DateTime
óó 
.
óó 
UtcNow
óó #
;
óó# $
}
òò 
public
ùù 

void
ùù 
ClearTableType
ùù 
(
ùù 
)
ùù  
{
ûû 
TableTypeId
üü 
=
üü 
null
üü 
;
üü 
	UpdatedAt
†† 
=
†† 
DateTime
†† 
.
†† 
UtcNow
†† #
;
††# $
}
°° 
public
¶¶ 

void
¶¶ 
AssignTicket
¶¶ 
(
¶¶ 
Guid
¶¶ !
ticketId
¶¶" *
)
¶¶* +
{
ßß 
if
©© 

(
©© 
CurrentTicketId
©© 
.
©© 
HasValue
©© $
&&
©©% '
CurrentTicketId
©©( 7
.
©©7 8
Value
©©8 =
==
©©> @
ticketId
©©A I
)
©©I J
{
™™ 	
return
´´ 
;
´´ 
}
¨¨ 	
if
ÆÆ 

(
ÆÆ 
Status
ÆÆ 
!=
ÆÆ 
TableStatus
ÆÆ !
.
ÆÆ! "
	Available
ÆÆ" +
&&
ÆÆ, .
Status
ÆÆ/ 5
!=
ÆÆ6 8
TableStatus
ÆÆ9 D
.
ÆÆD E
Booked
ÆÆE K
)
ÆÆK L
{
ØØ 	
throw
∞∞ 
new
∞∞ 

Exceptions
∞∞  
.
∞∞  !'
InvalidOperationException
∞∞! :
(
∞∞: ;
$"
∞∞; =
$str
∞∞= g
{
∞∞g h
Status
∞∞h n
}
∞∞n o
$str
∞∞o p
"
∞∞p q
)
∞∞q r
;
∞∞r s
}
±± 	
if
≥≥ 

(
≥≥ 
CurrentTicketId
≥≥ 
.
≥≥ 
HasValue
≥≥ $
&&
≥≥% '
CurrentTicketId
≥≥( 7
.
≥≥7 8
Value
≥≥8 =
!=
≥≥> @
ticketId
≥≥A I
)
≥≥I J
{
¥¥ 	
throw
µµ 
new
µµ 

Exceptions
µµ  
.
µµ  !'
InvalidOperationException
µµ! :
(
µµ: ;
$str
µµ; b
)
µµb c
;
µµc d
}
∂∂ 	
CurrentTicketId
∏∏ 
=
∏∏ 
ticketId
∏∏ "
;
∏∏" #
Status
ππ 
=
ππ 
TableStatus
ππ 
.
ππ 
Seat
ππ !
;
ππ! "
}
∫∫ 
public
øø 

void
øø 
ReleaseTicket
øø 
(
øø 
)
øø 
{
¿¿ 
if
¡¡ 

(
¡¡ 
!
¡¡ 
CurrentTicketId
¡¡ 
.
¡¡ 
HasValue
¡¡ %
)
¡¡% &
{
¬¬ 	
throw
√√ 
new
√√ 

Exceptions
√√  
.
√√  !'
InvalidOperationException
√√! :
(
√√: ;
$str
√√; d
)
√√d e
;
√√e f
}
ƒƒ 	
CurrentTicketId
∆∆ 
=
∆∆ 
null
∆∆ 
;
∆∆ 
Status
«« 
=
«« 
TableStatus
«« 
.
«« 
	Available
«« &
;
««& '
}
»» 
public
ÕÕ 

void
ÕÕ 
Book
ÕÕ 
(
ÕÕ 
)
ÕÕ 
{
ŒŒ 
if
œœ 

(
œœ 
Status
œœ 
!=
œœ 
TableStatus
œœ !
.
œœ! "
	Available
œœ" +
)
œœ+ ,
{
–– 	
throw
—— 
new
—— 

Exceptions
——  
.
——  !'
InvalidOperationException
——! :
(
——: ;
$"
——; =
$str
——= [
{
——[ \
Status
——\ b
}
——b c
$str
——c d
"
——d e
)
——e f
;
——f g
}
““ 	
Status
‘‘ 
=
‘‘ 
TableStatus
‘‘ 
.
‘‘ 
Booked
‘‘ #
;
‘‘# $
}
’’ 
public
⁄⁄ 

void
⁄⁄ 
	MarkInUse
⁄⁄ 
(
⁄⁄ 
)
⁄⁄ 
{
€€ 
if
‹‹ 

(
‹‹ 
Status
‹‹ 
==
‹‹ 
TableStatus
‹‹ !
.
‹‹! "
Seat
‹‹" &
)
‹‹& '
{
›› 	
return
ﬂﬂ 
;
ﬂﬂ 
}
‡‡ 	
if
‚‚ 

(
‚‚ 
Status
‚‚ 
!=
‚‚ 
TableStatus
‚‚ !
.
‚‚! "
	Available
‚‚" +
&&
‚‚, .
Status
‚‚/ 5
!=
‚‚6 8
TableStatus
‚‚9 D
.
‚‚D E
Booked
‚‚E K
)
‚‚K L
{
„„ 	
throw
‰‰ 
new
‰‰ 

Exceptions
‰‰  
.
‰‰  !'
InvalidOperationException
‰‰! :
(
‰‰: ;
$"
‰‰; =
$str
‰‰= b
{
‰‰b c
Status
‰‰c i
}
‰‰i j
$str
‰‰j k
"
‰‰k l
)
‰‰l m
;
‰‰m n
}
ÂÂ 	
Status
ÁÁ 
=
ÁÁ 
TableStatus
ÁÁ 
.
ÁÁ 
Seat
ÁÁ !
;
ÁÁ! "
	UpdatedAt
ËË 
=
ËË 
DateTime
ËË 
.
ËË 
UtcNow
ËË #
;
ËË# $
}
ÈÈ 
public
ÓÓ 

void
ÓÓ 
MarkAvailable
ÓÓ 
(
ÓÓ 
)
ÓÓ 
{
ÔÔ 
if
 

(
 
CurrentTicketId
 
.
 
HasValue
 $
)
$ %
{
ÒÒ 	
throw
ÚÚ 
new
ÚÚ 

Exceptions
ÚÚ  
.
ÚÚ  !'
InvalidOperationException
ÚÚ! :
(
ÚÚ: ;
$strÚÚ; à
)ÚÚà â
;ÚÚâ ä
}
ÛÛ 	
Status
ıı 
=
ıı 
TableStatus
ıı 
.
ıı 
	Available
ıı &
;
ıı& '
	UpdatedAt
ˆˆ 
=
ˆˆ 
DateTime
ˆˆ 
.
ˆˆ 
UtcNow
ˆˆ #
;
ˆˆ# $
}
˜˜ 
public
¸¸ 

void
¸¸ 
	MarkDirty
¸¸ 
(
¸¸ 
)
¸¸ 
{
˝˝ 
if
˛˛ 

(
˛˛ 
Status
˛˛ 
!=
˛˛ 
TableStatus
˛˛ !
.
˛˛! "
	Available
˛˛" +
)
˛˛+ ,
{
ˇˇ 	
throw
ÄÄ 
new
ÄÄ 

Exceptions
ÄÄ  
.
ÄÄ  !'
InvalidOperationException
ÄÄ! :
(
ÄÄ: ;
$"
ÄÄ; =
$str
ÄÄ= d
{
ÄÄd e
Status
ÄÄe k
}
ÄÄk l
$str
ÄÄl m
"
ÄÄm n
)
ÄÄn o
;
ÄÄo p
}
ÅÅ 	
Status
ÉÉ 
=
ÉÉ 
TableStatus
ÉÉ 
.
ÉÉ 
Dirty
ÉÉ "
;
ÉÉ" #
}
ÑÑ 
public
ââ 

void
ââ 
	MarkClean
ââ 
(
ââ 
)
ââ 
{
ää 
if
ãã 

(
ãã 
Status
ãã 
!=
ãã 
TableStatus
ãã !
.
ãã! "
Dirty
ãã" '
)
ãã' (
{
åå 	
throw
çç 
new
çç 

Exceptions
çç  
.
çç  !'
InvalidOperationException
çç! :
(
çç: ;
$"
çç; =
$str
çç= d
{
ççd e
Status
ççe k
}
ççk l
$str
ççl m
"
ççm n
)
ççn o
;
çço p
}
éé 	
Status
êê 
=
êê 
TableStatus
êê 
.
êê 
	Available
êê &
;
êê& '
}
ëë 
public
ññ 

void
ññ 
Disable
ññ 
(
ññ 
)
ññ 
{
óó 
if
òò 

(
òò 
Status
òò 
==
òò 
TableStatus
òò !
.
òò! "
Seat
òò" &
)
òò& '
{
ôô 	
throw
öö 
new
öö 

Exceptions
öö  
.
öö  !'
InvalidOperationException
öö! :
(
öö: ;
$str
öö; e
)
ööe f
;
ööf g
}
õõ 	
Status
ùù 
=
ùù 
TableStatus
ùù 
.
ùù 
Disable
ùù $
;
ùù$ %
}
ûû 
public
££ 

void
££ 
Enable
££ 
(
££ 
)
££ 
{
§§ 
if
•• 

(
•• 
Status
•• 
!=
•• 
TableStatus
•• !
.
••! "
Disable
••" )
)
••) *
{
¶¶ 	
throw
ßß 
new
ßß 

Exceptions
ßß  
.
ßß  !'
InvalidOperationException
ßß! :
(
ßß: ;
$"
ßß; =
$str
ßß= ]
{
ßß] ^
Status
ßß^ d
}
ßßd e
$str
ßße f
"
ßßf g
)
ßßg h
;
ßßh i
}
®® 	
Status
™™ 
=
™™ 
TableStatus
™™ 
.
™™ 
	Available
™™ &
;
™™& '
}
´´ 
public
∞∞ 

void
∞∞ 
Activate
∞∞ 
(
∞∞ 
)
∞∞ 
{
±± 
IsActive
≤≤ 
=
≤≤ 
true
≤≤ 
;
≤≤ 
}
≥≥ 
public
∏∏ 

void
∏∏ 

Deactivate
∏∏ 
(
∏∏ 
)
∏∏ 
{
ππ 
if
∫∫ 

(
∫∫ 
Status
∫∫ 
==
∫∫ 
TableStatus
∫∫ !
.
∫∫! "
Seat
∫∫" &
)
∫∫& '
{
ªª 	
throw
ºº 
new
ºº 

Exceptions
ºº  
.
ºº  !'
InvalidOperationException
ºº! :
(
ºº: ;
$str
ºº; h
)
ººh i
;
ººi j
}
ΩΩ 	
IsActive
øø 
=
øø 
false
øø 
;
øø 
}
¿¿ 
public
≈≈ 

bool
≈≈ 
IsAvailable
≈≈ 
(
≈≈ 
)
≈≈ 
{
∆∆ 
return
«« 
IsActive
«« 
&&
«« 
Status
«« !
==
««" $
TableStatus
««% 0
.
««0 1
	Available
««1 :
;
««: ;
}
»» 
}…… è"
oC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\StockMovement.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 
StockMovement

 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 

MenuItemId 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

int 
QuantityChange 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

StockMovementType 
Type !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 

string 
	Reference 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

DateTime 
	Timestamp 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

Guid 
? 
UserId 
{ 
get 
; 
private &
set' *
;* +
}, -
	protected 
StockMovement 
( 
) 
{ 
}  !
private 
StockMovement 
( 
Guid 

menuItemId 
, 
int 
quantityChange 
, 
StockMovementType 
type 
, 
string 
	reference 
, 
Guid 
? 
userId 
) 
{ 
Id   

=   
Guid   
.   
NewGuid   
(   
)   
;   

MenuItemId!! 
=!! 

menuItemId!! 
;!!  
QuantityChange"" 
="" 
quantityChange"" '
;""' (
Type## 
=## 
type## 
;## 
	Reference$$ 
=$$ 
	reference$$ 
??$$  
throw$$! &
new$$' *!
ArgumentNullException$$+ @
($$@ A
nameof$$A G
($$G H
	reference$$H Q
)$$Q R
)$$R S
;$$S T
	Timestamp%% 
=%% 
DateTime%% 
.%% 
UtcNow%% #
;%%# $
UserId&& 
=&& 
userId&& 
;&& 
}'' 
public)) 

static)) 
StockMovement)) 
Create))  &
())& '
Guid** 

menuItemId** 
,** 
int++ 
quantityChange++ 
,++ 
StockMovementType,, 
type,, 
,,, 
string-- 
	reference-- 
,-- 
Guid.. 
?.. 
userId.. 
).. 
{// 
if00 

(00 

menuItemId00 
==00 
Guid00 
.00 
Empty00 $
)00$ %
throw11 
new11 
ArgumentException11 '
(11' (
$str11( E
,11E F
nameof11G M
(11M N

menuItemId11N X
)11X Y
)11Y Z
;11Z [
if33 

(33 
string33 
.33 
IsNullOrWhiteSpace33 %
(33% &
	reference33& /
)33/ 0
)330 1
throw44 
new44 
ArgumentException44 '
(44' (
$str44( G
,44G H
nameof44I O
(44O P
	reference44P Y
)44Y Z
)44Z [
;44[ \
return66 
new66 
StockMovement66  
(66  !

menuItemId66! +
,66+ ,
quantityChange66- ;
,66; <
type66= A
,66A B
	reference66C L
,66L M
userId66N T
)66T U
;66U V
}77 
}88 Ò1
gC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Shift.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
Shift		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
null/ 3
!3 4
;4 5
public 

TimeSpan 
	StartTime 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

TimeSpan 
EndTime 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

int 
Version 
{ 
get 
; 
private %
set& )
;) *
}+ ,
private 
Shift 
( 
) 
{ 
Name 
= 
string 
. 
Empty 
; 
} 
public 

static 
Shift 
Create 
( 
string 
name 
, 
TimeSpan 
	startTime 
, 
TimeSpan 
endTime 
, 
bool 
isActive 
= 
true 
) 
{   
if!! 

(!! 
string!! 
.!! 
IsNullOrWhiteSpace!! %
(!!% &
name!!& *
)!!* +
)!!+ ,
{"" 	
throw## 
new## 

Exceptions##  
.##  !*
BusinessRuleViolationException##! ?
(##? @
$str##@ ]
)##] ^
;##^ _
}$$ 	
if&& 

(&& 
	startTime&& 
==&& 
endTime&&  
)&&  !
{'' 	
throw(( 
new(( 

Exceptions((  
.((  !*
BusinessRuleViolationException((! ?
(((? @
$str((@ s
)((s t
;((t u
})) 	
return++ 
new++ 
Shift++ 
{,, 	
Id-- 
=-- 
Guid-- 
.-- 
NewGuid-- 
(-- 
)-- 
,--  
Name.. 
=.. 
name.. 
,.. 
	StartTime// 
=// 
	startTime// !
,//! "
EndTime00 
=00 
endTime00 
,00 
IsActive11 
=11 
isActive11 
,11  
Version22 
=22 
$num22 
}33 	
;33	 

}44 
public99 

void99 

UpdateName99 
(99 
string99 !
name99" &
)99& '
{:: 
if;; 

(;; 
string;; 
.;; 
IsNullOrWhiteSpace;; %
(;;% &
name;;& *
);;* +
);;+ ,
{<< 	
throw== 
new== 

Exceptions==  
.==  !*
BusinessRuleViolationException==! ?
(==? @
$str==@ ]
)==] ^
;==^ _
}>> 	
Name@@ 
=@@ 
name@@ 
;@@ 
}AA 
publicFF 

voidFF 
UpdateTimesFF 
(FF 
TimeSpanFF $
	startTimeFF% .
,FF. /
TimeSpanFF0 8
endTimeFF9 @
)FF@ A
{GG 
ifHH 

(HH 
	startTimeHH 
==HH 
endTimeHH  
)HH  !
{II 	
throwJJ 
newJJ 

ExceptionsJJ  
.JJ  !*
BusinessRuleViolationExceptionJJ! ?
(JJ? @
$strJJ@ s
)JJs t
;JJt u
}KK 	
	StartTimeMM 
=MM 
	startTimeMM 
;MM 
EndTimeNN 
=NN 
endTimeNN 
;NN 
}OO 
publicTT 

voidTT 
ActivateTT 
(TT 
)TT 
{UU 
IsActiveVV 
=VV 
trueVV 
;VV 
}WW 
public\\ 

void\\ 

Deactivate\\ 
(\\ 
)\\ 
{]] 
IsActive^^ 
=^^ 
false^^ 
;^^ 
}__ 
publicee 

boolee 
IsTimeInShiftee 
(ee 
TimeSpanee &
timeee' +
)ee+ ,
{ff 
ifgg 

(gg 
	StartTimegg 
<gg 
EndTimegg 
)gg  
{hh 	
returnjj 
timejj 
>=jj 
	StartTimejj $
&&jj% '
timejj( ,
<=jj- /
EndTimejj0 7
;jj7 8
}kk 	
elsell 
{mm 	
returnoo 
timeoo 
>=oo 
	StartTimeoo $
||oo% '
timeoo( ,
<=oo- /
EndTimeoo0 7
;oo7 8
}pp 	
}qq 
publicvv 

staticvv 
Shiftvv 
?vv 
GetCurrentShiftvv (
(vv( )
IEnumerablevv) 4
<vv4 5
Shiftvv5 :
>vv: ;
shiftsvv< B
)vvB C
{ww 
varxx 
currentTimexx 
=xx 
DateTimexx "
.xx" #
Nowxx# &
.xx& '
	TimeOfDayxx' 0
;xx0 1
returnyy 
shiftsyy 
.yy 
FirstOrDefaultyy $
(yy$ %
syy% &
=>yy' )
syy* +
.yy+ ,
IsActiveyy, 4
&&yy5 7
syy8 9
.yy9 :
IsTimeInShiftyy: G
(yyG H
currentTimeyyH S
)yyS T
)yyT U
;yyU V
}zz 
}{{ §m
oC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\ServerSection.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
ServerSection 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

string 
Description 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
string6 <
.< =
Empty= B
;B C
public 

Guid 
ServerId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

List 
< 
Guid 
> 
TableIds 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
=5 6
new7 :
(: ;
); <
;< =
public 

string 
Color 
{ 
get 
; 
private &
set' *
;* +
}, -
=. /
$str0 9
;9 :
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
=/ 0
true1 5
;5 6
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

int 
Version 
{ 
get 
; 
private %
set& )
;) *
}+ ,
public 

virtual 
User 
? 
Server 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

virtual 
ICollection 
< 
Table $
>$ %
Tables& ,
{- .
get/ 2
;2 3
private4 ;
set< ?
;? @
}A B
=C D
newE H
ListI M
<M N
TableN S
>S T
(T U
)U V
;V W
private 
ServerSection 
( 
) 
{ 
} 
public$$ 

static$$ 
ServerSection$$ 
Create$$  &
($$& '
string$$' -
name$$. 2
,$$2 3
Guid$$4 8
serverId$$9 A
,$$A B
string$$C I
description$$J U
=$$V W
$str$$X Z
,$$Z [
string$$\ b
color$$c h
=$$i j
$str$$k t
)$$t u
{%% 
if&& 

(&& 
string&& 
.&& 
IsNullOrWhiteSpace&& %
(&&% &
name&&& *
)&&* +
)&&+ ,
{'' 	
throw(( 
new(( 

Exceptions((  
.((  !*
BusinessRuleViolationException((! ?
(((? @
$str((@ _
)((_ `
;((` a
})) 	
if++ 

(++ 
serverId++ 
==++ 
Guid++ 
.++ 
Empty++ "
)++" #
{,, 	
throw-- 
new-- 

Exceptions--  
.--  !*
BusinessRuleViolationException--! ?
(--? @
$str--@ X
)--X Y
;--Y Z
}.. 	
return00 
new00 
ServerSection00  
{11 	
Id22 
=22 
Guid22 
.22 
NewGuid22 
(22 
)22 
,22  
Name33 
=33 
name33 
,33 
Description44 
=44 
description44 %
??44& (
string44) /
.44/ 0
Empty440 5
,445 6
ServerId55 
=55 
serverId55 
,55  
Color66 
=66 
color66 
??66 
$str66 &
,66& '
	CreatedAt77 
=77 
DateTime77  
.77  !
UtcNow77! '
,77' (
	UpdatedAt88 
=88 
DateTime88  
.88  !
UtcNow88! '
,88' (
Version99 
=99 
$num99 
}:: 	
;::	 

};; 
public@@ 

void@@ 

UpdateName@@ 
(@@ 
string@@ !
name@@" &
)@@& '
{AA 
ifBB 

(BB 
stringBB 
.BB 
IsNullOrWhiteSpaceBB %
(BB% &
nameBB& *
)BB* +
)BB+ ,
{CC 	
throwDD 
newDD 

ExceptionsDD  
.DD  !*
BusinessRuleViolationExceptionDD! ?
(DD? @
$strDD@ _
)DD_ `
;DD` a
}EE 	
NameGG 
=GG 
nameGG 
;GG 
	UpdatedAtHH 
=HH 
DateTimeHH 
.HH 
UtcNowHH #
;HH# $
VersionII 
++II 
;II 
}JJ 
publicOO 

voidOO 
UpdateDescriptionOO !
(OO! "
stringOO" (
descriptionOO) 4
)OO4 5
{PP 
DescriptionQQ 
=QQ 
descriptionQQ !
??QQ" $
stringQQ% +
.QQ+ ,
EmptyQQ, 1
;QQ1 2
	UpdatedAtRR 
=RR 
DateTimeRR 
.RR 
UtcNowRR #
;RR# $
VersionSS 
++SS 
;SS 
}TT 
publicYY 

voidYY 
UpdateColorYY 
(YY 
stringYY "
colorYY# (
)YY( )
{ZZ 
Color[[ 
=[[ 
color[[ 
??[[ 
$str[[ "
;[[" #
	UpdatedAt\\ 
=\\ 
DateTime\\ 
.\\ 
UtcNow\\ #
;\\# $
Version]] 
++]] 
;]] 
}^^ 
publiccc 

voidcc 
UpdateServercc 
(cc 
Guidcc !
serverIdcc" *
)cc* +
{dd 
ifee 

(ee 
serverIdee 
==ee 
Guidee 
.ee 
Emptyee "
)ee" #
{ff 	
throwgg 
newgg 

Exceptionsgg  
.gg  !*
BusinessRuleViolationExceptiongg! ?
(gg? @
$strgg@ X
)ggX Y
;ggY Z
}hh 	
ServerIdjj 
=jj 
serverIdjj 
;jj 
	UpdatedAtkk 
=kk 
DateTimekk 
.kk 
UtcNowkk #
;kk# $
Versionll 
++ll 
;ll 
}mm 
publicrr 

voidrr 
	AddTablesrr 
(rr 
IEnumerablerr %
<rr% &
Guidrr& *
>rr* +
tableIdsrr, 4
)rr4 5
{ss 
iftt 

(tt 
tableIdstt 
==tt 
nulltt 
)tt 
{uu 	
throwvv 
newvv !
ArgumentNullExceptionvv +
(vv+ ,
nameofvv, 2
(vv2 3
tableIdsvv3 ;
)vv; <
)vv< =
;vv= >
}ww 	
foreachyy 
(yy 
varyy 
tableIdyy 
inyy 
tableIdsyy  (
)yy( )
{zz 	
if{{ 
({{ 
!{{ 
TableIds{{ 
.{{ 
Contains{{ "
({{" #
tableId{{# *
){{* +
){{+ ,
{|| 
TableIds}} 
.}} 
Add}} 
(}} 
tableId}} $
)}}$ %
;}}% &
}~~ 
} 	
	UpdatedAt
ÅÅ 
=
ÅÅ 
DateTime
ÅÅ 
.
ÅÅ 
UtcNow
ÅÅ #
;
ÅÅ# $
Version
ÇÇ 
++
ÇÇ 
;
ÇÇ 
}
ÉÉ 
public
àà 

void
àà 
AddTable
àà 
(
àà 
Guid
àà 
tableId
àà %
)
àà% &
{
ââ 
if
ää 

(
ää 
tableId
ää 
==
ää 
Guid
ää 
.
ää 
Empty
ää !
)
ää! "
{
ãã 	
throw
åå 
new
åå 
ArgumentException
åå '
(
åå' (
$str
åå( C
,
ååC D
nameof
ååE K
(
ååK L
tableId
ååL S
)
ååS T
)
ååT U
;
ååU V
}
çç 	
if
èè 

(
èè 
!
èè 
TableIds
èè 
.
èè 
Contains
èè 
(
èè 
tableId
èè &
)
èè& '
)
èè' (
{
êê 	
TableIds
ëë 
.
ëë 
Add
ëë 
(
ëë 
tableId
ëë  
)
ëë  !
;
ëë! "
	UpdatedAt
íí 
=
íí 
DateTime
íí  
.
íí  !
UtcNow
íí! '
;
íí' (
Version
ìì 
++
ìì 
;
ìì 
}
îî 	
}
ïï 
public
öö 

void
öö 
RemoveTables
öö 
(
öö 
IEnumerable
öö (
<
öö( )
Guid
öö) -
>
öö- .
tableIds
öö/ 7
)
öö7 8
{
õõ 
if
úú 

(
úú 
tableIds
úú 
==
úú 
null
úú 
)
úú 
{
ùù 	
throw
ûû 
new
ûû #
ArgumentNullException
ûû +
(
ûû+ ,
nameof
ûû, 2
(
ûû2 3
tableIds
ûû3 ;
)
ûû; <
)
ûû< =
;
ûû= >
}
üü 	
foreach
°° 
(
°° 
var
°° 
tableId
°° 
in
°° 
tableIds
°°  (
)
°°( )
{
¢¢ 	
TableIds
££ 
.
££ 
Remove
££ 
(
££ 
tableId
££ #
)
££# $
;
££$ %
}
§§ 	
	UpdatedAt
¶¶ 
=
¶¶ 
DateTime
¶¶ 
.
¶¶ 
UtcNow
¶¶ #
;
¶¶# $
Version
ßß 
++
ßß 
;
ßß 
}
®® 
public
≠≠ 

void
≠≠ 
RemoveTable
≠≠ 
(
≠≠ 
Guid
≠≠  
tableId
≠≠! (
)
≠≠( )
{
ÆÆ 
if
ØØ 

(
ØØ 
TableIds
ØØ 
.
ØØ 
Remove
ØØ 
(
ØØ 
tableId
ØØ #
)
ØØ# $
)
ØØ$ %
{
∞∞ 	
	UpdatedAt
±± 
=
±± 
DateTime
±±  
.
±±  !
UtcNow
±±! '
;
±±' (
Version
≤≤ 
++
≤≤ 
;
≤≤ 
}
≥≥ 	
}
¥¥ 
public
ππ 

void
ππ 
ClearTables
ππ 
(
ππ 
)
ππ 
{
∫∫ 
if
ªª 

(
ªª 
TableIds
ªª 
.
ªª 
Any
ªª 
(
ªª 
)
ªª 
)
ªª 
{
ºº 	
TableIds
ΩΩ 
.
ΩΩ 
Clear
ΩΩ 
(
ΩΩ 
)
ΩΩ 
;
ΩΩ 
	UpdatedAt
ææ 
=
ææ 
DateTime
ææ  
.
ææ  !
UtcNow
ææ! '
;
ææ' (
Version
øø 
++
øø 
;
øø 
}
¿¿ 	
}
¡¡ 
public
∆∆ 

bool
∆∆ 
ContainsTable
∆∆ 
(
∆∆ 
Guid
∆∆ "
tableId
∆∆# *
)
∆∆* +
{
«« 
return
»» 
TableIds
»» 
.
»» 
Contains
»»  
(
»»  !
tableId
»»! (
)
»»( )
;
»») *
}
…… 
public
ŒŒ 

int
ŒŒ 

TableCount
ŒŒ 
=>
ŒŒ 
TableIds
ŒŒ %
.
ŒŒ% &
Count
ŒŒ& +
;
ŒŒ+ ,
public
”” 

void
”” 
Activate
”” 
(
”” 
)
”” 
{
‘‘ 
IsActive
’’ 
=
’’ 
true
’’ 
;
’’ 
	UpdatedAt
÷÷ 
=
÷÷ 
DateTime
÷÷ 
.
÷÷ 
UtcNow
÷÷ #
;
÷÷# $
Version
◊◊ 
++
◊◊ 
;
◊◊ 
}
ÿÿ 
public
›› 

void
›› 

Deactivate
›› 
(
›› 
)
›› 
{
ﬁﬁ 
IsActive
ﬂﬂ 
=
ﬂﬂ 
false
ﬂﬂ 
;
ﬂﬂ 
	UpdatedAt
‡‡ 
=
‡‡ 
DateTime
‡‡ 
.
‡‡ 
UtcNow
‡‡ #
;
‡‡# $
Version
·· 
++
·· 
;
·· 
}
‚‚ 
public
ÁÁ 

bool
ÁÁ 
IsValid
ÁÁ 
(
ÁÁ 
)
ÁÁ 
{
ËË 
return
ÈÈ 
!
ÈÈ 
string
ÈÈ 
.
ÈÈ  
IsNullOrWhiteSpace
ÈÈ )
(
ÈÈ) *
Name
ÈÈ* .
)
ÈÈ. /
&&
ÈÈ0 2
ServerId
ÈÈ3 ;
!=
ÈÈ< >
Guid
ÈÈ? C
.
ÈÈC D
Empty
ÈÈD I
;
ÈÈI J
}
ÍÍ 
}ÎÎ ﬁ;
rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\ServerAssignment.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 
ServerAssignment

 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 
	SessionId 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

Guid 
ServerId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

DateTime 

AssignedAt 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

DateTime 
? 
UnassignedAt !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 

bool 
	IsPrimary 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

decimal  
AllocationPercentage '
{( )
get* -
;- .
private/ 6
set7 :
;: ;
}< =
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
private 
ServerAssignment 
( 
) 
{ 
} 
public%% 

static%% 
ServerAssignment%% "
Create%%# )
(%%) *
Guid&& 
	sessionId&& 
,&& 
Guid'' 
serverId'' 
,'' 
bool(( 
	isPrimary(( 
=(( 
true(( 
,(( 
decimal))  
allocationPercentage)) $
=))% &
$num))' +
)))+ ,
{** 
if++ 

(++ 
	sessionId++ 
==++ 
Guid++ 
.++ 
Empty++ #
)++# $
{,, 	
throw-- 
new-- 
ArgumentException-- '
(--' (
$str--( E
,--E F
nameof--G M
(--M N
	sessionId--N W
)--W X
)--X Y
;--Y Z
}.. 	
if00 

(00 
serverId00 
==00 
Guid00 
.00 
Empty00 "
)00" #
{11 	
throw22 
new22 
ArgumentException22 '
(22' (
$str22( D
,22D E
nameof22F L
(22L M
serverId22M U
)22U V
)22V W
;22W X
}33 	
if55 

(55  
allocationPercentage55  
<=55! #
$num55$ %
||55& ( 
allocationPercentage55) =
>55> ?
$num55@ C
)55C D
{66 	
throw77 
new77 *
BusinessRuleViolationException77 4
(774 5
$str775 f
)77f g
;77g h
}88 	
var:: 
now:: 
=:: 
DateTime:: 
.:: 
UtcNow:: !
;::! "
return<< 
new<< 
ServerAssignment<< #
{== 	
Id>> 
=>> 
Guid>> 
.>> 
NewGuid>> 
(>> 
)>> 
,>>  
	SessionId?? 
=?? 
	sessionId?? !
,??! "
ServerId@@ 
=@@ 
serverId@@ 
,@@  

AssignedAtAA 
=AA 
nowAA 
,AA 
	IsPrimaryBB 
=BB 
	isPrimaryBB !
,BB! " 
AllocationPercentageCC  
=CC! " 
allocationPercentageCC# 7
,CC7 8
	CreatedAtDD 
=DD 
nowDD 
,DD 
	UpdatedAtEE 
=EE 
nowEE 
}FF 	
;FF	 

}GG 
publicMM 

voidMM 
UnassignMM 
(MM 
)MM 
{NN 
ifOO 

(OO 
UnassignedAtOO 
.OO 
HasValueOO !
)OO! "
{PP 	
throwQQ 
newQQ 
SystemQQ 
.QQ %
InvalidOperationExceptionQQ 6
(QQ6 7
$strQQ7 h
)QQh i
;QQi j
}RR 	
UnassignedAtTT 
=TT 
DateTimeTT 
.TT  
UtcNowTT  &
;TT& '
	UpdatedAtUU 
=UU 
DateTimeUU 
.UU 
UtcNowUU #
;UU# $
}VV 
public^^ 

void^^ &
UpdateAllocationPercentage^^ *
(^^* +
decimal^^+ 2 
allocationPercentage^^3 G
)^^G H
{__ 
if`` 

(`` 
UnassignedAt`` 
.`` 
HasValue`` !
)``! "
{aa 	
throwbb 
newbb 
Systembb 
.bb %
InvalidOperationExceptionbb 6
(bb6 7
$strbb7 k
)bbk l
;bbl m
}cc 	
ifee 

(ee  
allocationPercentageee  
<=ee! #
$numee$ %
||ee& ( 
allocationPercentageee) =
>ee> ?
$numee@ C
)eeC D
{ff 	
throwgg 
newgg *
BusinessRuleViolationExceptiongg 4
(gg4 5
$strgg5 f
)ggf g
;ggg h
}hh 	 
AllocationPercentagejj 
=jj  
allocationPercentagejj 3
;jj3 4
	UpdatedAtkk 
=kk 
DateTimekk 
.kk 
UtcNowkk #
;kk# $
}ll 
publicss 

voidss 

SetPrimaryss 
(ss 
boolss 
	isPrimaryss  )
)ss) *
{tt 
ifuu 

(uu 
UnassignedAtuu 
.uu 
HasValueuu !
)uu! "
{vv 	
throwww 
newww 
Systemww 
.ww %
InvalidOperationExceptionww 6
(ww6 7
$strww7 o
)wwo p
;wwp q
}xx 	
	IsPrimaryzz 
=zz 
	isPrimaryzz 
;zz 
	UpdatedAt{{ 
={{ 
DateTime{{ 
.{{ 
UtcNow{{ #
;{{# $
}|| 
public
ÇÇ 

bool
ÇÇ !
IsCurrentlyAssigned
ÇÇ #
(
ÇÇ# $
)
ÇÇ$ %
{
ÉÉ 
return
ÑÑ 
!
ÑÑ 
UnassignedAt
ÑÑ 
.
ÑÑ 
HasValue
ÑÑ %
;
ÑÑ% &
}
ÖÖ 
public
ãã 

TimeSpan
ãã #
GetAssignmentDuration
ãã )
(
ãã) *
)
ãã* +
{
åå 
var
çç 
endTime
çç 
=
çç 
UnassignedAt
çç "
??
çç# %
DateTime
çç& .
.
çç. /
UtcNow
çç/ 5
;
çç5 6
return
éé 
endTime
éé 
-
éé 

AssignedAt
éé #
;
éé# $
}
èè 
}êê ≤
fC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Role.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
Role 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public		 

string		 
Name		 
{		 
get		 
;		 
private		 %
set		& )
;		) *
}		+ ,
=		- .
string		/ 5
.		5 6
Empty		6 ;
;		; <
public

 

UserPermission

 
Permissions

 %
{

& '
get

( +
;

+ ,
private

- 4
set

5 8
;

8 9
}

: ;
private 
Role 
( 
) 
{ 
} 
public 

static 
Role 
Create 
( 
string $
name% )
,) *
UserPermission+ 9
permissions: E
)E F
{ 
if 

( 
string 
. 
IsNullOrWhiteSpace %
(% &
name& *
)* +
)+ ,
throw 
new 
ArgumentException '
(' (
$str( D
,D E
nameofF L
(L M
nameM Q
)Q R
)R S
;S T
return 
new 
Role 
{ 	
Id 
= 
Guid 
. 
NewGuid 
( 
) 
,  
Name 
= 
name 
, 
Permissions 
= 
permissions %
} 	
;	 

} 
public 

void 
UpdatePermissions !
(! "
UserPermission" 0
permissions1 <
)< =
{ 
Permissions 
= 
permissions !
;! "
} 
public   

void   
SetName   
(   
string   
name   #
)  # $
{!! 
if"" 

("" 
string"" 
."" 
IsNullOrWhiteSpace"" %
(""% &
name""& *
)""* +
)""+ ,
throw## 
new## 
ArgumentException## '
(##' (
$str##( D
,##D E
nameof##F L
(##L M
name##M Q
)##Q R
)##R S
;##S T
Name$$ 
=$$ 
name$$ 
;$$ 
}%% 
}&& ª"
yC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\RestaurantConfiguration.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class #
RestaurantConfiguration $
{ 
[ 
Key 
] 	
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
[

 
Required

 
]

 
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
=% &
$str' 6
;6 7
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
Address 
{ 
get 
;  
set! $
;$ %
}& '
=( )
string* 0
.0 1
Empty1 6
;6 7
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
Phone 
{ 
get 
; 
set "
;" #
}$ %
=& '
string( .
.. /
Empty/ 4
;4 5
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
Email 
{ 
get 
; 
set "
;" #
}$ %
=& '
string( .
.. /
Empty/ 4
;4 5
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
Website 
{ 
get 
;  
set! $
;$ %
}& '
=( )
string* 0
.0 1
Empty1 6
;6 7
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string  
ReceiptFooterMessage &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
=5 6
$str7 V
;V W
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
TaxId 
{ 
get 
; 
set "
;" #
}$ %
=& '
string( .
.. /
Empty/ 4
;4 5
[!! 
	MaxLength!! 
(!! 
$num!! 
)!! 
]!! 
public"" 

string"" 
ZipCode"" 
{"" 
get"" 
;""  
set""! $
;""$ %
}""& '
=""( )
string""* 0
.""0 1
Empty""1 6
;""6 7
public$$ 

int$$ 
Capacity$$ 
{$$ 
get$$ 
;$$ 
set$$ "
;$$" #
}$$$ %
=$$& '
$num$$( )
;$$) *
[&& 
	MaxLength&& 
(&& 
$num&& 
)&& 
]&& 
public'' 

string'' 
CurrencySymbol''  
{''! "
get''# &
;''& '
set''( +
;''+ ,
}''- .
=''/ 0
$str''1 4
;''4 5
public)) 

decimal)) #
ServiceChargePercentage)) *
{))+ ,
get))- 0
;))0 1
set))2 5
;))5 6
}))7 8
=))9 :
$num)); <
;))< =
public++ 

decimal++ %
DefaultGratuityPercentage++ ,
{++- .
get++/ 2
;++2 3
set++4 7
;++7 8
}++9 :
=++; <
$num++= >
;++> ?
public-- 

bool-- 
IsKioskMode-- 
{-- 
get-- !
;--! "
set--# &
;--& '
}--( )
=--* +
false--, 1
;--1 2
}.. ãT
oC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\PurchaseOrder.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
PurchaseOrder		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
PONumber 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
=1 2
null3 7
!7 8
;8 9
public 

Guid 
VendorId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
? 
	OrderedAt 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

DateTime 
? 

ReceivedAt 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

PurchaseOrderStatus 
Status %
{& '
get( +
;+ ,
private- 4
set5 8
;8 9
}: ;
public 

decimal 
TotalAmount 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

string 
? 
Notes 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
private 
readonly 
List 
< 
PurchaseOrderLine +
>+ ,
_lines- 3
=4 5
new6 9
(9 :
): ;
;; <
public 

IReadOnlyCollection 
< 
PurchaseOrderLine 0
>0 1
Lines2 7
=>8 :
_lines; A
.A B

AsReadOnlyB L
(L M
)M N
;N O
public 

virtual 
Vendor 
Vendor  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
=7 8
null9 =
!= >
;> ?
private 
PurchaseOrder 
( 
) 
{ 
} 
public 

static 
PurchaseOrder 
Create  &
(& '
string' -
poNumber. 6
,6 7
Guid8 <
vendorId= E
,E F
stringG M
?M N
notesO T
=U V
nullW [
)[ \
{ 
return 
new 
PurchaseOrder  
{ 	
Id   
=   
Guid   
.   
NewGuid   
(   
)   
,    
PONumber!! 
=!! 
poNumber!! 
,!!  
VendorId"" 
="" 
vendorId"" 
,""  
	CreatedAt## 
=## 
DateTime##  
.##  !
UtcNow##! '
,##' (
Status$$ 
=$$ 
PurchaseOrderStatus$$ (
.$$( )
Draft$$) .
,$$. /
Notes%% 
=%% 
notes%% 
}&& 	
;&&	 

}'' 
public)) 

void)) 
AddLine)) 
()) 
Guid)) 
inventoryItemId)) ,
,)), -
decimal)). 5
quantity))6 >
,))> ?
decimal))@ G
unitCost))H P
)))P Q
{** 
if++ 

(++ 
Status++ 
!=++ 
PurchaseOrderStatus++ )
.++) *
Draft++* /
)++/ 0
throw++1 6
new++7 :%
InvalidOperationException++; T
(++T U
$str++U y
)++y z
;++z {
var-- 
existing-- 
=-- 
_lines-- 
.-- 
FirstOrDefault-- ,
(--, -
l--- .
=>--/ 1
l--2 3
.--3 4
InventoryItemId--4 C
==--D F
inventoryItemId--G V
)--V W
;--W X
if.. 

(.. 
existing.. 
!=.. 
null.. 
).. 
{// 	
_lines00 
.00 
Remove00 
(00 
existing00 "
)00" #
;00# $
}11 	
_lines33 
.33 
Add33 
(33 
new33 
PurchaseOrderLine33 (
(33( )
Id33) +
,33+ ,
inventoryItemId33- <
,33< =
quantity33> F
,33F G
unitCost33H P
)33P Q
)33Q R
;33R S
CalculateTotal44 
(44 
)44 
;44 
}55 
public77 

void77 
MarkAsOrdered77 
(77 
)77 
{88 
if99 

(99 
Status99 
!=99 
PurchaseOrderStatus99 )
.99) *
Draft99* /
)99/ 0
throw991 6
new997 :%
InvalidOperationException99; T
(99T U
$str99U o
)99o p
;99p q
Status:: 
=:: 
PurchaseOrderStatus:: $
.::$ %
Ordered::% ,
;::, -
	OrderedAt;; 
=;; 
DateTime;; 
.;; 
UtcNow;; #
;;;# $
}<< 
public>> 

void>> 
MarkAsReceived>> 
(>> 
)>>  
{?? 
if@@ 

(@@ 
Status@@ 
!=@@ 
PurchaseOrderStatus@@ )
.@@) *
Ordered@@* 1
)@@1 2
throw@@3 8
new@@9 <%
InvalidOperationException@@= V
(@@V W
$str@@W u
)@@u v
;@@v w
StatusAA 
=AA 
PurchaseOrderStatusAA $
.AA$ %
ReceivedAA% -
;AA- .

ReceivedAtBB 
=BB 
DateTimeBB 
.BB 
UtcNowBB $
;BB$ %
foreachDD 
(DD 
varDD 
lineDD 
inDD 
_linesDD #
)DD# $
{EE 	
lineFF 
.FF 
MarkAsReceivedFF 
(FF  
)FF  !
;FF! "
}GG 	
}HH 
publicJJ 

voidJJ 
CancelJJ 
(JJ 
)JJ 
{KK 
ifLL 

(LL 
StatusLL 
==LL 
PurchaseOrderStatusLL )
.LL) *
ReceivedLL* 2
)LL2 3
throwLL4 9
newLL: =%
InvalidOperationExceptionLL> W
(LLW X
$strLLX w
)LLw x
;LLx y
StatusMM 
=MM 
PurchaseOrderStatusMM $
.MM$ %
	CancelledMM% .
;MM. /
}NN 
privatePP 
voidPP 
CalculateTotalPP 
(PP  
)PP  !
{QQ 
TotalAmountRR 
=RR 
_linesRR 
.RR 
SumRR  
(RR  !
lRR! "
=>RR# %
lRR& '
.RR' (
SubtotalRR( 0
)RR0 1
;RR1 2
}SS 
}TT 
publicVV 
classVV 
PurchaseOrderLineVV 
{WW 
publicXX 

GuidXX 
IdXX 
{XX 
getXX 
;XX 
privateXX !
setXX" %
;XX% &
}XX' (
publicYY 

GuidYY 
PurchaseOrderIdYY 
{YY  !
getYY" %
;YY% &
privateYY' .
setYY/ 2
;YY2 3
}YY4 5
publicZZ 

GuidZZ 
InventoryItemIdZZ 
{ZZ  !
getZZ" %
;ZZ% &
privateZZ' .
setZZ/ 2
;ZZ2 3
}ZZ4 5
public[[ 

decimal[[ 
QuantityExpected[[ #
{[[$ %
get[[& )
;[[) *
private[[+ 2
set[[3 6
;[[6 7
}[[8 9
public\\ 

decimal\\ 
QuantityReceived\\ #
{\\$ %
get\\& )
;\\) *
private\\+ 2
set\\3 6
;\\6 7
}\\8 9
public]] 

decimal]] 
UnitCost]] 
{]] 
get]] !
;]]! "
private]]# *
set]]+ .
;]]. /
}]]0 1
public^^ 

decimal^^ 
Subtotal^^ 
=>^^ 
QuantityExpected^^ /
*^^0 1
UnitCost^^2 :
;^^: ;
public__ 

bool__ 

IsReceived__ 
{__ 
get__  
;__  !
private__" )
set__* -
;__- .
}__/ 0
publicaa 

virtualaa 
InventoryItemaa  
InventoryItemaa! .
{aa/ 0
getaa1 4
;aa4 5
privateaa6 =
setaa> A
;aaA B
}aaC D
=aaE F
nullaaG K
!aaK L
;aaL M
privatecc 
PurchaseOrderLinecc 
(cc 
)cc 
{cc  !
}cc" #
internalee 
PurchaseOrderLineee 
(ee 
Guidee #
poIdee$ (
,ee( )
Guidee* .
itemIdee/ 5
,ee5 6
decimalee7 >
quantityee? G
,eeG H
decimaleeI P
costeeQ U
)eeU V
{ff 
Idgg 

=gg 
Guidgg 
.gg 
NewGuidgg 
(gg 
)gg 
;gg 
PurchaseOrderIdhh 
=hh 
poIdhh 
;hh 
InventoryItemIdii 
=ii 
itemIdii  
;ii  !
QuantityExpectedjj 
=jj 
quantityjj #
;jj# $
UnitCostkk 
=kk 
costkk 
;kk 
}ll 
internalnn 
voidnn 
MarkAsReceivednn  
(nn  !
)nn! "
{oo 
QuantityReceivedpp 
=pp 
QuantityExpectedpp +
;pp+ ,

IsReceivedqq 
=qq 
trueqq 
;qq 
}rr 
}ss Ω#
sC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\PromotionSchedule.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
PromotionSchedule		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 

DiscountId 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

	DayOfWeek 
	DayOfWeek 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

TimeSpan 
	StartTime 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

TimeSpan 
EndTime 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

virtual 
Discount 
Discount $
{% &
get' *
;* +
private, 3
set4 7
;7 8
}9 :
private 
PromotionSchedule 
( 
) 
{  !
}" #
public 

static 
PromotionSchedule #
Create$ *
(* +
Guid 

discountId 
, 
	DayOfWeek 
	dayOfWeek 
, 
TimeSpan 
	startTime 
, 
TimeSpan 
endTime 
) 
{   
if!! 

(!! 

discountId!! 
==!! 
Guid!! 
.!! 
Empty!! $
)!!$ %
{"" 	
throw## 
new## 
ArgumentException## '
(##' (
$str##( F
,##F G
nameof##H N
(##N O

discountId##O Y
)##Y Z
)##Z [
;##[ \
}$$ 	
if&& 

(&& 
endTime&& 
<=&& 
	startTime&&  
)&&  !
{'' 	
throw(( 
new(( *
BusinessRuleViolationException(( 4
(((4 5
$str((5 Y
)((Y Z
;((Z [
})) 	
return++ 
new++ 
PromotionSchedule++ $
{,, 	
Id-- 
=-- 
Guid-- 
.-- 
NewGuid-- 
(-- 
)-- 
,--  

DiscountId.. 
=.. 

discountId.. #
,..# $
	DayOfWeek// 
=// 
	dayOfWeek// !
,//! "
	StartTime00 
=00 
	startTime00 !
,00! "
EndTime11 
=11 
endTime11 
,11 
IsActive22 
=22 
true22 
}33 	
;33	 

}44 
public66 

void66 

Deactivate66 
(66 
)66 
{77 
IsActive88 
=88 
false88 
;88 
}99 
public;; 

void;; 
Activate;; 
(;; 
);; 
{<< 
IsActive== 
=== 
true== 
;== 
}>> 
publicCC 

boolCC 
IsApplicableCC 
(CC 
DateTimeCC %
dateTimeCC& .
)CC. /
{DD 
ifEE 

(EE 
!EE 
IsActiveEE 
)EE 
returnEE 
falseEE #
;EE# $
ifGG 

(GG 
dateTimeGG 
.GG 
	DayOfWeekGG 
!=GG !
	DayOfWeekGG" +
)GG+ ,
returnGG- 3
falseGG4 9
;GG9 :
varII 
	timeOfDayII 
=II 
dateTimeII  
.II  !
	TimeOfDayII! *
;II* +
returnJJ 
	timeOfDayJJ 
>=JJ 
	StartTimeJJ %
&&JJ& (
	timeOfDayJJ) 2
<=JJ3 5
EndTimeJJ6 =
;JJ= >
}KK 
}LL §&
oC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\PrintTemplate.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
PrintTemplate 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public		 

string		 
Name		 
{		 
get		 
;		 
private		 %
set		& )
;		) *
}		+ ,
=		- .
string		/ 5
.		5 6
Empty		6 ;
;		; <
public

 

TemplateType

 
Type

 
{

 
get

 "
;

" #
private

$ +
set

, /
;

/ 0
}

1 2
public 

string 
Content 
{ 
get 
;  
private! (
set) ,
;, -
}. /
=0 1
string2 8
.8 9
Empty9 >
;> ?
public 

bool 
IsSystem 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

int 
Version 
{ 
get 
; 
private %
set& )
;) *
}+ ,
private 
PrintTemplate 
( 
) 
{ 
} 
public 

static 
PrintTemplate 
Create  &
(& '
string' -
name. 2
,2 3
TemplateType4 @
typeA E
,E F
stringG M
contentN U
,U V
boolW [
isSystem\ d
=e f
falseg l
)l m
{ 
if 

( 
string 
. 
IsNullOrWhiteSpace %
(% &
name& *
)* +
)+ ,
throw- 2
new3 6
ArgumentException7 H
(H I
$strI [
)[ \
;\ ]
if 

( 
string 
. 
IsNullOrWhiteSpace %
(% &
content& -
)- .
). /
throw0 5
new6 9
ArgumentException: K
(K L
$strL a
)a b
;b c
return 
new 
PrintTemplate  
{ 	
Id 
= 
Guid 
. 
NewGuid 
( 
) 
,  
Name 
= 
name 
, 
Type 
= 
type 
, 
Content 
= 
content 
, 
IsSystem 
= 
isSystem 
,  
Version 
= 
$num 
}   	
;  	 

}!! 
public## 

void## 
UpdateContent## 
(## 
string## $
content##% ,
)##, -
{$$ 
if%% 

(%% 
IsSystem%% 
)%% 
throw%% 
new%% %
InvalidOperationException%%  9
(%%9 :
$str%%: [
)%%[ \
;%%\ ]
if&& 

(&& 
string&& 
.&& 
IsNullOrWhiteSpace&& %
(&&% &
content&&& -
)&&- .
)&&. /
throw&&0 5
new&&6 9
ArgumentException&&: K
(&&K L
$str&&L a
)&&a b
;&&b c
Content(( 
=(( 
content(( 
;(( 
})) 
public++ 

void++ 

UpdateName++ 
(++ 
string++ !
name++" &
)++& '
{,, 
if-- 

(-- 
IsSystem-- 
)-- 
throw-- 
new-- %
InvalidOperationException--  9
(--9 :
$str--: [
)--[ \
;--\ ]
if.. 

(.. 
string.. 
... 
IsNullOrWhiteSpace.. %
(..% &
name..& *
)..* +
)..+ ,
throw..- 2
new..3 6
ArgumentException..7 H
(..H I
$str..I [
)..[ \
;..\ ]
Name00 
=00 
name00 
;00 
}11 
public33 

void33 
UpdateIsSystem33 
(33 
bool33 #
isSystem33$ ,
)33, -
{44 
IsSystem55 
=55 
isSystem55 
;55 
}66 
}77 º<
pC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\PrinterMapping.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
PrinterMapping		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 

TerminalId 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

Guid 
PrinterGroupId 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

string 
PhysicalPrinterName %
{& '
get( +
;+ ,
private- 4
set5 8
;8 9
}: ;
=< =
string> D
.D E
EmptyE J
;J K
public 

PrinterFormat 
Format 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

bool 

CutEnabled 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

int 
PaperWidthMm 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

int 
PrintableWidthChars "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
public 

int 
Dpi 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

bool 
SupportsCashDrawer "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
public 

bool 
SupportsImages 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

bool 

SupportsQr 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
private 
PrinterMapping 
( 
) 
{ 
} 
public   

static   
PrinterMapping    
Create  ! '
(  ' (
Guid!! 

terminalId!! 
,!! 
Guid"" 
printerGroupId"" 
,"" 
string## 
physicalPrinterName## "
,##" #
PrinterFormat$$ 
format$$ 
=$$ 
PrinterFormat$$ ,
.$$, -
Thermal80mm$$- 8
,$$8 9
bool%% 

cutEnabled%% 
=%% 
true%% 
)%% 
{&& 
if'' 

('' 

terminalId'' 
=='' 
Guid'' 
.'' 
Empty'' $
)''$ %
throw''& +
new'', /
ArgumentException''0 A
(''A B
$str''B \
)''\ ]
;''] ^
if(( 

((( 
printerGroupId(( 
==(( 
Guid(( "
.((" #
Empty((# (
)((( )
throw((* /
new((0 3
ArgumentException((4 E
(((E F
$str((F e
)((e f
;((f g
if)) 

()) 
string)) 
.)) 
IsNullOrWhiteSpace)) %
())% &
physicalPrinterName))& 9
)))9 :
))): ;
throw))< A
new))B E
ArgumentException))F W
())W X
$str))X |
)))| }
;))} ~
return++ 
new++ 
PrinterMapping++ !
{,, 	
Id-- 
=-- 
Guid-- 
.-- 
NewGuid-- 
(-- 
)-- 
,--  

TerminalId.. 
=.. 

terminalId.. #
,..# $
PrinterGroupId// 
=// 
printerGroupId// +
,//+ ,
PhysicalPrinterName00 
=00  !
physicalPrinterName00" 5
,005 6
Format11 
=11 
format11 
,11 

CutEnabled22 
=22 

cutEnabled22 #
,22# $
PaperWidthMm44 
=44 
format44 !
==44" $
PrinterFormat44% 2
.442 3
Thermal58mm443 >
?44? @
$num44A C
:44D E
$num44F H
,44H I
PrintableWidthChars55 
=55  !
format55" (
==55) +
PrinterFormat55, 9
.559 :
Thermal58mm55: E
?55F G
$num55H J
:55K L
$num55M O
,55O P
Dpi66 
=66 
$num66 
,66 
SupportsCashDrawer77 
=77  
true77! %
,77% &
SupportsImages88 
=88 
true88 !
,88! "

SupportsQr99 
=99 
true99 
}:: 	
;::	 

};; 
public== 

void== 
Update== 
(== 
string== 
physicalPrinterName== 1
)==1 2
{>> 
if?? 

(?? 
string?? 
.?? 
IsNullOrWhiteSpace?? %
(??% &
physicalPrinterName??& 9
)??9 :
)??: ;
throw??< A
new??B E
ArgumentException??F W
(??W X
$str??X |
)??| }
;??} ~
PhysicalPrinterName@@ 
=@@ 
physicalPrinterName@@ 1
;@@1 2
}AA 
publicCC 

voidCC 
UpdateConfigurationCC #
(CC# $
stringDD 
physicalPrinterNameDD "
,DD" #
PrinterFormatEE 
formatEE 
,EE 
boolFF 

cutEnabledFF 
,FF 
intGG 
paperWidthMmGG 
,GG 
intHH 
printableWidthCharsHH 
,HH  
intII 
dpiII 
,II 
boolJJ 
supportsCashDrawerJJ 
,JJ  
boolKK 
supportsImagesKK 
,KK 
boolLL 

supportsQrLL 
)LL 
{MM 
ifNN 

(NN 
stringNN 
.NN 
IsNullOrWhiteSpaceNN %
(NN% &
physicalPrinterNameNN& 9
)NN9 :
)NN: ;
throwNN< A
newNNB E
ArgumentExceptionNNF W
(NNW X
$strNNX |
)NN| }
;NN} ~
PhysicalPrinterNameOO 
=OO 
physicalPrinterNameOO 1
;OO1 2
FormatPP 
=PP 
formatPP 
;PP 

CutEnabledQQ 
=QQ 

cutEnabledQQ 
;QQ  
PaperWidthMmRR 
=RR 
paperWidthMmRR #
;RR# $
PrintableWidthCharsSS 
=SS 
printableWidthCharsSS 1
;SS1 2
DpiTT 
=TT 
dpiTT 
;TT 
SupportsCashDrawerUU 
=UU 
supportsCashDrawerUU /
;UU/ 0
SupportsImagesVV 
=VV 
supportsImagesVV '
;VV' (

SupportsQrWW 
=WW 

supportsQrWW 
;WW  
}XX 
}YY ﬁ1
nC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\PrinterGroup.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 
PrinterGroup

 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

PrinterType 
Type 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

CutBehavior 
CutBehavior "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
public 

bool 

ShowPrices 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

int 

RetryCount 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

int 
RetryDelayMs 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

bool 
AllowReprint 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

Guid 
? "
FallbackPrinterGroupId '
{( )
get* -
;- .
private/ 6
set7 :
;: ;
}< =
public 

Guid 
? 
ReceiptTemplateId "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
public 

virtual 
PrintTemplate  
?  !
ReceiptTemplate" 1
{2 3
get4 7
;7 8
private9 @
setA D
;D E
}F G
public 

Guid 
? 
KitchenTemplateId "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
public 

virtual 
PrintTemplate  
?  !
KitchenTemplate" 1
{2 3
get4 7
;7 8
private9 @
setA D
;D E
}F G
private 
PrinterGroup 
( 
) 
{   
}!! 
public## 

static## 
PrinterGroup## 
Create## %
(##% &
string##& ,
name##- 1
,##1 2
PrinterType##3 >
type##? C
)##C D
{$$ 
if%% 

(%% 
string%% 
.%% 
IsNullOrWhiteSpace%% %
(%%% &
name%%& *
)%%* +
)%%+ ,
throw&& 
new&& 
ArgumentException&& '
(&&' (
$str&&( I
)&&I J
;&&J K
return(( 
new(( 
PrinterGroup(( 
{)) 	
Id** 
=** 
Guid** 
.** 
NewGuid** 
(** 
)** 
,**  
Name++ 
=++ 
name++ 
,++ 
Type,, 
=,, 
type,, 
,,, 
CutBehavior.. 
=.. 
CutBehavior.. %
...% &
Auto..& *
,..* +

ShowPrices// 
=// 
true// 
,// 

RetryCount00 
=00 
$num00 
,00 
RetryDelayMs11 
=11 
$num11 
,11 
AllowReprint22 
=22 
true22 
}33 	
;33	 

}44 
public66 

void66 
Update66 
(66 
string66 
name66 "
,66" #
PrinterType66$ /
type660 4
)664 5
{77 
if88 

(88 
string88 
.88 
IsNullOrWhiteSpace88 %
(88% &
name88& *
)88* +
)88+ ,
throw99 
new99 
ArgumentException99 '
(99' (
$str99( I
)99I J
;99J K
Name;; 
=;; 
name;; 
;;; 
Type<< 
=<< 
type<< 
;<< 
}== 
public?? 

void?? 
UpdateBehavior?? 
(?? 
CutBehavior@@ 
cutBehavior@@ 
,@@  
boolAA 

showPricesAA 
,AA 
intBB 

retryCountBB 
,BB 
intCC 
retryDelayMsCC 
,CC 
boolDD 
allowReprintDD 
,DD 
GuidEE 
?EE "
fallbackPrinterGroupIdEE $
)EE$ %
{FF 
CutBehaviorGG 
=GG 
cutBehaviorGG !
;GG! "

ShowPricesHH 
=HH 

showPricesHH 
;HH  

RetryCountII 
=II 

retryCountII 
;II  
RetryDelayMsJJ 
=JJ 
retryDelayMsJJ #
;JJ# $
AllowReprintKK 
=KK 
allowReprintKK #
;KK# $"
FallbackPrinterGroupIdLL 
=LL  "
fallbackPrinterGroupIdLL! 7
;LL7 8
}MM 
publicNN 

voidNN 
SetTemplatesNN 
(NN 
GuidNN !
?NN! "
receiptTemplateIdNN# 4
,NN4 5
GuidNN6 :
?NN: ;
kitchenTemplateIdNN< M
)NNM N
{OO 
ReceiptTemplateIdPP 
=PP 
receiptTemplateIdPP -
;PP- .
KitchenTemplateIdQQ 
=QQ 
kitchenTemplateIdQQ -
;QQ- .
}RR 
}SS ù
hC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Payout.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
Payout		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 
CashSessionId 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

Money 
Amount 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

string 
? 
Reason 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

UserId 
ProcessedBy 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
null6 :
!: ;
;; <
public 

DateTime 
ProcessedAt 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
private 
Payout 
( 
) 
{ 
Amount 
= 
Money 
. 
Zero 
( 
) 
; 
} 
public 

static 
Payout 
Create 
(  
Guid 
cashSessionId 
, 
Money 
amount 
, 
UserId 
processedBy 
, 
string 
? 
reason 
= 
null 
) 
{ 
if 

( 
amount 
<= 
Money 
. 
Zero  
(  !
)! "
)" #
{ 	
throw 
new 

Exceptions  
.  !*
BusinessRuleViolationException! ?
(? @
$str@ j
)j k
;k l
}   	
return"" 
new"" 
Payout"" 
{## 	
Id$$ 
=$$ 
Guid$$ 
.$$ 
NewGuid$$ 
($$ 
)$$ 
,$$  
CashSessionId%% 
=%% 
cashSessionId%% )
,%%) *
Amount&& 
=&& 
amount&& 
,&& 
Reason'' 
='' 
reason'' 
,'' 
ProcessedBy(( 
=(( 
processedBy(( %
,((% &
ProcessedAt)) 
=)) 
DateTime)) "
.))" #
UtcNow))# )
}** 	
;**	 

}++ 
},, Ÿ
nC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\PaymentBatch.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
PaymentBatch 
{ 
public		 

Guid		 
Id		 
{		 
get		 
;		 
private		 !
set		" %
;		% &
}		' (
public

 

Guid

 

TerminalId

 
{

 
get

  
;

  !
private

" )
set

* -
;

- .
}

/ 0
public 

PaymentBatchStatus 
Status $
{% &
get' *
;* +
private, 3
set4 7
;7 8
}9 :
public 

DateTime 
OpenedAt 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

DateTime 
? 
ClosedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

string 
? 
GatewayBatchId !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
	protected 
PaymentBatch 
( 
) 
{ 
}  
public 

PaymentBatch 
( 
Guid 

terminalId '
)' (
{ 
Id 

= 
Guid 
. 
NewGuid 
( 
) 
; 

TerminalId 
= 

terminalId 
;  
Status 
= 
PaymentBatchStatus #
.# $
Open$ (
;( )
OpenedAt 
= 
DateTime 
. 
UtcNow "
;" #
} 
public 

void 
Close 
( 
string 
? 
gatewayBatchId ,
=- .
null/ 3
)3 4
{ 
Status   
=   
PaymentBatchStatus   #
.  # $
Closed  $ *
;  * +
ClosedAt!! 
=!! 
DateTime!! 
.!! 
UtcNow!! "
;!!" #
GatewayBatchId"" 
="" 
gatewayBatchId"" '
;""' (
}## 
}$$ Ò≠
iC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Payment.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
abstract 
class 
Payment 
{ 
public 

Guid 
Id 
{ 
get 
; 
	protected #
set$ '
;' (
}) *
public 

string 
? 
GlobalId 
{ 
get !
;! "
	protected# ,
set- 0
;0 1
}2 3
public 

Guid 
TicketId 
{ 
get 
; 
	protected  )
set* -
;- .
}/ 0
public 

TransactionType 
TransactionType *
{+ ,
get- 0
;0 1
	protected2 ;
set< ?
;? @
}A B
public 

PaymentType 
PaymentType "
{# $
get% (
;( )
	protected* 3
set4 7
;7 8
}9 :
public 

Money 
Amount 
{ 
get 
; 
	protected (
set) ,
;, -
}. /
public 

Money 

TipsAmount 
{ 
get !
;! "
	protected# ,
set- 0
;0 1
}2 3
public 

Money 
TipsExceedAmount !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 

Money 
TenderAmount 
{ 
get  #
;# $
	protected% .
set/ 2
;2 3
}4 5
public 

Money 
ChangeAmount 
{ 
get  #
;# $
	protected% .
set/ 2
;2 3
}4 5
	protected 
void 
SetTipsAmount  
(  !
Money! &

tipsAmount' 1
)1 2
{ 

TipsAmount 
= 

tipsAmount 
;  
} 
	protected!! 
void!! 
SetTenderAmount!! "
(!!" #
Money!!# (
tenderAmount!!) 5
)!!5 6
{"" 
TenderAmount## 
=## 
tenderAmount## #
;### $
}$$ 
	protected)) 
void)) 
SetChangeAmount)) "
())" #
Money))# (
changeAmount))) 5
)))5 6
{** 
ChangeAmount++ 
=++ 
changeAmount++ #
;++# $
},, 
	protected11 
void11 
SetCashSessionId11 #
(11# $
Guid11$ (
cashSessionId11) 6
)116 7
{22 
CashSessionId33 
=33 
cashSessionId33 %
;33% &
}44 
public55 

DateTime55 
TransactionTime55 #
{55$ %
get55& )
;55) *
	protected55+ 4
set555 8
;558 9
}55: ;
public66 

UserId66 
ProcessedBy66 
{66 
get66  #
;66# $
	protected66% .
set66/ 2
;662 3
}664 5
=666 7
null668 <
!66< =
;66= >
public77 

Guid77 

TerminalId77 
{77 
get77  
;77  !
	protected77" +
set77, /
;77/ 0
}771 2
public88 

bool88 

IsCaptured88 
{88 
get88  
;88  !
	protected88" +
set88, /
;88/ 0
}881 2
public99 

bool99 
IsVoided99 
{99 
get99 
;99 
	protected99  )
set99* -
;99- .
}99/ 0
public:: 

bool:: 
IsAuthorizable:: 
{::  
get::! $
;::$ %
	protected::& /
set::0 3
;::3 4
}::5 6
public;; 

Guid;; 
?;; 
CashSessionId;; 
{;;  
get;;! $
;;;$ %
	protected;;& /
set;;0 3
;;;3 4
};;5 6
public<< 

Guid<< 
?<< 
BatchId<< 
{<< 
get<< 
;<< 
	protected<<  )
set<<* -
;<<- .
}<</ 0
public== 

string== 
?== 
Note== 
{== 
get== 
;== 
	protected== (
set==) ,
;==, -
}==. /
public@@ 

Guid@@ 
?@@ 
SplitGroupId@@ 
{@@ 
get@@  #
;@@# $
	protected@@% .
set@@/ 2
;@@2 3
}@@4 5
publicAA 

intAA 
?AA 
SplitSequenceAA 
{AA 
getAA  #
;AA# $
	protectedAA% .
setAA/ 2
;AA2 3
}AA4 5
publicBB 

MoneyBB 
RefundedAmountBB 
{BB  !
getBB" %
;BB% &
	protectedBB' 0
setBB1 4
;BB4 5
}BB6 7
publicCC 

boolCC 

IsRefundedCC 
{CC 
getCC  
;CC  !
	protectedCC" +
setCC, /
;CC/ 0
}CC1 2
publicHH 

voidHH 

SetBatchIdHH 
(HH 
GuidHH 
batchIdHH  '
)HH' (
{II 
BatchIdJJ 
=JJ 
batchIdJJ 
;JJ 
}KK 
	protectedMM 
PaymentMM 
(MM 
)MM 
{NN 
AmountOO 
=OO 
MoneyOO 
.OO 
ZeroOO 
(OO 
)OO 
;OO 

TipsAmountPP 
=PP 
MoneyPP 
.PP 
ZeroPP 
(PP  
)PP  !
;PP! "
TipsExceedAmountQQ 
=QQ 
MoneyQQ  
.QQ  !
ZeroQQ! %
(QQ% &
)QQ& '
;QQ' (
TenderAmountRR 
=RR 
MoneyRR 
.RR 
ZeroRR !
(RR! "
)RR" #
;RR# $
ChangeAmountSS 
=SS 
MoneySS 
.SS 
ZeroSS !
(SS! "
)SS" #
;SS# $
RefundedAmountTT 
=TT 
MoneyTT 
.TT 
ZeroTT #
(TT# $
)TT$ %
;TT% &

IsRefundedUU 
=UU 
falseUU 
;UU 
}VV 
	protectedXX 
PaymentXX 
(XX 
GuidYY 
ticketIdYY 
,YY 
PaymentTypeZZ 
paymentTypeZZ 
,ZZ  
Money[[ 
amount[[ 
,[[ 
UserId\\ 
processedBy\\ 
,\\ 
Guid]] 

terminalId]] 
,]] 
string^^ 
?^^ 
globalId^^ 
=^^ 
null^^ 
,^^  
Guid__ 
?__ 
splitGroupId__ 
=__ 
null__ !
,__! "
int`` 
?`` 
splitSequence`` 
=`` 
null`` !
)``! "
{aa 
Idbb 

=bb 
Guidbb 
.bb 
NewGuidbb 
(bb 
)bb 
;bb 
GlobalIdcc 
=cc 
globalIdcc 
;cc 
TicketIddd 
=dd 
ticketIddd 
;dd 
PaymentTypeee 
=ee 
paymentTypeee !
;ee! "
TransactionTypeff 
=ff 
TransactionTypeff )
.ff) *
Creditff* 0
;ff0 1
Amountgg 
=gg 
amountgg 
;gg 

TipsAmounthh 
=hh 
Moneyhh 
.hh 
Zerohh 
(hh  
)hh  !
;hh! "
TipsExceedAmountii 
=ii 
Moneyii  
.ii  !
Zeroii! %
(ii% &
)ii& '
;ii' (
TenderAmountjj 
=jj 
Moneyjj 
.jj 
Zerojj !
(jj! "
)jj" #
;jj# $
ChangeAmountkk 
=kk 
Moneykk 
.kk 
Zerokk !
(kk! "
)kk" #
;kk# $
RefundedAmountll 
=ll 
Moneyll 
.ll 
Zeroll #
(ll# $
)ll$ %
;ll% &
TransactionTimemm 
=mm 
DateTimemm "
.mm" #
UtcNowmm# )
;mm) *
ProcessedBynn 
=nn 
processedBynn !
;nn! "

TerminalIdoo 
=oo 

terminalIdoo 
;oo  

IsCapturedpp 
=pp 
falsepp 
;pp 
IsVoidedqq 
=qq 
falseqq 
;qq 
IsAuthorizablerr 
=rr 
falserr 
;rr 

IsRefundedss 
=ss 
falsess 
;ss 
SplitGroupIdtt 
=tt 
splitGroupIdtt #
;tt# $
SplitSequenceuu 
=uu 
splitSequenceuu %
;uu% &
}vv 
public|| 

static|| 
Payment|| 
Create||  
(||  !
Guid}} 
ticketId}} 
,}} 
PaymentType~~ 
paymentType~~ 
,~~  
Money 
amount 
, 
UserId
ÄÄ 
processedBy
ÄÄ 
,
ÄÄ 
Guid
ÅÅ 

terminalId
ÅÅ 
,
ÅÅ 
string
ÇÇ 
?
ÇÇ 
globalId
ÇÇ 
=
ÇÇ 
null
ÇÇ 
)
ÇÇ  
{
ÉÉ 
if
ÑÑ 

(
ÑÑ 
paymentType
ÑÑ 
!=
ÑÑ 
PaymentType
ÑÑ &
.
ÑÑ& '
Cash
ÑÑ' +
)
ÑÑ+ ,
{
ÖÖ 	
throw
ÜÜ 
new
ÜÜ '
InvalidOperationException
ÜÜ /
(
ÜÜ/ 0
$"
áá 
$str
áá ?
{
áá? @
paymentType
áá@ K
}
ááK L
$str
ááL N
"
ááN O
+
ááP Q
$"
àà 
$str
àà 5
"
àà5 6
)
àà6 7
;
àà7 8
}
ââ 	
return
ãã 
CashPayment
ãã 
.
ãã 
Create
ãã !
(
ãã! "
ticketId
ãã" *
,
ãã* +
amount
ãã, 2
,
ãã2 3
processedBy
ãã4 ?
,
ãã? @

terminalId
ããA K
,
ããK L
globalId
ããM U
)
ããU V
;
ããV W
}
åå 
public
éé 

void
éé 
Void
éé 
(
éé 
)
éé 
{
èè 
if
êê 

(
êê 
IsVoided
êê 
)
êê 
{
ëë 	
throw
íí 
new
íí 

Exceptions
íí  
.
íí  !'
InvalidOperationException
íí! :
(
íí: ;
$str
íí; W
)
ííW X
;
ííX Y
}
ìì 	
IsVoided
ïï 
=
ïï 
true
ïï 
;
ïï 
}
ññ 
public
òò 

void
òò 
Capture
òò 
(
òò 
)
òò 
{
ôô 
if
öö 

(
öö 
!
öö 
IsAuthorizable
öö 
)
öö 
{
õõ 	
throw
úú 
new
úú 

Exceptions
úú  
.
úú  !'
InvalidOperationException
úú! :
(
úú: ;
$str
úú; k
)
úúk l
;
úúl m
}
ùù 	
if
üü 

(
üü 

IsCaptured
üü 
)
üü 
{
†† 	
throw
°° 
new
°° 

Exceptions
°°  
.
°°  !'
InvalidOperationException
°°! :
(
°°: ;
$str
°°; Y
)
°°Y Z
;
°°Z [
}
¢¢ 	

IsCaptured
§§ 
=
§§ 
true
§§ 
;
§§ 
}
•• 
public
™™ 

virtual
™™ 
void
™™ 
AddTips
™™ 
(
™™  
Money
™™  %

tipsAmount
™™& 0
)
™™0 1
{
´´ 
if
¨¨ 

(
¨¨ 

tipsAmount
¨¨ 
<
¨¨ 
Money
¨¨ 
.
¨¨ 
Zero
¨¨ #
(
¨¨# $
)
¨¨$ %
)
¨¨% &
{
≠≠ 	
throw
ÆÆ 
new
ÆÆ 

Exceptions
ÆÆ  
.
ÆÆ  !,
BusinessRuleViolationException
ÆÆ! ?
(
ÆÆ? @
$str
ÆÆ@ a
)
ÆÆa b
;
ÆÆb c
}
ØØ 	
if
±± 

(
±± 
IsVoided
±± 
)
±± 
{
≤≤ 	
throw
≥≥ 
new
≥≥ 

Exceptions
≥≥  
.
≥≥  !'
InvalidOperationException
≥≥! :
(
≥≥: ;
$str
≥≥; a
)
≥≥a b
;
≥≥b c
}
¥¥ 	
SetTipsAmount
∂∂ 
(
∂∂ 

tipsAmount
∂∂  
)
∂∂  !
;
∂∂! "
}
∑∑ 
public
ΩΩ 

static
ΩΩ 
Payment
ΩΩ 
CreateRefund
ΩΩ &
(
ΩΩ& '
Payment
ææ 
originalPayment
ææ 
,
ææ  
Money
øø 
refundAmount
øø 
,
øø 
UserId
¿¿ 
processedBy
¿¿ 
,
¿¿ 
Guid
¡¡ 

terminalId
¡¡ 
,
¡¡ 
string
¬¬ 
?
¬¬ 
reason
¬¬ 
=
¬¬ 
null
¬¬ 
,
¬¬ 
string
√√ 
?
√√ 
globalId
√√ 
=
√√ 
null
√√ 
)
√√  
{
ƒƒ 
if
≈≈ 

(
≈≈ 
originalPayment
≈≈ 
==
≈≈ 
null
≈≈ #
)
≈≈# $
{
∆∆ 	
throw
«« 
new
«« #
ArgumentNullException
«« +
(
««+ ,
nameof
««, 2
(
««2 3
originalPayment
««3 B
)
««B C
)
««C D
;
««D E
}
»» 	
if
   

(
   
refundAmount
   
<=
   
Money
   !
.
  ! "
Zero
  " &
(
  & '
)
  ' (
)
  ( )
{
ÀÀ 	
throw
ÃÃ 
new
ÃÃ 

Exceptions
ÃÃ  
.
ÃÃ  !,
BusinessRuleViolationException
ÃÃ! ?
(
ÃÃ? @
$str
ÃÃ@ j
)
ÃÃj k
;
ÃÃk l
}
ÕÕ 	
if
œœ 

(
œœ 
refundAmount
œœ 
>
œœ 
originalPayment
œœ *
.
œœ* +
Amount
œœ+ 1
)
œœ1 2
{
–– 	
throw
—— 
new
—— 

Exceptions
——  
.
——  !,
BusinessRuleViolationException
——! ?
(
——? @
$"
““ 
$str
““ !
{
““! "
refundAmount
““" .
}
““. /
$str
““/ X
{
““X Y
originalPayment
““Y h
.
““h i
Amount
““i o
}
““o p
$str
““p r
"
““r s
)
““s t
;
““t u
}
”” 	
if
’’ 

(
’’ 
originalPayment
’’ 
.
’’ 
IsVoided
’’ $
)
’’$ %
{
÷÷ 	
throw
◊◊ 
new
◊◊ 

Exceptions
◊◊  
.
◊◊  !'
InvalidOperationException
◊◊! :
(
◊◊: ;
$str
◊◊; \
)
◊◊\ ]
;
◊◊] ^
}
ÿÿ 	
Payment
‹‹ 
refundPayment
‹‹ 
=
‹‹ 
originalPayment
‹‹  /
.
‹‹/ 0
PaymentType
‹‹0 ;
switch
‹‹< B
{
›› 	
PaymentType
ﬁﬁ 
.
ﬁﬁ 
Cash
ﬁﬁ 
=>
ﬁﬁ 
CashPayment
ﬁﬁ  +
.
ﬁﬁ+ ,
Create
ﬁﬁ, 2
(
ﬁﬁ2 3
originalPayment
ﬂﬂ 
.
ﬂﬂ  
TicketId
ﬂﬂ  (
,
ﬂﬂ( )
refundAmount
‡‡ 
,
‡‡ 
processedBy
·· 
,
·· 

terminalId
‚‚ 
,
‚‚ 
globalId
„„ 
)
„„ 
,
„„ 
PaymentType
‰‰ 
.
‰‰ 

CreditCard
‰‰ "
=>
‰‰# %
CreditCardPayment
‰‰& 7
.
‰‰7 8
Create
‰‰8 >
(
‰‰> ?
originalPayment
ÂÂ 
.
ÂÂ  
TicketId
ÂÂ  (
,
ÂÂ( )
refundAmount
ÊÊ 
,
ÊÊ 
processedBy
ÁÁ 
,
ÁÁ 

terminalId
ËË 
,
ËË 
globalId
ÈÈ 
:
ÈÈ 
globalId
ÈÈ "
)
ÈÈ" #
,
ÈÈ# $
PaymentType
ÍÍ 
.
ÍÍ 
	DebitCard
ÍÍ !
=>
ÍÍ" $
DebitCardPayment
ÍÍ% 5
.
ÍÍ5 6
Create
ÍÍ6 <
(
ÍÍ< =
originalPayment
ÎÎ 
.
ÎÎ  
TicketId
ÎÎ  (
,
ÎÎ( )
refundAmount
ÏÏ 
,
ÏÏ 
processedBy
ÌÌ 
,
ÌÌ 

terminalId
ÓÓ 
,
ÓÓ 
globalId
ÔÔ 
:
ÔÔ 
globalId
ÔÔ "
)
ÔÔ" #
,
ÔÔ# $
PaymentType
 
.
 
GiftCertificate
 '
=>
( *
originalPayment
ÒÒ 
is
ÒÒ  "$
GiftCertificatePayment
ÒÒ# 9
	gcPayment
ÒÒ: C
?
ÚÚ $
GiftCertificatePayment
ÚÚ ,
.
ÚÚ, -
Create
ÚÚ- 3
(
ÚÚ3 4
originalPayment
ÛÛ '
.
ÛÛ' (
TicketId
ÛÛ( 0
,
ÛÛ0 1
refundAmount
ÙÙ $
,
ÙÙ$ %
processedBy
ıı #
,
ıı# $

terminalId
ˆˆ "
,
ˆˆ" #
	gcPayment
˜˜ !
.
˜˜! "#
GiftCertificateNumber
˜˜" 7
,
˜˜7 8
	gcPayment
¯¯ !
.
¯¯! "
OriginalAmount
¯¯" 0
,
¯¯0 1
	gcPayment
˘˘ !
.
˘˘! "
RemainingBalance
˘˘" 2
+
˘˘3 4
refundAmount
˘˘5 A
,
˘˘A B
globalId
˙˙  
)
˙˙  !
:
˚˚ 
throw
˚˚ 
new
˚˚ 

Exceptions
˚˚  *
.
˚˚* +'
InvalidOperationException
˚˚+ D
(
˚˚D E
$str˚˚E ä
)˚˚ä ã
,˚˚ã å
PaymentType
¸¸ 
.
¸¸ 
CustomPayment
¸¸ %
=>
¸¸& (
originalPayment
˝˝ 
is
˝˝  "
CustomPayment
˝˝# 0
customPayment
˝˝1 >
?
˛˛ 
CustomPayment
˛˛ #
.
˛˛# $
Create
˛˛$ *
(
˛˛* +
originalPayment
ˇˇ '
.
ˇˇ' (
TicketId
ˇˇ( 0
,
ˇˇ0 1
refundAmount
ÄÄ $
,
ÄÄ$ %
processedBy
ÅÅ #
,
ÅÅ# $

terminalId
ÇÇ "
,
ÇÇ" #
customPayment
ÉÉ %
.
ÉÉ% &
PaymentName
ÉÉ& 1
,
ÉÉ1 2
null
ÑÑ 
,
ÑÑ 
null
ÖÖ 
,
ÖÖ 
globalId
ÜÜ  
)
ÜÜ  !
:
áá 
throw
áá 
new
áá 

Exceptions
áá  *
.
áá* +'
InvalidOperationException
áá+ D
(
ááD E
$str
ááE |
)
áá| }
,
áá} ~
_
àà 
=>
àà 
throw
àà 
new
àà 

Exceptions
àà %
.
àà% &'
InvalidOperationException
àà& ?
(
àà? @
$"
àà@ B
$str
ààB h
{
ààh i
originalPayment
àài x
.
ààx y
PaymentTypeàày Ñ
}ààÑ Ö
$strààÖ Ü
"ààÜ á
)ààá à
}
ââ 	
;
ââ	 

var
åå %
transactionTypeProperty
åå #
=
åå$ %
typeof
åå& ,
(
åå, -
Payment
åå- 4
)
åå4 5
.
åå5 6
GetProperty
åå6 A
(
ååA B
$str
ååB S
,
ååS T
System
çç 
.
çç 

Reflection
çç 
.
çç 
BindingFlags
çç *
.
çç* +
Instance
çç+ 3
|
çç4 5
System
çç6 <
.
çç< =

Reflection
çç= G
.
ççG H
BindingFlags
ççH T
.
ççT U
Public
ççU [
|
çç\ ]
System
çç^ d
.
ççd e

Reflection
ççe o
.
çço p
BindingFlags
ççp |
.
çç| }
	NonPublicçç} Ü
)ççÜ á
;ççá à%
transactionTypeProperty
éé 
?
éé  
.
éé  !
SetValue
éé! )
(
éé) *
refundPayment
éé* 7
,
éé7 8
TransactionType
éé9 H
.
ééH I
Debit
ééI N
)
ééN O
;
ééO P
var
ëë 
noteProperty
ëë 
=
ëë 
typeof
ëë !
(
ëë! "
Payment
ëë" )
)
ëë) *
.
ëë* +
GetProperty
ëë+ 6
(
ëë6 7
$str
ëë7 =
,
ëë= >
System
íí 
.
íí 

Reflection
íí 
.
íí 
BindingFlags
íí *
.
íí* +
Instance
íí+ 3
|
íí4 5
System
íí6 <
.
íí< =

Reflection
íí= G
.
ííG H
BindingFlags
ííH T
.
ííT U
Public
ííU [
|
íí\ ]
System
íí^ d
.
ííd e

Reflection
ííe o
.
íío p
BindingFlags
ííp |
.
íí| }
	NonPublicíí} Ü
)ííÜ á
;ííá à
noteProperty
ìì 
?
ìì 
.
ìì 
SetValue
ìì 
(
ìì 
refundPayment
ìì ,
,
ìì, -
$"
ìì. 0
$str
ìì0 B
{
ììB C
originalPayment
ììC R
.
ììR S
Id
ììS U
}
ììU V
$str
ììV `
{
ìì` a
reason
ììa g
??
ììh j
$str
ììk p
}
ììp q
"
ììq r
)
ììr s
;
ììs t
return
ïï 
refundPayment
ïï 
;
ïï 
}
ññ 
}óó ≥B
kC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\OrderType.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 
	OrderType

 
{ 
private 
readonly 

Dictionary 
<  
string  &
,& '
string( .
>. /
_properties0 ;
=< =
new> A
(A B
)B C
;C D
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
null/ 3
!3 4
;4 5
public 

bool 
CloseOnPaid 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

bool 
AllowSeatBasedOrder #
{$ %
get& )
;) *
private+ 2
set3 6
;6 7
}8 9
public 

bool 
AllowToAddTipsLater #
{$ %
get& )
;) *
private+ 2
set3 6
;6 7
}8 9
public 

bool 
IsBarTab 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

int 
Version 
{ 
get 
; 
private %
set& )
;) *
}+ ,
public 

IReadOnlyDictionary 
< 
string %
,% &
string' -
>- .

Properties/ 9
=>: <
_properties= H
;H I
private 
	OrderType 
( 
) 
{ 
Name 
= 
string 
. 
Empty 
; 
} 
public!! 

static!! 
	OrderType!! 
Create!! "
(!!" #
string"" 
name"" 
,"" 
bool## 
closeOnPaid## 
=## 
false##  
,##  !
bool$$ 
allowSeatBasedOrder$$  
=$$! "
false$$# (
,$$( )
bool%% 
allowToAddTipsLater%%  
=%%! "
false%%# (
,%%( )
bool&& 
isBarTab&& 
=&& 
false&& 
,&& 
bool'' 
isActive'' 
='' 
true'' 
)'' 
{(( 
if)) 

()) 
string)) 
.)) 
IsNullOrWhiteSpace)) %
())% &
name))& *
)))* +
)))+ ,
{** 	
throw++ 
new++ 

Exceptions++  
.++  !*
BusinessRuleViolationException++! ?
(++? @
$str++@ b
)++b c
;++c d
},, 	
return.. 
new.. 
	OrderType.. 
{// 	
Id00 
=00 
Guid00 
.00 
NewGuid00 
(00 
)00 
,00  
Name11 
=11 
name11 
,11 
CloseOnPaid22 
=22 
closeOnPaid22 %
,22% &
AllowSeatBasedOrder33 
=33  !
allowSeatBasedOrder33" 5
,335 6
AllowToAddTipsLater44 
=44  !
allowToAddTipsLater44" 5
,445 6
IsBarTab55 
=55 
isBarTab55 
,55  
IsActive66 
=66 
isActive66 
,66  
Version77 
=77 
$num77 
}88 	
;88	 

}99 
public>> 

void>> 

UpdateName>> 
(>> 
string>> !
name>>" &
)>>& '
{?? 
if@@ 

(@@ 
string@@ 
.@@ 
IsNullOrWhiteSpace@@ %
(@@% &
name@@& *
)@@* +
)@@+ ,
{AA 	
throwBB 
newBB 

ExceptionsBB  
.BB  !*
BusinessRuleViolationExceptionBB! ?
(BB? @
$strBB@ b
)BBb c
;BBc d
}CC 	
NameEE 
=EE 
nameEE 
;EE 
}FF 
publicKK 

voidKK 
SetCloseOnPaidKK 
(KK 
boolKK #
closeOnPaidKK$ /
)KK/ 0
{LL 
CloseOnPaidMM 
=MM 
closeOnPaidMM !
;MM! "
}NN 
publicSS 

voidSS "
SetAllowSeatBasedOrderSS &
(SS& '
boolSS' +
allowSeatBasedOrderSS, ?
)SS? @
{TT 
AllowSeatBasedOrderUU 
=UU 
allowSeatBasedOrderUU 1
;UU1 2
}VV 
public[[ 

void[[ "
SetAllowToAddTipsLater[[ &
([[& '
bool[[' +
allowToAddTipsLater[[, ?
)[[? @
{\\ 
AllowToAddTipsLater]] 
=]] 
allowToAddTipsLater]] 1
;]]1 2
}^^ 
publiccc 

voidcc 
SetIsBarTabcc 
(cc 
boolcc  
isBarTabcc! )
)cc) *
{dd 
IsBarTabee 
=ee 
isBarTabee 
;ee 
}ff 
publickk 

voidkk 
Activatekk 
(kk 
)kk 
{ll 
IsActivemm 
=mm 
truemm 
;mm 
}nn 
publicss 

voidss 

Deactivatess 
(ss 
)ss 
{tt 
IsActiveuu 
=uu 
falseuu 
;uu 
}vv 
public{{ 

void{{ 
SetProperty{{ 
({{ 
string{{ "
key{{# &
,{{& '
string{{( .
value{{/ 4
){{4 5
{|| 
if}} 

(}} 
string}} 
.}} 
IsNullOrWhiteSpace}} %
(}}% &
key}}& )
)}}) *
)}}* +
{~~ 	
throw 
new 

Exceptions  
.  !*
BusinessRuleViolationException! ?
(? @
$str@ _
)_ `
;` a
}
ÄÄ 	
_properties
ÇÇ 
[
ÇÇ 
key
ÇÇ 
]
ÇÇ 
=
ÇÇ 
value
ÇÇ  
;
ÇÇ  !
}
ÉÉ 
public
àà 

void
àà 
RemoveProperty
àà 
(
àà 
string
àà %
key
àà& )
)
àà) *
{
ââ 
_properties
ää 
.
ää 
Remove
ää 
(
ää 
key
ää 
)
ää 
;
ää  
}
ãã 
public
êê 

string
êê 
?
êê 
GetProperty
êê 
(
êê 
string
êê %
key
êê& )
)
êê) *
{
ëë 
return
íí 
_properties
íí 
.
íí 
TryGetValue
íí &
(
íí& '
key
íí' *
,
íí* +
out
íí, /
var
íí0 3
value
íí4 9
)
íí9 :
?
íí; <
value
íí= B
:
ííC D
null
ííE I
;
ííI J
}
ìì 
public
ññ 

bool
ññ 
RequiresTable
ññ 
=>
ññ  
GetProperty
ññ! ,
(
ññ, -
$str
ññ- <
)
ññ< =
?
ññ= >
.
ññ> ?
ToLower
ññ? F
(
ññF G
)
ññG H
==
ññI K
$str
ññL R
;
ññR S
public
óó 

bool
óó 
RequiresCustomer
óó  
=>
óó! #
GetProperty
óó$ /
(
óó/ 0
$str
óó0 B
)
óóB C
?
óóC D
.
óóD E
ToLower
óóE L
(
óóL M
)
óóM N
==
óóO Q
$str
óóR X
;
óóX Y
}òò †}
sC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\OrderLineModifier.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 
OrderLineModifier

 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 
OrderLineId 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

Guid 
? 

ModifierId 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

Guid 
? 
ModifierGroupId  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
public 

Guid 
? #
MenuItemModifierGroupId (
{) *
get+ .
;. /
private0 7
set8 ;
;; <
}= >
public 

Guid 
? %
ParentOrderLineModifierId *
{+ ,
get- 0
;0 1
private2 9
set: =
;= >
}? @
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

ModifierType 
ModifierType $
{% &
get' *
;* +
private, 3
set4 7
;7 8
}9 :
public 

int 
	ItemCount 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

Money 
	UnitPrice 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

Money 
	BasePrice 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

decimal 
PortionValue 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
=6 7
$num8 <
;< =
public 

decimal 
TaxRate 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

Money 
	TaxAmount 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

Money 
SubtotalAmount 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

Money 
TotalAmount 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

bool  
ShouldPrintToKitchen $
{% &
get' *
;* +
private, 3
set4 7
;7 8
}9 :
public 

bool 
PrintedToKitchen  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
public 

string 
? 
MultiplierName !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 

string 
? 
SectionName 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public   

bool   
IsSectionWisePrice   "
{  # $
get  % (
;  ( )
private  * 1
set  2 5
;  5 6
}  7 8
public!! 

PriceStrategy!! 
?!! 
PriceStrategy!! '
{!!( )
get!!* -
;!!- .
private!!/ 6
set!!7 :
;!!: ;
}!!< =
public"" 

DateTime"" 
	CreatedAt"" 
{"" 
get""  #
;""# $
private""% ,
set""- 0
;""0 1
}""2 3
private$$ 
OrderLineModifier$$ 
($$ 
)$$ 
{%% 
	UnitPrice&& 
=&& 
Money&& 
.&& 
Zero&& 
(&& 
)&&  
;&&  !
	BasePrice'' 
='' 
Money'' 
.'' 
Zero'' 
('' 
)''  
;''  !
	TaxAmount(( 
=(( 
Money(( 
.(( 
Zero(( 
((( 
)((  
;((  !
SubtotalAmount)) 
=)) 
Money)) 
.)) 
Zero)) #
())# $
)))$ %
;))% &
TotalAmount** 
=** 
Money** 
.** 
Zero**  
(**  !
)**! "
;**" #
}++ 
public-- 

static-- 
OrderLineModifier-- #
Create--$ *
(--* +
Guid.. 
orderLineId.. 
,.. 
Guid// 
?// 

modifierId// 
,// 
string00 
name00 
,00 
ModifierType11 
modifierType11 !
,11! "
int22 
	itemCount22 
,22 
Money33 
	unitPrice33 
,33 
Money44 
	basePrice44 
,44 
decimal55 
portionValue55 
=55 
$num55 #
,55# $
decimal66 
taxRate66 
=66 
$num66 
,66 
Guid77 
?77 #
menuItemModifierGroupId77 %
=77& '
null77( ,
,77, -
Guid88 
?88 
modifierGroupId88 
=88 
null88  $
,88$ %
bool99  
shouldPrintToKitchen99 !
=99" #
true99$ (
,99( )
string:: 
?:: 
sectionName:: 
=:: 
null:: "
,::" #
string;; 
?;; 
multiplierName;; 
=;;  
null;;! %
,;;% &
bool== 
isSectionWisePrice== 
===  !
false==" '
,==' (
Guid>> 
?>> %
parentOrderLineModifierId>> '
=>>( )
null>>* .
,>>. /
PriceStrategy?? 
??? 
priceStrategy?? $
=??% &
null??' +
)??+ ,
{@@ 
ifAA 

(AA 
	itemCountAA 
<=AA 
$numAA 
)AA 
{BB 	
throwCC 
newCC 

ExceptionsCC  
.CC  !*
BusinessRuleViolationExceptionCC! ?
(CC? @
$strCC@ g
)CCg h
;CCh i
}DD 	
ifFF 

(FF 
	unitPriceFF 
<FF 
MoneyFF 
.FF 
ZeroFF "
(FF" #
)FF# $
)FF$ %
{GG 	
throwHH 
newHH 

ExceptionsHH  
.HH  !*
BusinessRuleViolationExceptionHH! ?
(HH? @
$strHH@ `
)HH` a
;HHa b
}II 	
ifKK 

(KK 
!KK 

modifierIdKK 
.KK 
HasValueKK  
&&KK! #
modifierTypeKK$ 0
!=KK1 3
ModifierTypeKK4 @
.KK@ A
InfoOnlyKKA I
)KKI J
{LL 	
}PP 	
varRR 
modifierRR 
=RR 
newRR 
OrderLineModifierRR ,
{SS 	
IdTT 
=TT 
GuidTT 
.TT 
NewGuidTT 
(TT 
)TT 
,TT  
OrderLineIdUU 
=UU 
orderLineIdUU %
,UU% &

ModifierIdVV 
=VV 

modifierIdVV #
,VV# $
ModifierGroupIdWW 
=WW 
modifierGroupIdWW -
,WW- .#
MenuItemModifierGroupIdXX #
=XX$ %#
menuItemModifierGroupIdXX& =
,XX= >
NameYY 
=YY 
nameYY 
,YY 
ModifierTypeZZ 
=ZZ 
modifierTypeZZ '
,ZZ' (
	ItemCount[[ 
=[[ 
	itemCount[[ !
,[[! "
	UnitPrice\\ 
=\\ 
	unitPrice\\ !
,\\! "
	BasePrice]] 
=]] 
	basePrice]] !
,]]! "
PortionValue^^ 
=^^ 
portionValue^^ '
,^^' (
TaxRate__ 
=__ 
taxRate__ 
,__  
ShouldPrintToKitchen``  
=``! " 
shouldPrintToKitchen``# 7
,``7 8
SectionNameaa 
=aa 
sectionNameaa %
,aa% &
MultiplierNamebb 
=bb 
multiplierNamebb +
,bb+ ,
IsSectionWisePricecc 
=cc  
isSectionWisePricecc! 3
,cc3 4%
ParentOrderLineModifierIddd %
=dd& '%
parentOrderLineModifierIddd( A
,ddA B
PriceStrategyee 
=ee 
priceStrategyee )
,ee) *
	CreatedAtff 
=ff 
DateTimeff  
.ff  !
UtcNowff! '
}gg 	
;gg	 

modifierii 
.ii 
CalculateTotalsii  
(ii  !
)ii! "
;ii" #
returnjj 
modifierjj 
;jj 
}kk 
publicpp 

staticpp 
OrderLineModifierpp #&
CreatePizzaSectionModifierpp$ >
(pp> ?
Guidqq 
orderLineIdqq 
,qq 
Guidrr 

modifierIdrr 
,rr 
stringss 
namess 
,ss 
ModifierTypett 
modifierTypett !
,tt! "
intuu 
	itemCountuu 
,uu 
Moneyvv 
	unitPricevv 
,vv 
Moneyww 
	basePriceww 
,ww 
stringxx 
sectionNamexx 
,xx 
stringyy 
multiplierNameyy 
,yy 
decimalzz 
taxRatezz 
=zz 
$numzz 
)zz 
{{{ 
return|| 
Create|| 
(|| 
orderLineId}} 
,}} 

modifierId~~ 
,~~ 
name 
, 
modifierType
ÄÄ 
,
ÄÄ 
	itemCount
ÅÅ 
,
ÅÅ 
	unitPrice
ÇÇ 
,
ÇÇ 
	basePrice
ÉÉ 
,
ÉÉ 
$num
ÑÑ 
,
ÑÑ 
taxRate
ÖÖ 
,
ÖÖ 
null
ÜÜ 
,
ÜÜ "
shouldPrintToKitchen
áá  
:
áá  !
true
áá" &
,
áá& '
sectionName
àà 
:
àà 
sectionName
àà $
,
àà$ %
multiplierName
ââ 
:
ââ 
multiplierName
ââ *
,
ââ* + 
isSectionWisePrice
ää 
:
ää 
true
ää  $
)
ää$ %
;
ää% &
}
ãã 
private
çç 
void
çç 
CalculateTotals
çç  
(
çç  !
)
çç! "
{
éé 
SubtotalAmount
èè 
=
èè 
	UnitPrice
èè "
*
èè# $
	ItemCount
èè% .
;
èè. /
	TaxAmount
êê 
=
êê 
SubtotalAmount
êê "
*
êê# $
TaxRate
êê% ,
;
êê, -
TotalAmount
ëë 
=
ëë 
SubtotalAmount
ëë $
+
ëë% &
	TaxAmount
ëë' 0
;
ëë0 1
}
íí 
public
óó 

void
óó 
UpdateUnitPrice
óó 
(
óó  
Money
óó  %
newUnitPrice
óó& 2
)
óó2 3
{
òò 
if
ôô 

(
ôô 
newUnitPrice
ôô 
<
ôô 
Money
ôô  
.
ôô  !
Zero
ôô! %
(
ôô% &
)
ôô& '
)
ôô' (
throw
öö 
new
öö 

Exceptions
öö !
.
öö! ",
BusinessRuleViolationException
öö" @
(
öö@ A
$str
ööA a
)
ööa b
;
ööb c
	UnitPrice
úú 
=
úú 
newUnitPrice
úú  
;
úú  !
CalculateTotals
ùù 
(
ùù 
)
ùù 
;
ùù 
}
ûû 
public
££ 

void
££ "
MarkPrintedToKitchen
££ $
(
££$ %
)
££% &
{
§§ 
if
•• 

(
•• 
!
•• "
ShouldPrintToKitchen
•• !
)
••! "
{
¶¶ 	
throw
ßß 
new
ßß 

Exceptions
ßß  
.
ßß  !,
BusinessRuleViolationException
ßß! ?
(
ßß? @
$str
ßß@ q
)
ßßq r
;
ßßr s
}
®® 	
PrintedToKitchen
™™ 
=
™™ 
true
™™ 
;
™™  
}
´´ 
public
∞∞ 

static
∞∞ 
OrderLineModifier
∞∞ #
CreateInstruction
∞∞$ 5
(
∞∞5 6
Guid
±± 
orderLineId
±± 
,
±± 
string
≤≤ 
instruction
≤≤ 
)
≤≤ 
{
≥≥ 
if
¥¥ 

(
¥¥ 
string
¥¥ 
.
¥¥  
IsNullOrWhiteSpace
¥¥ %
(
¥¥% &
instruction
¥¥& 1
)
¥¥1 2
)
¥¥2 3
{
µµ 	
throw
∂∂ 
new
∂∂ 

Exceptions
∂∂  
.
∂∂  !,
BusinessRuleViolationException
∂∂! ?
(
∂∂? @
$str
∂∂@ c
)
∂∂c d
;
∂∂d e
}
∑∑ 	
return
ππ 
Create
ππ 
(
ππ 
orderLineId
∫∫ 
:
∫∫ 
orderLineId
∫∫ $
,
∫∫$ %

modifierId
ªª 
:
ªª 
null
ªª 
,
ªª 
name
ºº 
:
ºº 
instruction
ºº 
.
ºº 
Trim
ºº "
(
ºº" #
)
ºº# $
.
ºº$ %
ToUpperInvariant
ºº% 5
(
ºº5 6
)
ºº6 7
,
ºº7 8
modifierType
ΩΩ 
:
ΩΩ 
ModifierType
ΩΩ &
.
ΩΩ& '
InfoOnly
ΩΩ' /
,
ΩΩ/ 0
	itemCount
ææ 
:
ææ 
$num
ææ 
,
ææ 
	unitPrice
øø 
:
øø 
Money
øø 
.
øø 
Zero
øø !
(
øø! "
)
øø" #
,
øø# $
	basePrice
¿¿ 
:
¿¿ 
Money
¿¿ 
.
¿¿ 
Zero
¿¿ !
(
¿¿! "
)
¿¿" #
,
¿¿# $
portionValue
¡¡ 
:
¡¡ 
$num
¡¡ 
,
¡¡ 
taxRate
¬¬ 
:
¬¬ 
$num
¬¬ 
,
¬¬ %
menuItemModifierGroupId
√√ #
:
√√# $
null
√√% )
,
√√) *
modifierGroupId
ƒƒ 
:
ƒƒ 
null
ƒƒ !
,
ƒƒ! ""
shouldPrintToKitchen
≈≈  
:
≈≈  !
true
≈≈" &
,
≈≈& '
sectionName
∆∆ 
:
∆∆ 
null
∆∆ 
,
∆∆ 
multiplierName
«« 
:
«« 
null
««  
,
««  ! 
isSectionWisePrice
»» 
:
»» 
false
»»  %
,
»»% &'
parentOrderLineModifierId
…… %
:
……% &
null
……' +
)
   	
;
  	 

}
ÀÀ 
}ÃÃ œ
sC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\OrderLineDiscount.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 
OrderLineDiscount

 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 
OrderLineId 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

Guid 

DiscountId 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

DiscountType 
Type 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

decimal 
Value 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

int 
? 
MinimumQuantity 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

Money 
Amount 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

bool 
	AutoApply 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

DateTime 
	AppliedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
private 
OrderLineDiscount 
( 
) 
{ 
Amount 
= 
Money 
. 
Zero 
( 
) 
; 
} 
public 

static 
OrderLineDiscount #
Create$ *
(* +
Guid 
orderLineId 
, 
Guid 

discountId 
, 
string 
name 
, 
DiscountType   
type   
,   
decimal!! 
value!! 
,!! 
Money"" 
amount"" 
,"" 
int## 
?## 
minimumQuantity## 
=## 
null## #
,### $
bool$$ 
	autoApply$$ 
=$$ 
false$$ 
)$$ 
{%% 
return&& 
new&& 
OrderLineDiscount&& $
{'' 	
Id(( 
=(( 
Guid(( 
.(( 
NewGuid(( 
((( 
)(( 
,((  
OrderLineId)) 
=)) 
orderLineId)) %
,))% &

DiscountId** 
=** 

discountId** #
,**# $
Name++ 
=++ 
name++ 
,++ 
Type,, 
=,, 
type,, 
,,, 
Value-- 
=-- 
value-- 
,-- 
Amount.. 
=.. 
amount.. 
,.. 
MinimumQuantity// 
=// 
minimumQuantity// -
,//- .
	AutoApply00 
=00 
	autoApply00 !
,00! "
	AppliedAt11 
=11 
DateTime11  
.11  !
UtcNow11! '
}22 	
;22	 

}33 
}44 ·Ë
kC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\OrderLine.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
	OrderLine 
{ 
private 
readonly 
List 
< 
OrderLineModifier +
>+ ,

_modifiers- 7
=8 9
new: =
(= >
)> ?
;? @
private 
readonly 
List 
< 
OrderLineDiscount +
>+ ,

_discounts- 7
=8 9
new: =
(= >
)> ?
;? @
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 
TicketId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

Guid 

MenuItemId 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

string 
MenuItemName 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
=5 6
string7 =
.= >
Empty> C
;C D
public 

string 
? 
CategoryName 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

string 
? 
	GroupName 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

decimal 
Quantity 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

int 
	ItemCount 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

string 
? 
ItemUnitName 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

bool 
IsFractionalUnit  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
public 

Money 
	UnitPrice 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public   

Money   
SubtotalAmount   
{    !
get  " %
;  % &
private  ' .
set  / 2
;  2 3
}  4 5
public!! 

Money!! *
SubtotalAmountWithoutModifiers!! /
{!!0 1
get!!2 5
;!!5 6
private!!7 >
set!!? B
;!!B C
}!!D E
public"" 

Money"" 
DiscountAmount"" 
{""  !
get""" %
;""% &
private""' .
set""/ 2
;""2 3
}""4 5
public## 

decimal## 
TaxRate## 
{## 
get##  
;##  !
private##" )
set##* -
;##- .
}##/ 0
public$$ 

Money$$ 
	TaxAmount$$ 
{$$ 
get$$  
;$$  !
private$$" )
set$$* -
;$$- .
}$$/ 0
public%% 

Money%% %
TaxAmountWithoutModifiers%% *
{%%+ ,
get%%- 0
;%%0 1
private%%2 9
set%%: =
;%%= >
}%%? @
public&& 

Money&& 
TotalAmount&& 
{&& 
get&& "
;&&" #
private&&$ +
set&&, /
;&&/ 0
}&&1 2
public'' 

Money'' '
TotalAmountWithoutModifiers'' ,
{''- .
get''/ 2
;''2 3
private''4 ;
set''< ?
;''? @
}''A B
public** 

bool** 

IsBeverage** 
{** 
get**  
;**  !
private**" )
set*** -
;**- .
}**/ 0
public++ 

bool++  
ShouldPrintToKitchen++ $
{++% &
get++' *
;++* +
private++, 3
set++4 7
;++7 8
}++9 :
public,, 

bool,, 
PrintedToKitchen,,  
{,,! "
get,,# &
;,,& '
private,,( /
set,,0 3
;,,3 4
},,5 6
public// 

string// 
?// 
Instructions// 
{//  !
get//" %
;//% &
private//' .
set/// 2
;//2 3
}//4 5
public22 

int22 
?22 

SeatNumber22 
{22 
get22  
;22  !
private22" )
set22* -
;22- .
}22/ 0
public33 

bool33 
TreatAsSeat33 
{33 
get33 !
;33! "
private33# *
set33+ .
;33. /
}330 1
public77 

IReadOnlyCollection77 
<77 
OrderLineModifier77 0
>770 1
	Modifiers772 ;
=>77< >

_modifiers77? I
.77I J
Where77J O
(77O P
m77P Q
=>77R T
m77U V
.77V W
ModifierType77W c
!=77d f
Enumerations77g s
.77s t
ModifierType	77t Ä
.
77Ä Å
Extra
77Å Ü
)
77Ü á
.
77á à
ToList
77à é
(
77é è
)
77è ê
.
77ê ë

AsReadOnly
77ë õ
(
77õ ú
)
77ú ù
;
77ù û
public88 

IReadOnlyCollection88 
<88 
OrderLineModifier88 0
>880 1
AddOns882 8
=>889 ;

_modifiers88< F
.88F G
Where88G L
(88L M
m88M N
=>88O Q
m88R S
.88S T
ModifierType88T `
==88a c
Enumerations88d p
.88p q
ModifierType88q }
.88} ~
Extra	88~ É
)
88É Ñ
.
88Ñ Ö
ToList
88Ö ã
(
88ã å
)
88å ç
.
88ç é

AsReadOnly
88é ò
(
88ò ô
)
88ô ö
;
88ö õ
public:: 

IReadOnlyCollection:: 
<:: 
OrderLineDiscount:: 0
>::0 1
	Discounts::2 ;
=>::< >

_discounts::? I
.::I J

AsReadOnly::J T
(::T U
)::U V
;::V W
public;; 

OrderLineModifier;; 
?;; 
SizeModifier;; *
{;;+ ,
get;;- 0
;;;0 1
private;;2 9
set;;: =
;;;= >
};;? @
public>> 

Guid>> 
?>> 
PrinterGroupId>> 
{>>  !
get>>" %
;>>% &
private>>' .
set>>/ 2
;>>2 3
}>>4 5
public?? 

DateTime?? 
	CreatedAt?? 
{?? 
get??  #
;??# $
private??% ,
set??- 0
;??0 1
}??2 3
publicBB 

TimeSpanBB 
?BB 
DurationBB 
{BB 
getBB  #
;BB# $
privateBB% ,
setBB- 0
;BB0 1
}BB2 3
publicCC 

decimalCC 
?CC 

HourlyRateCC 
{CC  
getCC! $
;CC$ %
privateCC& -
setCC. 1
;CC1 2
}CC3 4
publicDD 

boolDD 
IsTimeChargeDD 
{DD 
getDD "
;DD" #
privateDD$ +
setDD, /
;DD/ 0
}DD1 2
privateGG 
	OrderLineGG 
(GG 
)GG 
{HH 
	UnitPriceII 
=II 
MoneyII 
.II 
ZeroII 
(II 
)II  
;II  !
SubtotalAmountJJ 
=JJ 
MoneyJJ 
.JJ 
ZeroJJ #
(JJ# $
)JJ$ %
;JJ% &*
SubtotalAmountWithoutModifiersKK &
=KK' (
MoneyKK) .
.KK. /
ZeroKK/ 3
(KK3 4
)KK4 5
;KK5 6
DiscountAmountLL 
=LL 
MoneyLL 
.LL 
ZeroLL #
(LL# $
)LL$ %
;LL% &
	TaxAmountMM 
=MM 
MoneyMM 
.MM 
ZeroMM 
(MM 
)MM  
;MM  !%
TaxAmountWithoutModifiersNN !
=NN" #
MoneyNN$ )
.NN) *
ZeroNN* .
(NN. /
)NN/ 0
;NN0 1
TotalAmountOO 
=OO 
MoneyOO 
.OO 
ZeroOO  
(OO  !
)OO! "
;OO" #'
TotalAmountWithoutModifiersPP #
=PP$ %
MoneyPP& +
.PP+ ,
ZeroPP, 0
(PP0 1
)PP1 2
;PP2 3
}QQ 
publicVV 

staticVV 
	OrderLineVV 
CreateVV "
(VV" #
GuidWW 
ticketIdWW 
,WW 
GuidXX 

menuItemIdXX 
,XX 
stringYY 
menuItemNameYY 
,YY 
decimalZZ 
quantityZZ 
,ZZ 
Money[[ 
	unitPrice[[ 
,[[ 
decimal\\ 
taxRate\\ 
=\\ 
$num\\ 
,\\ 
string]] 
?]] 
categoryName]] 
=]] 
null]] #
,]]# $
string^^ 
?^^ 
	groupName^^ 
=^^ 
null^^  
)^^  !
{__ 
if`` 

(`` 
quantity`` 
<=`` 
$num`` 
)`` 
{aa 	
throwbb 
newbb *
BusinessRuleViolationExceptionbb 4
(bb4 5
$strbb5 Z
)bbZ [
;bb[ \
}cc 	
ifee 

(ee 
	unitPriceee 
<ee 
Moneyee 
.ee 
Zeroee "
(ee" #
)ee# $
)ee$ %
{ff 	
throwgg 
newgg *
BusinessRuleViolationExceptiongg 4
(gg4 5
$strgg5 U
)ggU V
;ggV W
}hh 	
varjj 
	orderLinejj 
=jj 
newjj 
	OrderLinejj %
{kk 	
Idll 
=ll 
Guidll 
.ll 
NewGuidll 
(ll 
)ll 
,ll  
TicketIdmm 
=mm 
ticketIdmm 
,mm  

MenuItemIdnn 
=nn 

menuItemIdnn #
,nn# $
MenuItemNameoo 
=oo 
menuItemNameoo '
,oo' (
CategoryNamepp 
=pp 
categoryNamepp '
,pp' (
	GroupNameqq 
=qq 
	groupNameqq !
,qq! "
Quantityrr 
=rr 
quantityrr 
,rr  
	ItemCountss 
=ss 
(ss 
intss 
)ss 
Mathss !
.ss! "
Ceilingss" )
(ss) *
quantityss* 2
)ss2 3
,ss3 4
	UnitPricett 
=tt 
	unitPricett !
,tt! "
TaxRateuu 
=uu 
taxRateuu 
,uu 
	CreatedAtvv 
=vv 
DateTimevv  
.vv  !
UtcNowvv! '
}ww 	
;ww	 

	orderLineyy 
.yy 
CalculatePriceyy  
(yy  !
)yy! "
;yy" #
returnzz 
	orderLinezz 
;zz 
}{{ 
public
ÄÄ 

static
ÄÄ 
	OrderLine
ÄÄ 
CreateTimeCharge
ÄÄ ,
(
ÄÄ, -
Guid
ÅÅ 
ticketId
ÅÅ 
,
ÅÅ 
TimeSpan
ÇÇ 
duration
ÇÇ 
,
ÇÇ 
decimal
ÉÉ 

hourlyRate
ÉÉ 
,
ÉÉ 
Money
ÑÑ 
totalCharge
ÑÑ 
)
ÑÑ 
{
ÖÖ 
if
ÜÜ 

(
ÜÜ 
duration
ÜÜ 
<
ÜÜ 
TimeSpan
ÜÜ 
.
ÜÜ  
Zero
ÜÜ  $
)
ÜÜ$ %
throw
áá 
new
áá ,
BusinessRuleViolationException
áá 4
(
áá4 5
$str
áá5 S
)
ááS T
;
ááT U
if
ââ 

(
ââ 

hourlyRate
ââ 
<
ââ 
$num
ââ 
)
ââ 
throw
ää 
new
ää ,
BusinessRuleViolationException
ää 4
(
ää4 5
$str
ää5 V
)
ääV W
;
ääW X
if
åå 

(
åå 
totalCharge
åå 
==
åå 
null
åå 
)
åå  
throw
çç 
new
çç #
ArgumentNullException
çç +
(
çç+ ,
nameof
çç, 2
(
çç2 3
totalCharge
çç3 >
)
çç> ?
,
çç? @
$str
ççA _
)
çç_ `
;
çç` a
if
èè 

(
èè 
totalCharge
èè 
<
èè 
Money
èè 
.
èè  
Zero
èè  $
(
èè$ %
)
èè% &
)
èè& '
throw
êê 
new
êê ,
BusinessRuleViolationException
êê 5
(
êê5 6
$str
êê6 X
)
êêX Y
;
êêY Z
var
îî 
	unitPrice
îî 
=
îî 
new
îî 
Money
îî !
(
îî! "
totalCharge
îî" -
.
îî- .
Amount
îî. 4
,
îî4 5
totalCharge
îî6 A
.
îîA B
Currency
îîB J
)
îîJ K
;
îîK L
var
ññ 
	orderLine
ññ 
=
ññ 
new
ññ 
	OrderLine
ññ %
{
óó 	
Id
òò 
=
òò 
Guid
òò 
.
òò 
NewGuid
òò 
(
òò 
)
òò 
,
òò  
TicketId
ôô 
=
ôô 
ticketId
ôô 
,
ôô  

MenuItemId
öö 
=
öö 
Guid
öö 
.
öö 
Empty
öö #
,
öö# $
MenuItemName
õõ 
=
õõ 
$str
õõ .
,
õõ. /
CategoryName
úú 
=
úú 
$str
úú )
,
úú) *
	GroupName
ùù 
=
ùù 
$str
ùù  
,
ùù  !
Quantity
ûû 
=
ûû 
$num
ûû 
,
ûû 
	ItemCount
üü 
=
üü 
$num
üü 
,
üü 
	UnitPrice
†† 
=
†† 
	unitPrice
†† !
,
††! "
TaxRate
°° 
=
°° 
$num
°° 
,
°° 
Duration
¢¢ 
=
¢¢ 
duration
¢¢ 
,
¢¢  

HourlyRate
££ 
=
££ 

hourlyRate
££ #
,
££# $
IsTimeCharge
§§ 
=
§§ 
true
§§ 
,
§§  
	CreatedAt
•• 
=
•• 
DateTime
••  
.
••  !
UtcNow
••! '
,
••' (
Instructions
¶¶ 
=
¶¶ 
$"
¶¶ 
{
¶¶ 
duration
¶¶ &
.
¶¶& '

TotalHours
¶¶' 1
:
¶¶1 2
$str
¶¶2 4
}
¶¶4 5
$str
¶¶5 <
{
¶¶< =

hourlyRate
¶¶= G
:
¶¶G H
$str
¶¶H I
}
¶¶I J
$str
¶¶J M
"
¶¶M N
}
ßß 	
;
ßß	 

	orderLine
©© 
.
©© 
CalculatePrice
©©  
(
©©  !
)
©©! "
;
©©" #
if
¨¨ 

(
¨¨ 
	orderLine
¨¨ 
.
¨¨ 
	UnitPrice
¨¨ 
==
¨¨  "
null
¨¨# '
)
¨¨' (
throw
≠≠ 
new
≠≠ 
System
≠≠ 
.
≠≠ '
InvalidOperationException
≠≠ 6
(
≠≠6 7
$str
≠≠7 _
)
≠≠_ `
;
≠≠` a
if
ÆÆ 

(
ÆÆ 
	orderLine
ÆÆ 
.
ÆÆ 
SubtotalAmount
ÆÆ $
==
ÆÆ% '
null
ÆÆ( ,
)
ÆÆ, -
throw
ØØ 
new
ØØ 
System
ØØ 
.
ØØ '
InvalidOperationException
ØØ 6
(
ØØ6 7
$str
ØØ7 d
)
ØØd e
;
ØØe f
if
∞∞ 

(
∞∞ 
	orderLine
∞∞ 
.
∞∞ 
TotalAmount
∞∞ !
==
∞∞" $
null
∞∞% )
)
∞∞) *
throw
±± 
new
±± 
System
±± 
.
±± '
InvalidOperationException
±± 6
(
±±6 7
$str
±±7 a
)
±±a b
;
±±b c
return
≥≥ 
	orderLine
≥≥ 
;
≥≥ 
}
¥¥ 
public
ππ 

void
ππ 
CalculatePrice
ππ 
(
ππ 
)
ππ  
{
∫∫ ,
SubtotalAmountWithoutModifiers
ºº &
=
ºº' (
	UnitPrice
ºº) 2
*
ºº3 4
Quantity
ºº5 =
;
ºº= >
var
øø 
modifierTotal
øø 
=
øø 

_modifiers
øø &
.
øø& '
	Aggregate
øø' 0
(
øø0 1
Money
øø1 6
.
øø6 7
Zero
øø7 ;
(
øø; <
)
øø< =
,
øø= >
(
øø? @
sum
øø@ C
,
øøC D
m
øøE F
)
øøF G
=>
øøH J
sum
øøK N
+
øøO P
m
øøQ R
.
øøR S
TotalAmount
øøS ^
)
øø^ _
;
øø_ `
var
¬¬ 
sizeModifierTotal
¬¬ 
=
¬¬ 
SizeModifier
¬¬  ,
?
¬¬, -
.
¬¬- .
TotalAmount
¬¬. 9
??
¬¬: <
Money
¬¬= B
.
¬¬B C
Zero
¬¬C G
(
¬¬G H
)
¬¬H I
;
¬¬I J
SubtotalAmount
ƒƒ 
=
ƒƒ ,
SubtotalAmountWithoutModifiers
ƒƒ 7
+
ƒƒ8 9
modifierTotal
ƒƒ: G
+
ƒƒH I
sizeModifierTotal
ƒƒJ [
;
ƒƒ[ \'
TaxAmountWithoutModifiers
«« !
=
««" #,
SubtotalAmountWithoutModifiers
««$ B
*
««C D
TaxRate
««E L
;
««L M
var
»» 
modifierTax
»» 
=
»» 

_modifiers
»» $
.
»»$ %
	Aggregate
»»% .
(
»». /
Money
»»/ 4
.
»»4 5
Zero
»»5 9
(
»»9 :
)
»»: ;
,
»»; <
(
»»= >
sum
»»> A
,
»»A B
m
»»C D
)
»»D E
=>
»»F H
sum
»»I L
+
»»M N
m
»»O P
.
»»P Q
	TaxAmount
»»Q Z
)
»»Z [
;
»»[ \
var
…… 
sizeModifierTax
…… 
=
…… 
SizeModifier
…… *
?
……* +
.
……+ ,
	TaxAmount
……, 5
??
……6 8
Money
……9 >
.
……> ?
Zero
……? C
(
……C D
)
……D E
;
……E F
	TaxAmount
ÀÀ 
=
ÀÀ '
TaxAmountWithoutModifiers
ÀÀ -
+
ÀÀ. /
modifierTax
ÀÀ0 ;
+
ÀÀ< =
sizeModifierTax
ÀÀ> M
;
ÀÀM N
DiscountAmount
ŒŒ 
=
ŒŒ 

_discounts
ŒŒ #
.
ŒŒ# $
	Aggregate
ŒŒ$ -
(
ŒŒ- .
Money
ŒŒ. 3
.
ŒŒ3 4
Zero
ŒŒ4 8
(
ŒŒ8 9
)
ŒŒ9 :
,
ŒŒ: ;
(
ŒŒ< =
sum
ŒŒ= @
,
ŒŒ@ A
d
ŒŒB C
)
ŒŒC D
=>
ŒŒE G
sum
ŒŒH K
+
ŒŒL M
d
ŒŒN O
.
ŒŒO P
Amount
ŒŒP V
)
ŒŒV W
;
ŒŒW X
if
–– 

(
–– 
DiscountAmount
–– 
>
–– 
SubtotalAmount
–– +
)
––+ ,
{
—— 	
DiscountAmount
““ 
=
““ 
SubtotalAmount
““ +
;
““+ ,
}
”” 	)
TotalAmountWithoutModifiers
÷÷ #
=
÷÷$ %,
SubtotalAmountWithoutModifiers
÷÷& D
+
÷÷E F'
TaxAmountWithoutModifiers
÷÷G `
-
÷÷a b
DiscountAmount
÷÷c q
;
÷÷q r
TotalAmount
◊◊ 
=
◊◊ 
SubtotalAmount
◊◊ $
+
◊◊% &
	TaxAmount
◊◊' 0
-
◊◊1 2
DiscountAmount
◊◊3 A
;
◊◊A B
if
⁄⁄ 

(
⁄⁄ 
TotalAmount
⁄⁄ 
<
⁄⁄ 
Money
⁄⁄ 
.
⁄⁄  
Zero
⁄⁄  $
(
⁄⁄$ %
)
⁄⁄% &
)
⁄⁄& '
{
€€ 	
TotalAmount
‹‹ 
=
‹‹ 
Money
‹‹ 
.
‹‹  
Zero
‹‹  $
(
‹‹$ %
)
‹‹% &
;
‹‹& '
}
›› 	
}
ﬁﬁ 
public
„„ 

void
„„ 
UpdateQuantity
„„ 
(
„„ 
decimal
„„ &
quantity
„„' /
)
„„/ 0
{
‰‰ 
if
ÂÂ 

(
ÂÂ 
quantity
ÂÂ 
<=
ÂÂ 
$num
ÂÂ 
)
ÂÂ 
{
ÊÊ 	
throw
ÁÁ 
new
ÁÁ ,
BusinessRuleViolationException
ÁÁ 4
(
ÁÁ4 5
$str
ÁÁ5 Z
)
ÁÁZ [
;
ÁÁ[ \
}
ËË 	
Quantity
ÍÍ 
=
ÍÍ 
quantity
ÍÍ 
;
ÍÍ 
	ItemCount
ÎÎ 
=
ÎÎ 
(
ÎÎ 
int
ÎÎ 
)
ÎÎ 
Math
ÎÎ 
.
ÎÎ 
Ceiling
ÎÎ %
(
ÎÎ% &
quantity
ÎÎ& .
)
ÎÎ. /
;
ÎÎ/ 0
CalculatePrice
ÏÏ 
(
ÏÏ 
)
ÏÏ 
;
ÏÏ 
}
ÌÌ 
public
ÚÚ 

bool
ÚÚ 
CanMerge
ÚÚ 
(
ÚÚ 
	OrderLine
ÚÚ "
other
ÚÚ# (
)
ÚÚ( )
{
ÛÛ 
if
ÙÙ 

(
ÙÙ 
other
ÙÙ 
==
ÙÙ 
null
ÙÙ 
)
ÙÙ 
return
ÙÙ !
false
ÙÙ" '
;
ÙÙ' (
if
ıı 

(
ıı 

MenuItemId
ıı 
!=
ıı 
other
ıı 
.
ıı  

MenuItemId
ıı  *
)
ıı* +
return
ıı, 2
false
ıı3 8
;
ıı8 9
if
ˆˆ 

(
ˆˆ 
	UnitPrice
ˆˆ 
!=
ˆˆ 
other
ˆˆ 
.
ˆˆ 
	UnitPrice
ˆˆ (
)
ˆˆ( )
return
ˆˆ* 0
false
ˆˆ1 6
;
ˆˆ6 7
if
˜˜ 

(
˜˜ 
TaxRate
˜˜ 
!=
˜˜ 
other
˜˜ 
.
˜˜ 
TaxRate
˜˜ $
)
˜˜$ %
return
˜˜& ,
false
˜˜- 2
;
˜˜2 3
if
¯¯ 

(
¯¯ 

_modifiers
¯¯ 
.
¯¯ 
Count
¯¯ 
!=
¯¯ 
other
¯¯  %
.
¯¯% &

_modifiers
¯¯& 0
.
¯¯0 1
Count
¯¯1 6
)
¯¯6 7
return
¯¯8 >
false
¯¯? D
;
¯¯D E
return
ˇˇ 
true
ˇˇ 
;
ˇˇ 
}
ÄÄ 
public
ÖÖ 

void
ÖÖ 
Merge
ÖÖ 
(
ÖÖ 
	OrderLine
ÖÖ 
other
ÖÖ  %
)
ÖÖ% &
{
ÜÜ 
if
áá 

(
áá 
!
áá 
CanMerge
áá 
(
áá 
other
áá 
)
áá 
)
áá 
{
àà 	
throw
ââ 
new
ââ ,
BusinessRuleViolationException
ââ 4
(
ââ4 5
$str
ââ5 h
)
ââh i
;
ââi j
}
ää 	
Quantity
åå 
+=
åå 
other
åå 
.
åå 
Quantity
åå "
;
åå" #
	ItemCount
çç 
+=
çç 
other
çç 
.
çç 
	ItemCount
çç $
;
çç$ %
CalculatePrice
éé 
(
éé 
)
éé 
;
éé 
}
èè 
public
îî 

void
îî 
AddModifier
îî 
(
îî 
OrderLineModifier
îî -
modifier
îî. 6
)
îî6 7
{
ïï 
if
ññ 

(
ññ 
modifier
ññ 
==
ññ 
null
ññ 
)
ññ 
throw
ññ #
new
ññ$ '#
ArgumentNullException
ññ( =
(
ññ= >
nameof
ññ> D
(
ññD E
modifier
ññE M
)
ññM N
)
ññN O
;
ññO P

_modifiers
òò 
.
òò 
Add
òò 
(
òò 
modifier
òò 
)
òò  
;
òò  !
CalculatePrice
ôô 
(
ôô 
)
ôô 
;
ôô 
}
öö 
public
üü 

void
üü 
RemoveModifier
üü 
(
üü 
OrderLineModifier
üü 0
modifier
üü1 9
)
üü9 :
{
†† 
if
°°	 
(
°° 
modifier
°° 
==
°° 
null
°° 
)
°° 
throw
°° $
new
°°% (#
ArgumentNullException
°°) >
(
°°> ?
nameof
°°? E
(
°°E F
modifier
°°F N
)
°°N O
)
°°O P
;
°°P Q

_modifiers
££ 
.
££ 
Remove
££ 
(
££ 
modifier
££ "
)
££" #
;
££# $
CalculatePrice
§§ 
(
§§ 
)
§§ 
;
§§ 
}
•• 
public
™™ 

void
™™ 
ApplyDiscount
™™ 
(
™™ 
OrderLineDiscount
™™ /
discount
™™0 8
)
™™8 9
{
´´ 
if
¨¨ 

(
¨¨ 
discount
¨¨ 
==
¨¨ 
null
¨¨ 
)
¨¨ 
throw
¨¨ #
new
¨¨$ '#
ArgumentNullException
¨¨( =
(
¨¨= >
nameof
¨¨> D
(
¨¨D E
discount
¨¨E M
)
¨¨M N
)
¨¨N O
;
¨¨O P
if
ÆÆ 

(
ÆÆ 
discount
ÆÆ 
.
ÆÆ 
OrderLineId
ÆÆ  
!=
ÆÆ! #
Id
ÆÆ$ &
)
ÆÆ& '
throw
ØØ 
new
ØØ ,
BusinessRuleViolationException
ØØ 4
(
ØØ4 5
$str
ØØ5 c
)
ØØc d
;
ØØd e

_discounts
±± 
.
±± 
Add
±± 
(
±± 
discount
±± 
)
±±  
;
±±  !
CalculatePrice
≤≤ 
(
≤≤ 
)
≤≤ 
;
≤≤ 
}
≥≥ 
public
∏∏ 

void
∏∏ "
MarkPrintedToKitchen
∏∏ $
(
∏∏$ %
)
∏∏% &
{
ππ 
if
∫∫ 

(
∫∫ 
!
∫∫ "
ShouldPrintToKitchen
∫∫ !
)
∫∫! "
{
ªª 	
throw
ºº 
new
ºº ,
BusinessRuleViolationException
ºº 4
(
ºº4 5
$str
ºº5 h
)
ººh i
;
ººi j
}
ΩΩ 	
PrintedToKitchen
øø 
=
øø 
true
øø 
;
øø  
foreach
¬¬ 
(
¬¬ 
var
¬¬ 
modifier
¬¬ 
in
¬¬  

_modifiers
¬¬! +
.
¬¬+ ,
Where
¬¬, 1
(
¬¬1 2
m
¬¬2 3
=>
¬¬4 6
m
¬¬7 8
.
¬¬8 9"
ShouldPrintToKitchen
¬¬9 M
)
¬¬M N
)
¬¬N O
{
√√ 	
modifier
ƒƒ 
.
ƒƒ "
MarkPrintedToKitchen
ƒƒ )
(
ƒƒ) *
)
ƒƒ* +
;
ƒƒ+ ,
}
≈≈ 	
}
∆∆ 
public
ÀÀ 

void
ÀÀ 
SetPrinterGroup
ÀÀ 
(
ÀÀ  
Guid
ÀÀ  $
?
ÀÀ$ %
printerGroupId
ÀÀ& 4
)
ÀÀ4 5
{
ÃÃ 
PrinterGroupId
ÕÕ 
=
ÕÕ 
printerGroupId
ÕÕ '
;
ÕÕ' ("
ShouldPrintToKitchen
ŒŒ 
=
ŒŒ 
printerGroupId
ŒŒ -
.
ŒŒ- .
HasValue
ŒŒ. 6
;
ŒŒ6 7
}
œœ 
public
‘‘ 

void
‘‘ 
SetInstructions
‘‘ 
(
‘‘  
string
‘‘  &
?
‘‘& '
instructions
‘‘( 4
)
‘‘4 5
{
’’ 
Instructions
÷÷ 
=
÷÷ 
instructions
÷÷ #
;
÷÷# $
}
◊◊ 
public
‹‹ 

void
‹‹ 
UpdateModifiers
‹‹ 
(
‹‹  
IEnumerable
‹‹  +
<
‹‹+ ,
OrderLineModifier
‹‹, =
>
‹‹= >
newModifiers
‹‹? K
)
‹‹K L
{
›› 

_modifiers
ﬁﬁ 
.
ﬁﬁ 
Clear
ﬁﬁ 
(
ﬁﬁ 
)
ﬁﬁ 
;
ﬁﬁ 
foreach
‡‡ 
(
‡‡ 
var
‡‡ 
modifier
‡‡ 
in
‡‡  
newModifiers
‡‡! -
)
‡‡- .
{
·· 	
AddModifier
‚‚ 
(
‚‚ 
modifier
‚‚  
)
‚‚  !
;
‚‚! "
}
„„ 	
CalculatePrice
ÂÂ 
(
ÂÂ 
)
ÂÂ 
;
ÂÂ 
}
ÊÊ 
public
ÍÍ 

void
ÍÍ 
SetSeatNumber
ÍÍ 
(
ÍÍ 
int
ÍÍ !
?
ÍÍ! "

seatNumber
ÍÍ# -
)
ÍÍ- .
{
ÎÎ 
if
ÏÏ 

(
ÏÏ 

seatNumber
ÏÏ 
.
ÏÏ 
HasValue
ÏÏ 
&&
ÏÏ  "

seatNumber
ÏÏ# -
.
ÏÏ- .
Value
ÏÏ. 3
<
ÏÏ4 5
$num
ÏÏ6 7
)
ÏÏ7 8
{
ÌÌ 	
throw
ÓÓ 
new
ÓÓ ,
BusinessRuleViolationException
ÓÓ 5
(
ÓÓ5 6
$str
ÓÓ6 W
)
ÓÓW X
;
ÓÓX Y
}
ÔÔ 	

SeatNumber
 
=
 

seatNumber
 
;
  
}
ÒÒ 
}ÚÚ ¿k
oC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\ModifierGroup.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
ModifierGroup		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
null/ 3
!3 4
;4 5
public 

string 
? 
Description 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

bool 

IsRequired 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

int 
MinSelections 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

int 
MaxSelections 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

bool #
AllowMultipleSelections '
{( )
get* -
;- .
private/ 6
set7 :
;: ;
}< =
public 

int 
DisplayOrder 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

int 
Version 
{ 
get 
; 
private %
set& )
;) *
}+ ,
public 

int 
FreeModifiers 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

decimal 
ExtraModifierPrice %
{& '
get( +
;+ ,
private- 4
set5 8
;8 9
}: ;
private 
readonly 
List 
< 
MenuModifier &
>& '

_modifiers( 2
=3 4
new5 8
(8 9
)9 :
;: ;
public 

IReadOnlyCollection 
< 
MenuModifier +
>+ ,
	Modifiers- 6
=>7 9

_modifiers: D
.D E

AsReadOnlyE O
(O P
)P Q
;Q R
private 
ModifierGroup 
( 
) 
{   
Name!! 
=!! 
string!! 
.!! 
Empty!! 
;!! 
}"" 
public'' 

static'' 
ModifierGroup'' 
Create''  &
(''& '
string(( 
name(( 
,(( 
bool)) 

isRequired)) 
=)) 
false)) 
,))  
int** 
minSelections** 
=** 
$num** 
,** 
int++ 
maxSelections++ 
=++ 
$num++ 
,++ 
bool,, #
allowMultipleSelections,, $
=,,% &
false,,' ,
,,,, -
string-- 
?-- 
description-- 
=-- 
null-- "
,--" #
int.. 
displayOrder.. 
=.. 
$num.. 
,.. 
bool// 
isActive// 
=// 
true// 
,// 
int00 
freeModifiers00 
=00 
$num00 
,00 
decimal11 
extraModifierPrice11 "
=11# $
$num11% *
)11* +
{22 
if33 

(33 
string33 
.33 
IsNullOrWhiteSpace33 %
(33% &
name33& *
)33* +
)33+ ,
{44 	
throw55 
new55 

Exceptions55  
.55  !*
BusinessRuleViolationException55! ?
(55? @
$str55@ f
)55f g
;55g h
}66 	
if88 

(88 
minSelections88 
<88 
$num88 
)88 
{99 	
throw:: 
new:: 

Exceptions::  
.::  !*
BusinessRuleViolationException::! ?
(::? @
$str::@ h
)::h i
;::i j
};; 	
if== 

(== 
maxSelections== 
<== 
minSelections== )
)==) *
{>> 	
throw?? 
new?? 

Exceptions??  
.??  !*
BusinessRuleViolationException??! ?
(??? @
$str??@ |
)??| }
;??} ~
}@@ 	
ifBB 

(BB 
maxSelectionsBB 
>BB 
$numBB 
&&BB  
!BB! "#
allowMultipleSelectionsBB" 9
)BB9 :
{CC 	
throwDD 
newDD 

ExceptionsDD  
.DD  !*
BusinessRuleViolationExceptionDD! ?
(DD? @
$str	DD@ å
)
DDå ç
;
DDç é
}EE 	
ifGG 

(GG 
freeModifiersGG 
<GG 
$numGG 
)GG 
{HH 	
throwII 
newII 

ExceptionsII  
.II  !*
BusinessRuleViolationExceptionII! ?
(II? @
$strII@ j
)IIj k
;IIk l
}JJ 	
ifLL 

(LL 
extraModifierPriceLL 
<LL  
$numLL! "
)LL" #
{MM 	
throwNN 
newNN 

ExceptionsNN  
.NN  !*
BusinessRuleViolationExceptionNN! ?
(NN? @
$strNN@ j
)NNj k
;NNk l
}OO 	
returnQQ 
newQQ 
ModifierGroupQQ  
{RR 	
IdSS 
=SS 
GuidSS 
.SS 
NewGuidSS 
(SS 
)SS 
,SS  
NameTT 
=TT 
nameTT 
,TT 
DescriptionUU 
=UU 
descriptionUU %
,UU% &

IsRequiredVV 
=VV 

isRequiredVV #
,VV# $
MinSelectionsWW 
=WW 
minSelectionsWW )
,WW) *
MaxSelectionsXX 
=XX 
maxSelectionsXX )
,XX) *#
AllowMultipleSelectionsYY #
=YY$ %#
allowMultipleSelectionsYY& =
,YY= >
DisplayOrderZZ 
=ZZ 
displayOrderZZ '
,ZZ' (
IsActive[[ 
=[[ 
isActive[[ 
,[[  
FreeModifiers\\ 
=\\ 
freeModifiers\\ )
,\\) *
ExtraModifierPrice]] 
=]]  
extraModifierPrice]]! 3
,]]3 4
Version^^ 
=^^ 
$num^^ 
}__ 	
;__	 

}`` 
publicee 

voidee 

UpdateNameee 
(ee 
stringee !
nameee" &
)ee& '
{ff 
ifgg 

(gg 
stringgg 
.gg 
IsNullOrWhiteSpacegg %
(gg% &
namegg& *
)gg* +
)gg+ ,
{hh 	
throwii 
newii 

Exceptionsii  
.ii  !*
BusinessRuleViolationExceptionii! ?
(ii? @
$strii@ f
)iif g
;iig h
}jj 	
Namell 
=ll 
namell 
;ll 
}mm 
publicrr 

voidrr 
UpdateDescriptionrr !
(rr! "
stringrr" (
?rr( )
descriptionrr* 5
)rr5 6
{ss 
Descriptiontt 
=tt 
descriptiontt !
;tt! "
}uu 
publiczz 

voidzz 
SetIsRequiredzz 
(zz 
boolzz "

isRequiredzz# -
)zz- .
{{{ 

IsRequired|| 
=|| 

isRequired|| 
;||  
}}} 
public
ÇÇ 

void
ÇÇ (
UpdateSelectionConstraints
ÇÇ *
(
ÇÇ* +
int
ÇÇ+ .
minSelections
ÇÇ/ <
,
ÇÇ< =
int
ÇÇ> A
maxSelections
ÇÇB O
,
ÇÇO P
bool
ÇÇQ U%
allowMultipleSelections
ÇÇV m
)
ÇÇm n
{
ÉÉ 
if
ÑÑ 

(
ÑÑ 
minSelections
ÑÑ 
<
ÑÑ 
$num
ÑÑ 
)
ÑÑ 
{
ÖÖ 	
throw
ÜÜ 
new
ÜÜ 

Exceptions
ÜÜ  
.
ÜÜ  !,
BusinessRuleViolationException
ÜÜ! ?
(
ÜÜ? @
$str
ÜÜ@ h
)
ÜÜh i
;
ÜÜi j
}
áá 	
if
ââ 

(
ââ 
maxSelections
ââ 
<
ââ 
minSelections
ââ )
)
ââ) *
{
ää 	
throw
ãã 
new
ãã 

Exceptions
ãã  
.
ãã  !,
BusinessRuleViolationException
ãã! ?
(
ãã? @
$str
ãã@ |
)
ãã| }
;
ãã} ~
}
åå 	
if
éé 

(
éé 
maxSelections
éé 
>
éé 
$num
éé 
&&
éé  
!
éé! "%
allowMultipleSelections
éé" 9
)
éé9 :
{
èè 	
throw
êê 
new
êê 

Exceptions
êê  
.
êê  !,
BusinessRuleViolationException
êê! ?
(
êê? @
$strêê@ å
)êêå ç
;êêç é
}
ëë 	
MinSelections
ìì 
=
ìì 
minSelections
ìì %
;
ìì% &
MaxSelections
îî 
=
îî 
maxSelections
îî %
;
îî% &%
AllowMultipleSelections
ïï 
=
ïï  !%
allowMultipleSelections
ïï" 9
;
ïï9 :
}
ññ 
public
õõ 

void
õõ  
UpdateDisplayOrder
õõ "
(
õõ" #
int
õõ# &
displayOrder
õõ' 3
)
õõ3 4
{
úú 
DisplayOrder
ùù 
=
ùù 
displayOrder
ùù #
;
ùù# $
}
ûû 
public
££ 

void
££ 
Activate
££ 
(
££ 
)
££ 
{
§§ 
IsActive
•• 
=
•• 
true
•• 
;
•• 
}
¶¶ 
public
´´ 

void
´´ 

Deactivate
´´ 
(
´´ 
)
´´ 
{
¨¨ 
IsActive
≠≠ 
=
≠≠ 
false
≠≠ 
;
≠≠ 
}
ÆÆ 
public
≥≥ 

void
≥≥ 
UpdatePricingTier
≥≥ !
(
≥≥! "
int
≥≥" %
freeModifiers
≥≥& 3
,
≥≥3 4
decimal
≥≥5 < 
extraModifierPrice
≥≥= O
)
≥≥O P
{
¥¥ 
if
µµ 

(
µµ 
freeModifiers
µµ 
<
µµ 
$num
µµ 
)
µµ 
{
∂∂ 	
throw
∑∑ 
new
∑∑ 

Exceptions
∑∑  
.
∑∑  !,
BusinessRuleViolationException
∑∑! ?
(
∑∑? @
$str
∑∑@ j
)
∑∑j k
;
∑∑k l
}
∏∏ 	
if
∫∫ 

(
∫∫  
extraModifierPrice
∫∫ 
<
∫∫  
$num
∫∫! "
)
∫∫" #
{
ªª 	
throw
ºº 
new
ºº 

Exceptions
ºº  
.
ºº  !,
BusinessRuleViolationException
ºº! ?
(
ºº? @
$str
ºº@ j
)
ººj k
;
ººk l
}
ΩΩ 	
FreeModifiers
øø 
=
øø 
freeModifiers
øø %
;
øø% & 
ExtraModifierPrice
¿¿ 
=
¿¿  
extraModifierPrice
¿¿ /
;
¿¿/ 0
}
¡¡ 
public
»» 

decimal
»» #
CalculateModifierCost
»» (
(
»»( )
int
»») ,
selectedCount
»»- :
)
»»: ;
{
…… 
if
   

(
   
selectedCount
   
<=
   
FreeModifiers
   *
)
  * +
{
ÀÀ 	
return
ÃÃ 
$num
ÃÃ 
;
ÃÃ 
}
ÕÕ 	
var
œœ 
chargeableCount
œœ 
=
œœ 
selectedCount
œœ +
-
œœ, -
FreeModifiers
œœ. ;
;
œœ; <
return
–– 
chargeableCount
–– 
*
––   
ExtraModifierPrice
––! 3
;
––3 4
}
—— 
public
÷÷ 

bool
÷÷ #
IsValidSelectionCount
÷÷ %
(
÷÷% &
int
÷÷& )
selectionCount
÷÷* 8
)
÷÷8 9
{
◊◊ 
if
ÿÿ 

(
ÿÿ 

IsRequired
ÿÿ 
&&
ÿÿ 
selectionCount
ÿÿ (
<
ÿÿ) *
MinSelections
ÿÿ+ 8
)
ÿÿ8 9
{
ŸŸ 	
return
⁄⁄ 
false
⁄⁄ 
;
⁄⁄ 
}
€€ 	
if
›› 

(
›› 
selectionCount
›› 
<
›› 
MinSelections
›› *
||
››+ -
selectionCount
››. <
>
››= >
MaxSelections
››? L
)
››L M
{
ﬁﬁ 	
return
ﬂﬂ 
false
ﬂﬂ 
;
ﬂﬂ 
}
‡‡ 	
return
‚‚ 
true
‚‚ 
;
‚‚ 
}
„„ 
}‰‰ Ê4
~C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\MerchantGatewayConfiguration.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 (
MerchantGatewayConfiguration

 )
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 

TerminalId 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

string 
ProviderName 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
=5 6
null7 ;
!; <
;< =
public 

string 

MerchantId 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
=3 4
null5 9
!9 :
;: ;
public 

string 
EncryptedApiKey !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
=8 9
null: >
!> ?
;? @
public 

string 

GatewayUrl 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
=3 4
null5 9
!9 :
;: ;
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

string 
CardTypesAccepted #
{$ %
get& )
;) *
private+ 2
set3 6
;6 7
}8 9
=: ;
$str< O
;O P
public 

decimal 
SignatureThreshold %
{& '
get( +
;+ ,
private- 4
set5 8
;8 9
}: ;
=< =
$num> D
;D E
public 

bool 
AllowTipAdjustment "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
=9 :
true; ?
;? @
public 

bool 
IsExternalTerminal "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
=9 :
false; @
;@ A
public 

bool 
AllowManualEntry  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
=7 8
true9 =
;= >
public 

bool 
EnablePreAuth 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
false6 ;
;; <
public 

int 
Version 
{ 
get 
; 
private %
set& )
;) *
}+ ,
private (
MerchantGatewayConfiguration (
(( )
)) *
{+ ,
}- .
public!! 

static!! (
MerchantGatewayConfiguration!! .
Create!!/ 5
(!!5 6
Guid"" 

terminalId"" 
,"" 
string## 
providerName## 
,## 
string$$ 

merchantId$$ 
,$$ 
string%% 
encryptedApiKey%% 
,%% 
string&& 

gatewayUrl&& 
)&& 
{'' 
return(( 
new(( (
MerchantGatewayConfiguration(( /
{)) 	
Id** 
=** 
Guid** 
.** 
NewGuid** 
(** 
)** 
,**  

TerminalId++ 
=++ 

terminalId++ #
,++# $
ProviderName,, 
=,, 
providerName,, '
,,,' (

MerchantId-- 
=-- 

merchantId-- #
,--# $
EncryptedApiKey.. 
=.. 
encryptedApiKey.. -
,..- .

GatewayUrl// 
=// 

gatewayUrl// #
,//# $
IsActive00 
=00 
true00 
,00 
Version11 
=11 
$num11 
}22 	
;22	 

}33 
public55 

void55 
UpdateCredentials55 !
(55! "
string55" (

merchantId55) 3
,553 4
string555 ;
encryptedApiKey55< K
,55K L
string55M S

gatewayUrl55T ^
)55^ _
{66 

MerchantId77 
=77 

merchantId77 
;77  
EncryptedApiKey88 
=88 
encryptedApiKey88 )
;88) *

GatewayUrl99 
=99 

gatewayUrl99 
;99  
Version:: 
++:: 
;:: 
};; 
public== 

void== 
UpdateSettings== 
(== 
string>> 
cardTypesAccepted>>  
,>>  !
decimal?? 
signatureThreshold?? "
,??" #
bool@@ 
allowTipAdjustment@@ 
,@@  
boolAA 
isExternalTerminalAA 
,AA  
boolBB 
allowManualEntryBB 
,BB 
boolCC 
enablePreAuthCC 
)CC 
{DD 
CardTypesAcceptedEE 
=EE 
cardTypesAcceptedEE -
;EE- .
SignatureThresholdFF 
=FF 
signatureThresholdFF /
;FF/ 0
AllowTipAdjustmentGG 
=GG 
allowTipAdjustmentGG /
;GG/ 0
IsExternalTerminalHH 
=HH 
isExternalTerminalHH /
;HH/ 0
AllowManualEntryII 
=II 
allowManualEntryII +
;II+ ,
EnablePreAuthJJ 
=JJ 
enablePreAuthJJ %
;JJ% &
VersionKK 
++KK 
;KK 
}LL 
publicNN 

voidNN 
	SetActiveNN 
(NN 
boolNN 
isActiveNN '
)NN' (
{OO 
IsActivePP 
=PP 
isActivePP 
;PP 
VersionQQ 
++QQ 
;QQ 
}RR 
}SS œ^
nC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\MenuModifier.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
MenuModifier 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
null/ 3
!3 4
;4 5
public 

string 
? 
Description 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

Guid 
? 
ModifierGroupId  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
public 

ModifierType 
ModifierType $
{% &
get' *
;* +
private, 3
set4 7
;7 8
}9 :
public 

Money 
	BasePrice 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

decimal 
TaxRate 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

bool  
ShouldPrintToKitchen $
{% &
get' *
;* +
private, 3
set4 7
;7 8
}9 :
public 

bool 
IsSectionWisePrice "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
public 

string 
? 
SectionName 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

string 
? 
MultiplierName !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 

int 
DisplayOrder 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

int 
Version 
{ 
get 
; 
private %
set& )
;) *
}+ ,
	protected 
MenuModifier 
( 
) 
{ 
Name   
=   
string   
.   
Empty   
;   
	BasePrice!! 
=!! 
Money!! 
.!! 
Zero!! 
(!! 
)!!  
;!!  !
}"" 
	protected$$ 
MenuModifier$$ 
($$ 
string$$ !
name$$" &
,$$& '
Money$$( -
	basePrice$$. 7
,$$7 8
int$$9 <
displayOrder$$= I
)$$I J
{%% 
Id&& 

=&& 
Guid&& 
.&& 
NewGuid&& 
(&& 
)&& 
;&& 
Name'' 
='' 
name'' 
;'' 
	BasePrice(( 
=(( 
	basePrice(( 
;(( 
DisplayOrder)) 
=)) 
displayOrder)) #
;))# $
ModifierType** 
=** 
ModifierType** #
.**# $
Normal**$ *
;*** +
IsActive++ 
=++ 
true++ 
;++ 
Version,, 
=,, 
$num,, 
;,, 
}-- 
public22 

static22 
MenuModifier22 
Create22 %
(22% &
string33 
name33 
,33 
ModifierType44 
modifierType44 !
,44! "
Money55 
	basePrice55 
,55 
Guid66 
?66 
modifierGroupId66 
=66 
null66  $
,66$ %
string77 
?77 
description77 
=77 
null77 "
,77" #
decimal88 
taxRate88 
=88 
$num88 
,88 
bool99  
shouldPrintToKitchen99 !
=99" #
true99$ (
,99( )
bool:: 
isSectionWisePrice:: 
=::  !
false::" '
,::' (
string;; 
?;; 
sectionName;; 
=;; 
null;; "
,;;" #
string<< 
?<< 
multiplierName<< 
=<<  
null<<! %
,<<% &
int== 
displayOrder== 
=== 
$num== 
,== 
bool>> 
isActive>> 
=>> 
true>> 
)>> 
{?? 
if@@ 

(@@ 
string@@ 
.@@ 
IsNullOrWhiteSpace@@ %
(@@% &
name@@& *
)@@* +
)@@+ ,
{AA 	
throwBB 
newBB 

ExceptionsBB  
.BB  !*
BusinessRuleViolationExceptionBB! ?
(BB? @
$strBB@ `
)BB` a
;BBa b
}CC 	
ifEE 

(EE 
	basePriceEE 
<EE 
MoneyEE 
.EE 
ZeroEE "
(EE" #
)EE# $
)EE$ %
{FF 	
throwGG 
newGG 

ExceptionsGG  
.GG  !*
BusinessRuleViolationExceptionGG! ?
(GG? @
$strGG@ `
)GG` a
;GGa b
}HH 	
ifJJ 

(JJ 
taxRateJJ 
<JJ 
$numJJ 
||JJ 
taxRateJJ "
>JJ# $
$numJJ% &
)JJ& '
{KK 	
throwLL 
newLL 

ExceptionsLL  
.LL  !*
BusinessRuleViolationExceptionLL! ?
(LL? @
$strLL@ c
)LLc d
;LLd e
}MM 	
returnOO 
newOO 
MenuModifierOO 
{PP 	
IdQQ 
=QQ 
GuidQQ 
.QQ 
NewGuidQQ 
(QQ 
)QQ 
,QQ  
NameRR 
=RR 
nameRR 
,RR 
DescriptionSS 
=SS 
descriptionSS %
,SS% &
ModifierGroupIdTT 
=TT 
modifierGroupIdTT -
,TT- .
ModifierTypeUU 
=UU 
modifierTypeUU '
,UU' (
	BasePriceVV 
=VV 
	basePriceVV !
,VV! "
TaxRateWW 
=WW 
taxRateWW 
,WW  
ShouldPrintToKitchenXX  
=XX! " 
shouldPrintToKitchenXX# 7
,XX7 8
IsSectionWisePriceYY 
=YY  
isSectionWisePriceYY! 3
,YY3 4
SectionNameZZ 
=ZZ 
sectionNameZZ %
,ZZ% &
MultiplierName[[ 
=[[ 
multiplierName[[ +
,[[+ ,
DisplayOrder\\ 
=\\ 
displayOrder\\ '
,\\' (
IsActive]] 
=]] 
isActive]] 
,]]  
Version^^ 
=^^ 
$num^^ 
}__ 	
;__	 

}`` 
publicee 

voidee 

UpdateNameee 
(ee 
stringee !
nameee" &
)ee& '
{ff 
ifgg 

(gg 
stringgg 
.gg 
IsNullOrWhiteSpacegg %
(gg% &
namegg& *
)gg* +
)gg+ ,
{hh 	
throwii 
newii 

Exceptionsii  
.ii  !*
BusinessRuleViolationExceptionii! ?
(ii? @
$strii@ `
)ii` a
;iia b
}jj 	
Namell 
=ll 
namell 
;ll 
}mm 
publicrr 

voidrr 
UpdateDescriptionrr !
(rr! "
stringrr" (
?rr( )
descriptionrr* 5
)rr5 6
{ss 
Descriptiontt 
=tt 
descriptiontt !
;tt! "
}uu 
publiczz 

voidzz 
UpdateBasePricezz 
(zz  
Moneyzz  %
	basePricezz& /
)zz/ 0
{{{ 
if|| 

(|| 
	basePrice|| 
<|| 
Money|| 
.|| 
Zero|| "
(||" #
)||# $
)||$ %
{}} 	
throw~~ 
new~~ 

Exceptions~~  
.~~  !*
BusinessRuleViolationException~~! ?
(~~? @
$str~~@ `
)~~` a
;~~a b
} 	
	BasePrice
ÅÅ 
=
ÅÅ 
	basePrice
ÅÅ 
;
ÅÅ 
}
ÇÇ 
public
áá 

void
áá 
UpdateTaxRate
áá 
(
áá 
decimal
áá %
taxRate
áá& -
)
áá- .
{
àà 
if
ââ 

(
ââ 
taxRate
ââ 
<
ââ 
$num
ââ 
||
ââ 
taxRate
ââ "
>
ââ# $
$num
ââ% &
)
ââ& '
{
ää 	
throw
ãã 
new
ãã 

Exceptions
ãã  
.
ãã  !,
BusinessRuleViolationException
ãã! ?
(
ãã? @
$str
ãã@ c
)
ããc d
;
ããd e
}
åå 	
TaxRate
éé 
=
éé 
taxRate
éé 
;
éé 
}
èè 
public
îî 

void
îî !
UpdateModifierGroup
îî #
(
îî# $
Guid
îî$ (
?
îî( )
modifierGroupId
îî* 9
)
îî9 :
{
ïï 
ModifierGroupId
ññ 
=
ññ 
modifierGroupId
ññ )
;
ññ) *
}
óó 
public
úú 

void
úú %
SetShouldPrintToKitchen
úú '
(
úú' (
bool
úú( ,
shouldPrint
úú- 8
)
úú8 9
{
ùù "
ShouldPrintToKitchen
ûû 
=
ûû 
shouldPrint
ûû *
;
ûû* +
}
üü 
public
§§ 

void
§§ !
SetSectionWisePrice
§§ #
(
§§# $
bool
§§$ ( 
isSectionWisePrice
§§) ;
,
§§; <
string
§§= C
?
§§C D
sectionName
§§E P
=
§§Q R
null
§§S W
)
§§W X
{
••  
IsSectionWisePrice
¶¶ 
=
¶¶  
isSectionWisePrice
¶¶ /
;
¶¶/ 0
SectionName
ßß 
=
ßß 
sectionName
ßß !
;
ßß! "
}
®® 
public
≠≠ 

void
≠≠ 
SetMultiplierName
≠≠ !
(
≠≠! "
string
≠≠" (
?
≠≠( )
multiplierName
≠≠* 8
)
≠≠8 9
{
ÆÆ 
MultiplierName
ØØ 
=
ØØ 
multiplierName
ØØ '
;
ØØ' (
}
∞∞ 
public
µµ 

void
µµ  
UpdateDisplayOrder
µµ "
(
µµ" #
int
µµ# &
displayOrder
µµ' 3
)
µµ3 4
{
∂∂ 
DisplayOrder
∑∑ 
=
∑∑ 
displayOrder
∑∑ #
;
∑∑# $
}
∏∏ 
public
ΩΩ 

void
ΩΩ 
Activate
ΩΩ 
(
ΩΩ 
)
ΩΩ 
{
ææ 
IsActive
øø 
=
øø 
true
øø 
;
øø 
}
¿¿ 
public
≈≈ 

void
≈≈ 

Deactivate
≈≈ 
(
≈≈ 
)
≈≈ 
{
∆∆ 
IsActive
«« 
=
«« 
false
«« 
;
«« 
}
»» 
}…… ≈
wC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\MenuItemModifierGroup.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 !
MenuItemModifierGroup		 "
{

 
public 

Guid 

MenuItemId 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

Guid 
ModifierGroupId 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

int 
DisplayOrder 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

bool 

IsRequired 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

MenuItem 
MenuItem 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
=3 4
null5 9
!9 :
;: ;
public 

ModifierGroup 
ModifierGroup &
{' (
get) ,
;, -
private. 5
set6 9
;9 :
}; <
== >
null? C
!C D
;D E
private !
MenuItemModifierGroup !
(! "
)" #
{$ %
}& '
public 

static !
MenuItemModifierGroup '
Create( .
(. /
Guid 

menuItemId 
, 
Guid 
modifierGroupId 
, 
int 
displayOrder 
= 
$num 
) 
{ 
return 
new !
MenuItemModifierGroup (
{ 	

MenuItemId   
=   

menuItemId   #
,  # $
ModifierGroupId!! 
=!! 
modifierGroupId!! -
,!!- .
DisplayOrder"" 
="" 
displayOrder"" '
}## 	
;##	 

}$$ 
}%% ñ~
jC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\MenuItem.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 
MenuItem

 
{ 
private 
readonly 
List 
< !
MenuItemModifierGroup /
>/ 0
_modifierGroups1 @
=A B
newC F
(F G
)G H
;H I
private 
readonly 

Dictionary 
<  
string  &
,& '
string( .
>. /
_properties0 ;
=< =
new> A
(A B
)B C
;C D
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
null/ 3
!3 4
;4 5
public 

string 
? 
Description 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

string 
? 
Barcode 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

Money 
Price 
{ 
get 
; 
private %
set& )
;) *
}+ ,
public 

decimal 
TaxRate 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

Guid 
? 

CategoryId 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

Guid 
? 
GroupId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

virtual 
MenuCategory 
?  
Category! )
{* +
get, /
;/ 0
private1 8
set9 <
;< =
}> ?
public 

virtual 
	MenuGroup 
? 
Group #
{$ %
get& )
;) *
private+ 2
set3 6
;6 7
}8 9
public 

Guid 
? 
ComboDefinitionId "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
public 

int 
DisplayOrder 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public"" 

bool"" 
	IsVisible"" 
{"" 
get"" 
;""  
private""! (
set"") ,
;"", -
}"". /
public## 

bool## 
IsAvailable## 
{## 
get## !
;##! "
private### *
set##+ .
;##. /
}##0 1
public$$ 

bool$$ 
ShowInKiosk$$ 
{$$ 
get$$ !
;$$! "
private$$# *
set$$+ .
;$$. /
}$$0 1
public%% 

bool%% 
IsStockItem%% 
{%% 
get%% !
;%%! "
private%%# *
set%%+ .
;%%. /
}%%0 1
public(( 

bool((  
ShouldPrintToKitchen(( $
{((% &
get((' *
;((* +
private((, 3
set((4 7
;((7 8
}((9 :
public)) 

Guid)) 
?)) 
PrinterGroupId)) 
{))  !
get))" %
;))% &
private))' .
set))/ 2
;))2 3
}))4 5
public,, 

IReadOnlyCollection,, 
<,, !
MenuItemModifierGroup,, 4
>,,4 5
ModifierGroups,,6 D
=>,,E G
_modifierGroups,,H W
.,,W X

AsReadOnly,,X b
(,,b c
),,c d
;,,d e
public-- 

IReadOnlyDictionary-- 
<-- 
string-- %
,--% &
string--' -
>--- .

Properties--/ 9
=>--: <
_properties--= H
.--H I

AsReadOnly--I S
(--S T
)--T U
;--U V
public00 

int00 
Version00 
{00 
get00 
;00 
private00 %
set00& )
;00) *
}00+ ,
public11 

bool11 
IsActive11 
{11 
get11 
;11 
private11  '
set11( +
;11+ ,
}11- .
public44 

string44 
?44 
	ColorCode44 
=>44 

Properties44  *
.44* +
TryGetValue44+ 6
(446 7
$str447 B
,44B C
out44D G
var44H K
color44L Q
)44Q R
?44S T
color44U Z
:44[ \
null44] a
;44a b
public77 

bool77 
IsVariablePrice77 
=>77  "

Properties77# -
.77- .
TryGetValue77. 9
(779 :
$str77: K
,77K L
out77M P
var77Q T
val77U X
)77X Y
&&77Z \
bool77] a
.77a b
TryParse77b j
(77j k
val77k n
,77n o
out77p s
var77t w
result77x ~
)77~ 
&&
77Ä Ç
result
77É â
;
77â ä
private:: 
MenuItem:: 
(:: 
):: 
{;; 
Name<< 
=<< 
string<< 
.<< 
Empty<< 
;<< 
Price== 
=== 
Money== 
.== 
Zero== 
(== 
)== 
;== 
}>> 
public@@ 

static@@ 
MenuItem@@ 
Create@@ !
(@@! "
stringAA 
nameAA 
,AA 
MoneyBB 
priceBB 
,BB 
decimalCC 
taxRateCC 
=CC 
$numCC 
)CC 
{DD 
ifEE 

(EE 
stringEE 
.EE 
IsNullOrWhiteSpaceEE %
(EE% &
nameEE& *
)EE* +
)EE+ ,
throwFF 
newFF 

ExceptionsFF  
.FF  !*
BusinessRuleViolationExceptionFF! ?
(FF? @
$strFF@ W
)FFW X
;FFX Y
returnHH 
newHH 
MenuItemHH 
{II 	
IdJJ 
=JJ 
GuidJJ 
.JJ 
NewGuidJJ 
(JJ 
)JJ 
,JJ  
NameKK 
=KK 
nameKK 
,KK 
PriceLL 
=LL 
priceLL 
,LL 
TaxRateMM 
=MM 
taxRateMM 
,MM 
	IsVisibleNN 
=NN 
trueNN 
,NN 
IsAvailableOO 
=OO 
trueOO 
,OO  
ShouldPrintToKitchenPP  
=PP! "
truePP# '
,PP' (
IsActiveQQ 
=QQ 
trueQQ 
,QQ 
VersionRR 
=RR 
$numRR 
}SS 	
;SS	 

}TT 
publicVV 

voidVV 
UpdatePriceVV 
(VV 
MoneyVV !
priceVV" '
)VV' (
{WW 
ifXX 

(XX 
priceXX 
<XX 
MoneyXX 
.XX 
ZeroXX 
(XX 
)XX  
)XX  !
throwYY 
newYY 

ExceptionsYY !
.YY! "*
BusinessRuleViolationExceptionYY" @
(YY@ A
$strYYA \
)YY\ ]
;YY] ^
PriceZZ 
=ZZ 
priceZZ 
;ZZ 
}[[ 
private]] 
readonly]] 
List]] 
<]] 

RecipeLine]] $
>]]$ %
_recipeLines]]& 2
=]]3 4
new]]5 8
(]]8 9
)]]9 :
;]]: ;
public^^ 

IReadOnlyCollection^^ 
<^^ 

RecipeLine^^ )
>^^) *
RecipeLines^^+ 6
=>^^7 9
_recipeLines^^: F
.^^F G

AsReadOnly^^G Q
(^^Q R
)^^R S
;^^S T
public`` 

void`` 
AddRecipeLine`` 
(`` 
Guid`` "
inventoryItemId``# 2
,``2 3
decimal``4 ;
quantity``< D
)``D E
{aa 
varbb 
linebb 
=bb 
newbb 

RecipeLinebb !
(bb! "
inventoryItemIdbb" 1
,bb1 2
quantitybb3 ;
)bb; <
;bb< =
_recipeLinescc 
.cc 
Addcc 
(cc 
linecc 
)cc 
;cc 
}dd 
publicff 

voidff 
RemoveRecipeLineff  
(ff  !
Guidff! %
inventoryItemIdff& 5
)ff5 6
{gg 
_recipeLineshh 
.hh 
	RemoveAllhh 
(hh 
xhh  
=>hh! #
xhh$ %
.hh% &
InventoryItemIdhh& 5
==hh6 8
inventoryItemIdhh9 H
)hhH I
;hhI J
}ii 
publickk 

voidkk 

UpdateNamekk 
(kk 
stringkk !
namekk" &
)kk& '
{ll 
ifmm 

(mm 
stringmm 
.mm 
IsNullOrWhiteSpacemm %
(mm% &
namemm& *
)mm* +
)mm+ ,
thrownn 
newnn 

Exceptionsnn  
.nn  !*
BusinessRuleViolationExceptionnn! ?
(nn? @
$strnn@ W
)nnW X
;nnX Y
Nameoo 
=oo 
nameoo 
;oo 
}pp 
publicss 

intss 
StockQuantityss 
{ss 
getss "
;ss" #
privatess$ +
setss, /
;ss/ 0
}ss1 2
publictt 

inttt 
MinimumStockLeveltt  
{tt! "
gettt# &
;tt& '
privatett( /
settt0 3
;tt3 4
}tt5 6
publicuu 

booluu 

TrackStockuu 
{uu 
getuu  
;uu  !
privateuu" )
setuu* -
;uu- .
}uu/ 0
publicww 

voidww 
EnableStockTrackingww #
(ww# $
intww$ '
initialQuantityww( 7
,ww7 8
intww9 <
minimumLevelww= I
=wwJ K
$numwwL M
)wwM N
{xx 

TrackStockyy 
=yy 
trueyy 
;yy 
StockQuantityzz 
=zz 
initialQuantityzz '
;zz' (
MinimumStockLevel{{ 
={{ 
minimumLevel{{ (
;{{( )
}|| 
public~~ 

void~~  
DisableStockTracking~~ $
(~~$ %
)~~% &
{ 

TrackStock
ÄÄ 
=
ÄÄ 
false
ÄÄ 
;
ÄÄ 
}
ÅÅ 
public
ÉÉ 

void
ÉÉ 
AdjustStock
ÉÉ 
(
ÉÉ 
int
ÉÉ 
quantityChange
ÉÉ  .
)
ÉÉ. /
{
ÑÑ 
if
ÖÖ 

(
ÖÖ 
!
ÖÖ 

TrackStock
ÖÖ 
)
ÖÖ 
return
ÖÖ 
;
ÖÖ  
StockQuantity
áá 
+=
áá 
quantityChange
áá '
;
áá' (
}
èè 
public
ëë 

void
ëë 
DeductStock
ëë 
(
ëë 
int
ëë 
quantity
ëë  (
)
ëë( )
{
íí 
if
ìì 

(
ìì 
!
ìì 

TrackStock
ìì 
)
ìì 
return
ìì 
;
ìì  
if
îî 

(
îî 
quantity
îî 
<
îî 
$num
îî 
)
îî 
throw
îî 
new
îî  #

Exceptions
îî$ .
.
îî. /,
BusinessRuleViolationException
îî/ M
(
îîM N
$str
îîN p
)
îîp q
;
îîq r
if
ññ 

(
ññ 
StockQuantity
ññ 
<
ññ 
quantity
ññ $
)
ññ$ %
{
óó 	
throw
òò 
new
òò 

Exceptions
òò !
.
òò! ",
BusinessRuleViolationException
òò" @
(
òò@ A
$"
òòA C
$str
òòC `
{
òò` a
Name
òòa e
}
òòe f
$str
òòf t
{
òòt u
StockQuantityòòu Ç
}òòÇ É
$stròòÉ ê
{òòê ë
quantityòòë ô
}òòô ö
"òòö õ
)òòõ ú
;òòú ù
}
ôô 	
StockQuantity
õõ 
-=
õõ 
quantity
õõ !
;
õõ! "
}
úú 
public
ûû 

void
ûû 
ReturnStock
ûû 
(
ûû 
int
ûû 
quantity
ûû  (
)
ûû( )
{
üü 
if
††	 
(
†† 
!
†† 

TrackStock
†† 
)
†† 
return
††  
;
††  !
if
°°	 
(
°° 
quantity
°° 
<
°° 
$num
°° 
)
°° 
throw
°°  
new
°°! $

Exceptions
°°% /
.
°°/ 0,
BusinessRuleViolationException
°°0 N
(
°°N O
$str
°°O q
)
°°q r
;
°°r s
StockQuantity
££	 
+=
££ 
quantity
££ "
;
££" #
}
§§ 
public
¶¶ 

void
¶¶ 
SetCategory
¶¶ 
(
¶¶ 
Guid
¶¶  

categoryId
¶¶! +
)
¶¶+ ,
{
ßß 
if
®® 

(
®® 

categoryId
®® 
==
®® 
Guid
®® 
.
®® 
Empty
®® $
)
®®$ %
throw
®®& +
new
®®, /
ArgumentException
®®0 A
(
®®A B
$str
®®B W
)
®®W X
;
®®X Y

CategoryId
©© 
=
©© 

categoryId
©© 
;
©©  
}
™™ 
public
¨¨ 

void
¨¨ 
SetGroup
¨¨ 
(
¨¨ 
Guid
¨¨ 
groupId
¨¨ %
)
¨¨% &
{
≠≠ 
if
ÆÆ 

(
ÆÆ 
groupId
ÆÆ 
==
ÆÆ 
Guid
ÆÆ 
.
ÆÆ 
Empty
ÆÆ !
)
ÆÆ! "
throw
ÆÆ# (
new
ÆÆ) ,
ArgumentException
ÆÆ- >
(
ÆÆ> ?
$str
ÆÆ? Q
)
ÆÆQ R
;
ÆÆR S
GroupId
ØØ 
=
ØØ 
groupId
ØØ 
;
ØØ 
}
∞∞ 
public
≤≤ 

void
≤≤ 
SetPrinterGroup
≤≤ 
(
≤≤  
Guid
≤≤  $
?
≤≤$ %
printerGroupId
≤≤& 4
)
≤≤4 5
{
≥≥ 
PrinterGroupId
¥¥ 
=
¥¥ 
printerGroupId
¥¥ '
;
¥¥' (
}
µµ 
public
∑∑ 

void
∑∑  
SetComboDefinition
∑∑ "
(
∑∑" #
Guid
∑∑# '
?
∑∑' (
comboDefinitionId
∑∑) :
)
∑∑: ;
{
∏∏ 
ComboDefinitionId
ππ 
=
ππ 
comboDefinitionId
ππ -
;
ππ- .
}
∫∫ 
public
ºº 

void
ºº 
AddModifierGroup
ºº  
(
ºº  !
ModifierGroup
ºº! .
group
ºº/ 4
,
ºº4 5
int
ºº6 9
displayOrder
ºº: F
=
ººG H
$num
ººI J
)
ººJ K
{
ΩΩ 
}
¬¬ 
}√√ ∑2
kC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\MenuGroup.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
	MenuGroup		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
null/ 3
!3 4
;4 5
public 

Guid 

CategoryId 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

int 
	SortOrder 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

bool 
	IsVisible 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

string 
? 
ButtonColor 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

MenuCategory 
? 
Category !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
private 
	MenuGroup 
( 
) 
{ 
Name 
= 
string 
. 
Empty 
; 
} 
public 

Guid 
? 
PrinterGroupId 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

static 
	MenuGroup 
Create "
(" #
string 
name 
, 
Guid   

categoryId   
,   
int!! 
	sortOrder!! 
=!! 
$num!! 
)!! 
{"" 
if## 

(## 
string## 
.## 
IsNullOrWhiteSpace## %
(##% &
name##& *
)##* +
)##+ ,
throw$$ 
new$$ 

Exceptions$$  
.$$  !*
BusinessRuleViolationException$$! ?
($$? @
$str$$@ ]
)$$] ^
;$$^ _
if&& 

(&& 

categoryId&& 
==&& 
Guid&& 
.&& 
Empty&& $
)&&$ %
throw'' 
new'' 

Exceptions''  
.''  !*
BusinessRuleViolationException''! ?
(''? @
$str''@ b
)''b c
;''c d
return)) 
new)) 
	MenuGroup)) 
{** 	
Id++ 
=++ 
Guid++ 
.++ 
NewGuid++ 
(++ 
)++ 
,++  
Name,, 
=,, 
name,, 
.,, 
Trim,, 
(,, 
),, 
,,, 

CategoryId-- 
=-- 

categoryId-- #
,--# $
	SortOrder.. 
=.. 
	sortOrder.. !
,..! "
	IsVisible// 
=// 
true// 
,// 
IsActive00 
=00 
true00 
}11 	
;11	 

}22 
public44 

void44 

UpdateName44 
(44 
string44 !
name44" &
)44& '
{55 
if66 

(66 
string66 
.66 
IsNullOrWhiteSpace66 %
(66% &
name66& *
)66* +
)66+ ,
throw77 
new77 

Exceptions77  
.77  !*
BusinessRuleViolationException77! ?
(77? @
$str77@ ]
)77] ^
;77^ _
Name99 
=99 
name99 
.99 
Trim99 
(99 
)99 
;99 
}:: 
public<< 

void<< 
UpdateCategory<< 
(<< 
Guid<< #

categoryId<<$ .
)<<. /
{== 
if>> 

(>> 

categoryId>> 
==>> 
Guid>> 
.>> 
Empty>> $
)>>$ %
throw?? 
new?? 

Exceptions??  
.??  !*
BusinessRuleViolationException??! ?
(??? @
$str??@ b
)??b c
;??c d

CategoryIdAA 
=AA 

categoryIdAA 
;AA  
}BB 
publicDD 

voidDD 
UpdateSortOrderDD 
(DD  
intDD  #
	sortOrderDD$ -
)DD- .
{EE 
	SortOrderFF 
=FF 
	sortOrderFF 
;FF 
}GG 
publicII 

voidII 
SetVisibilityII 
(II 
boolII "
	isVisibleII# ,
)II, -
{JJ 
	IsVisibleKK 
=KK 
	isVisibleKK 
;KK 
}LL 
publicNN 

voidNN 
SetButtonColorNN 
(NN 
stringNN %
?NN% &
colorNN' ,
)NN, -
{OO 
ButtonColorPP 
=PP 
colorPP 
;PP 
}QQ 
publicSS 

voidSS 

DeactivateSS 
(SS 
)SS 
{TT 
IsActiveUU 
=UU 
falseUU 
;UU 
}VV 
publicXX 

voidXX 
ActivateXX 
(XX 
)XX 
{YY 
IsActiveZZ 
=ZZ 
trueZZ 
;ZZ 
}[[ 
public]] 

void]] 
SetPrinterGroup]] 
(]]  
Guid]]  $
?]]$ %
printerGroupId]]& 4
)]]4 5
{^^ 
PrinterGroupId__ 
=__ 
printerGroupId__ '
;__' (
}`` 
}aa ¡C
nC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\MenuCategory.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
MenuCategory 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
null/ 3
!3 4
;4 5
public 

int 
	SortOrder 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

bool 
	IsVisible 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

bool 

IsBeverage 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

string 
? 
ButtonColor 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

Guid 
? 
PrinterGroupId 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

Guid 
? 
ParentCategoryId !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 

virtual 
MenuCategory 
?  
Parent! '
{( )
get* -
;- .
private/ 6
set7 :
;: ;
}< =
public 

virtual 
ICollection 
< 
MenuCategory +
>+ ,
Subcategories- :
{; <
get= @
;@ A
privateB I
setJ M
;M N
}O P
=Q R
newS V
ListW [
<[ \
MenuCategory\ h
>h i
(i j
)j k
;k l
private 
MenuCategory 
( 
) 
{ 
Name 
= 
string 
. 
Empty 
; 
}   
public"" 

static"" 
MenuCategory"" 
Create"" %
(""% &
string## 
name## 
,## 
int$$ 
	sortOrder$$ 
=$$ 
$num$$ 
,$$ 
bool%% 

isBeverage%% 
=%% 
false%% 
)%%  
{&& 
if'' 

('' 
string'' 
.'' 
IsNullOrWhiteSpace'' %
(''% &
name''& *
)''* +
)''+ ,
throw(( 
new(( 

Exceptions((  
.((  !*
BusinessRuleViolationException((! ?
(((? @
$str((@ `
)((` a
;((a b
return** 
new** 
MenuCategory** 
{++ 	
Id,, 
=,, 
Guid,, 
.,, 
NewGuid,, 
(,, 
),, 
,,,  
Name-- 
=-- 
name-- 
.-- 
Trim-- 
(-- 
)-- 
,-- 
	SortOrder.. 
=.. 
	sortOrder.. !
,..! "
	IsVisible// 
=// 
true// 
,// 

IsBeverage00 
=00 

isBeverage00 #
,00# $
IsActive11 
=11 
true11 
}22 	
;22	 

}33 
public55 

void55 

UpdateName55 
(55 
string55 !
name55" &
)55& '
{66 
if77 

(77 
string77 
.77 
IsNullOrWhiteSpace77 %
(77% &
name77& *
)77* +
)77+ ,
throw88 
new88 

Exceptions88  
.88  !*
BusinessRuleViolationException88! ?
(88? @
$str88@ `
)88` a
;88a b
Name:: 
=:: 
name:: 
.:: 
Trim:: 
(:: 
):: 
;:: 
};; 
public== 

void== 
UpdateSortOrder== 
(==  
int==  #
	sortOrder==$ -
)==- .
{>> 
	SortOrder?? 
=?? 
	sortOrder?? 
;?? 
}@@ 
publicBB 

voidBB 
SetVisibilityBB 
(BB 
boolBB "
	isVisibleBB# ,
)BB, -
{CC 
	IsVisibleDD 
=DD 
	isVisibleDD 
;DD 
}EE 
publicGG 

voidGG 
SetBeverageFlagGG 
(GG  
boolGG  $

isBeverageGG% /
)GG/ 0
{HH 

IsBeverageII 
=II 

isBeverageII 
;II  
}JJ 
publicLL 

voidLL 
SetButtonColorLL 
(LL 
stringLL %
?LL% &
colorLL' ,
)LL, -
{MM 
ButtonColorNN 
=NN 
colorNN 
;NN 
}OO 
publicQQ 

voidQQ 

DeactivateQQ 
(QQ 
)QQ 
{RR 
IsActiveSS 
=SS 
falseSS 
;SS 
}TT 
publicVV 

voidVV 
ActivateVV 
(VV 
)VV 
{WW 
IsActiveXX 
=XX 
trueXX 
;XX 
}YY 
public[[ 

void[[ 
SetPrinterGroup[[ 
([[  
Guid[[  $
?[[$ %
printerGroupId[[& 4
)[[4 5
{\\ 
PrinterGroupId]] 
=]] 
printerGroupId]] '
;]]' (
}^^ 
publicii 

voidii 
	SetParentii 
(ii 
Guidii 
?ii 
parentCategoryIdii  0
)ii0 1
{jj 
ifll 

(ll 
parentCategoryIdll 
.ll 
HasValuell %
&&ll& (
parentCategoryIdll) 9
.ll9 :
Valuell: ?
==ll@ B
IdllC E
)llE F
throwmm 
newmm 

Exceptionsmm  
.mm  !*
BusinessRuleViolationExceptionmm! ?
(mm? @
$strnn 8
)nn8 9
;nn9 :
ParentCategoryIdpp 
=pp 
parentCategoryIdpp +
;pp+ ,
}qq 
publicvv 

voidvv 
ClearParentvv 
(vv 
)vv 
{ww 
ParentCategoryIdxx 
=xx 
nullxx 
;xx  
}yy 
public
ÄÄ 

int
ÄÄ 
GetDepth
ÄÄ 
(
ÄÄ 
)
ÄÄ 
{
ÅÅ 
int
ÇÇ 
depth
ÇÇ 
=
ÇÇ 
$num
ÇÇ 
;
ÇÇ 
var
ÉÉ 
current
ÉÉ 
=
ÉÉ 
Parent
ÉÉ 
;
ÉÉ 
while
ÖÖ 
(
ÖÖ 
current
ÖÖ 
!=
ÖÖ 
null
ÖÖ 
)
ÖÖ 
{
ÜÜ 	
depth
áá 
++
áá 
;
áá 
current
àà 
=
àà 
current
àà 
.
àà 
Parent
àà $
;
àà$ %
if
ãã 
(
ãã 
depth
ãã 
>
ãã 
$num
ãã 
)
ãã 
break
ãã !
;
ãã! "
}
åå 	
return
éé 
depth
éé 
;
éé 
}
èè 
public
îî 

bool
îî 
IsRoot
îî 
=>
îî 
!
îî 
ParentCategoryId
îî +
.
îî+ ,
HasValue
îî, 4
;
îî4 5
public
ôô 

bool
ôô 
HasSubcategories
ôô  
=>
ôô! #
Subcategories
ôô$ 1
.
ôô1 2
Count
ôô2 7
>
ôô8 9
$num
ôô: ;
;
ôô; <
}öö Ω;
pC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\MembershipTier.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 
MembershipTier

 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

string 
Description 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
string6 <
.< =
Empty= B
;B C
public 

decimal 
DiscountPercent "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
public 

decimal 
? 
HourlyRateDiscount &
{' (
get) ,
;, -
private. 5
set6 9
;9 :
}; <
public 

bool 
IncludesFreeGuests "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
public 

int 
FreeGuestsPerVisit !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 

Money 

MonthlyFee 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

Money 
	AnnualFee 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

int 
	SortOrder 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
private 
MembershipTier 
( 
) 
{ 

MonthlyFee 
= 
Money 
. 
Zero 
(  
)  !
;! "
	AnnualFee 
= 
Money 
. 
Zero 
( 
)  
;  !
} 
public 

static 
MembershipTier  
Create! '
(' (
string   
name   
,   
decimal!! 
discountPercent!! 
,!!  
Money"" 

monthlyFee"" 
,"" 
string## 
description## 
=## 
$str## 
,##  
decimal$$ 
?$$ 
hourlyRateDiscount$$ #
=$$$ %
null$$& *
)$$* +
{%% 
if&& 

(&& 
string&& 
.&& 
IsNullOrWhiteSpace&& %
(&&% &
name&&& *
)&&* +
)&&+ ,
throw'' 
new'' *
BusinessRuleViolationException'' 4
(''4 5
$str''5 M
)''M N
;''N O
if)) 

()) 
discountPercent)) 
<)) 
$num)) 
||))  "
discountPercent))# 2
>))3 4
$num))5 8
)))8 9
throw** 
new** *
BusinessRuleViolationException** 4
(**4 5
$str**5 b
)**b c
;**c d
if,, 

(,, 

monthlyFee,, 
<,, 
Money,, 
.,, 
Zero,, #
(,,# $
),,$ %
),,% &
throw-- 
new-- *
BusinessRuleViolationException-- 4
(--4 5
$str--5 V
)--V W
;--W X
return// 
new// 
MembershipTier// !
{00 	
Id11 
=11 
Guid11 
.11 
NewGuid11 
(11 
)11 
,11  
Name22 
=22 
name22 
,22 
Description33 
=33 
description33 %
,33% &
DiscountPercent44 
=44 
discountPercent44 -
,44- .
HourlyRateDiscount55 
=55  
hourlyRateDiscount55! 3
,553 4

MonthlyFee66 
=66 

monthlyFee66 #
,66# $
	AnnualFee77 
=77 

monthlyFee77 "
*77# $
$num77% '
,77' (
IsActive88 
=88 
true88 
}99 	
;99	 

}:: 
public<< 

Money<<  
CalculateMemberPrice<< %
(<<% &
Money<<& +
regularPrice<<, 8
)<<8 9
{== 
if>> 

(>> 
DiscountPercent>> 
==>> 
$num>>  
)>>  !
return>>" (
regularPrice>>) 5
;>>5 6
var?? 
discountAmount?? 
=?? 
regularPrice?? )
*??* +
(??, -
DiscountPercent??- <
/??= >
$num??? C
)??C D
;??D E
return@@ 
regularPrice@@ 
-@@ 
discountAmount@@ ,
;@@, -
}AA 
publicCC 

decimalCC "
GetEffectiveHourlyRateCC )
(CC) *
decimalCC* 1
baseRateCC2 :
)CC: ;
{DD 
ifEE 

(EE 
!EE 
HourlyRateDiscountEE 
.EE  
HasValueEE  (
)EE( )
returnEE* 0
baseRateEE1 9
;EE9 :
varFF 
rateFF 
=FF 
baseRateFF 
-FF 
HourlyRateDiscountFF 0
.FF0 1
ValueFF1 6
;FF6 7
returnGG 
rateGG 
>GG 
$numGG 
?GG 
rateGG 
:GG  
$numGG! "
;GG" #
}HH 
publicJJ 

voidJJ 
UpdateBenefitsJJ 
(JJ 
decimalJJ &
discountPercentJJ' 6
,JJ6 7
decimalJJ8 ?
?JJ? @
hourlyRateDiscountJJA S
)JJS T
{KK 
ifLL 

(LL 
discountPercentLL 
<LL 
$numLL 
||LL  "
discountPercentLL# 2
>LL3 4
$numLL5 8
)LL8 9
throwMM 
newMM *
BusinessRuleViolationExceptionMM 4
(MM4 5
$strMM5 b
)MMb c
;MMc d
DiscountPercentOO 
=OO 
discountPercentOO )
;OO) *
HourlyRateDiscountPP 
=PP 
hourlyRateDiscountPP /
;PP/ 0
}QQ 
publicSS 

voidSS 

DeactivateSS 
(SS 
)SS 
=>SS 
IsActiveSS  (
=SS) *
falseSS+ 0
;SS0 1
publicTT 

voidTT 

ReactivateTT 
(TT 
)TT 
=>TT 
IsActiveTT  (
=TT) *
trueTT+ /
;TT/ 0
}UU ⁄
rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\MembershipStatus.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
enum 
MembershipStatus 
{ 
Active 

= 
$num 
, 
Expired		 
=		 
$num		 
,		 
	Suspended

 
=

 
$num

 
,

 
	Cancelled 
= 
$num 
} “;
hC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Member.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 
Member

 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 

CustomerId 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

Guid 
TierId 
{ 
get 
; 
private %
set& )
;) *
}+ ,
public 

string 
MemberNumber 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
=5 6
string7 =
.= >
Empty> C
;C D
public 

DateTime 
JoinDate 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

DateTime 
? 
ExpirationDate #
{$ %
get& )
;) *
private+ 2
set3 6
;6 7
}8 9
public 

MembershipStatus 
Status "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
public 

Money 
PrepaidBalance 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

Customer 
Customer 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
=3 4
null5 9
!9 :
;: ;
public 

MembershipTier 
Tier 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
=5 6
null7 ;
!; <
;< =
private 
Member 
( 
) 
{ 
PrepaidBalance 
= 
Money 
. 
Zero #
(# $
)$ %
;% &
} 
public 

static 
Member 
Create 
(  
Guid  $

customerId% /
,/ 0
Guid1 5
tierId6 <
,< =
string> D
memberNumberE Q
)Q R
{   
if!! 

(!! 
string!! 
.!! 
IsNullOrWhiteSpace!! %
(!!% &
memberNumber!!& 2
)!!2 3
)!!3 4
throw"" 
new"" *
BusinessRuleViolationException"" 4
(""4 5
$str""5 Q
)""Q R
;""R S
return$$ 
new$$ 
Member$$ 
{%% 	
Id&& 
=&& 
Guid&& 
.&& 
NewGuid&& 
(&& 
)&& 
,&&  

CustomerId'' 
='' 

customerId'' #
,''# $
TierId(( 
=(( 
tierId(( 
,(( 
MemberNumber)) 
=)) 
memberNumber)) '
,))' (
JoinDate** 
=** 
DateTime** 
.**  
UtcNow**  &
,**& '
Status++ 
=++ 
MembershipStatus++ %
.++% &
Active++& ,
,++, -
PrepaidBalance,, 
=,, 
Money,, "
.,," #
Zero,,# '
(,,' (
),,( )
}-- 	
;--	 

}.. 
public00 

bool00 
IsActive00 
=>00 
Status00 "
==00# %
MembershipStatus00& 6
.006 7
Active007 =
&&00> @
(11 
ExpirationDate11 *
==11+ -
null11. 2
||113 5
ExpirationDate116 D
>11E F
DateTime11G O
.11O P
UtcNow11P V
)11V W
;11W X
public33 

void33 
Renew33 
(33 
DateTime33 
newExpirationDate33 0
)330 1
{44 
if55 

(55 
newExpirationDate55 
<=55  
DateTime55! )
.55) *
UtcNow55* 0
)550 1
throw66 
new66 *
BusinessRuleViolationException66 4
(664 5
$str665 a
)66a b
;66b c
ExpirationDate88 
=88 
newExpirationDate88 *
;88* +
if99 

(99 
Status99 
==99 
MembershipStatus99 &
.99& '
Expired99' .
)99. /
{:: 	
Status;; 
=;; 
MembershipStatus;; %
.;;% &
Active;;& ,
;;;, -
}<< 	
}== 
public?? 

void?? 
Suspend?? 
(?? 
string?? 
reason?? %
)??% &
{@@ 
ifAA 

(AA 
stringAA 
.AA 
IsNullOrWhiteSpaceAA %
(AA% &
reasonAA& ,
)AA, -
)AA- .
throwBB 
newBB *
BusinessRuleViolationExceptionBB 4
(BB4 5
$strBB5 U
)BBU V
;BBV W
StatusDD 
=DD 
MembershipStatusDD !
.DD! "
	SuspendedDD" +
;DD+ ,
}EE 
publicGG 

voidGG 

ReactivateGG 
(GG 
)GG 
{HH 
StatusII 
=II 
MembershipStatusII !
.II! "
ActiveII" (
;II( )
}JJ 
publicLL 

voidLL 
UpgradeTierLL 
(LL 
GuidLL  
	newTierIdLL! *
)LL* +
{MM 
TierIdNN 
=NN 
	newTierIdNN 
;NN 
}OO 
publicQQ 

voidQQ 
AddPrepaidCreditQQ  
(QQ  !
MoneyQQ! &
amountQQ' -
)QQ- .
{RR 
ifSS 

(SS 
amountSS 
<=SS 
MoneySS 
.SS 
ZeroSS  
(SS  !
)SS! "
)SS" #
throwTT 
newTT *
BusinessRuleViolationExceptionTT 4
(TT4 5
$strTT5 V
)TTV W
;TTW X
PrepaidBalanceVV 
+=VV 
amountVV  
;VV  !
}WW 
publicYY 

boolYY 
TryDeductCreditYY 
(YY  
MoneyYY  %
amountYY& ,
)YY, -
{ZZ 
if[[ 

([[ 
amount[[ 
<=[[ 
Money[[ 
.[[ 
Zero[[  
([[  !
)[[! "
)[[" #
throw\\ 
new\\ *
BusinessRuleViolationException\\ 4
(\\4 5
$str\\5 Y
)\\Y Z
;\\Z [
if^^ 

(^^ 
PrepaidBalance^^ 
<^^ 
amount^^ #
)^^# $
return__ 
false__ 
;__ 
PrepaidBalanceaa 
-=aa 
amountaa  
;aa  !
returnbb 
truebb 
;bb 
}cc 
}dd ø
rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\KitchenOrderItem.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
KitchenOrderItem 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public		 

Guid		 
KitchenOrderId		 
{		  
get		! $
;		$ %
private		& -
set		. 1
;		1 2
}		3 4
public

 

Guid

 
TicketItemId

 
{

 
get

 "
;

" #
private

$ +
set

, /
;

/ 0
}

1 2
public 

string 
ItemName 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
=1 2
string3 9
.9 :
Empty: ?
;? @
public 

int 
Quantity 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

Guid 
DestinationId 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

List 
< 
string 
> 
	Modifiers !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
=8 9
new: =
(= >
)> ?
;? @
	protected 
KitchenOrderItem 
( 
)  
{! "
}# $
public 

KitchenOrderItem 
( 
Guid  
kitchenOrderId! /
,/ 0
Guid1 5
ticketItemId6 B
,B C
stringD J
itemNameK S
,S T
intU X
quantityY a
,a b
Guidc g
destinationIdh u
,u v
Listw {
<{ |
string	| Ç
>
Ç É
	modifiers
Ñ ç
)
ç é
{ 
Id 

= 
Guid 
. 
NewGuid 
( 
) 
; 
KitchenOrderId 
= 
kitchenOrderId '
;' (
TicketItemId 
= 
ticketItemId #
;# $
ItemName 
= 
itemName 
; 
Quantity 
= 
quantity 
; 
DestinationId 
= 
destinationId %
;% &
	Modifiers 
= 
	modifiers 
??  
new! $
List% )
<) *
string* 0
>0 1
(1 2
)2 3
;3 4
} 
} ⁄%
nC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\KitchenOrder.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
KitchenOrder 
{ 
public		 

Guid		 
Id		 
{		 
get		 
;		 
private		 !
set		" %
;		% &
}		' (
public

 

Guid

 
TicketId

 
{

 
get

 
;

 
private

  '
set

( +
;

+ ,
}

- .
public 

string 

ServerName 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
=3 4
string5 ;
.; <
Empty< A
;A B
public 

string 
TableNumber 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
string6 <
.< =
Empty= B
;B C
public 

DateTime 
	Timestamp 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

KitchenStatus 
Status 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
private 
readonly 
List 
< 
KitchenOrderItem *
>* +
_items, 2
=3 4
new5 8
(8 9
)9 :
;: ;
public 

IReadOnlyCollection 
< 
KitchenOrderItem /
>/ 0
Items1 6
=>7 9
_items: @
.@ A

AsReadOnlyA K
(K L
)L M
;M N
	protected 
KitchenOrder 
( 
) 
{ 
}  
public 

KitchenOrder 
( 
Guid 
ticketId %
,% &
string' -

serverName. 8
,8 9
string: @
tableNumberA L
)L M
{ 
Id 

= 
Guid 
. 
NewGuid 
( 
) 
; 
TicketId 
= 
ticketId 
; 

ServerName 
= 

serverName 
;  
TableNumber 
= 
tableNumber !
;! "
	Timestamp 
= 
DateTime 
. 
UtcNow #
;# $
Status 
= 
KitchenStatus 
. 
New "
;" #
} 
public 

void 
AddItem 
( 
Guid 
ticketItemId )
,) *
string+ 1
itemName2 :
,: ;
int< ?
quantity@ H
,H I
GuidJ N
destinationIdO \
,\ ]
List^ b
<b c
stringc i
>i j
	modifiersk t
)t u
{   
var!! 
item!! 
=!! 
new!! 
KitchenOrderItem!! '
(!!' (
Id!!( *
,!!* +
ticketItemId!!, 8
,!!8 9
itemName!!: B
,!!B C
quantity!!D L
,!!L M
destinationId!!N [
,!![ \
	modifiers!!] f
)!!f g
;!!g h
_items"" 
."" 
Add"" 
("" 
item"" 
)"" 
;"" 
}## 
public%% 

void%% 
Bump%% 
(%% 
)%% 
{&& 
if'' 

('' 
Status'' 
=='' 
KitchenStatus'' #
.''# $
New''$ '
)''' (
{(( 	
Status)) 
=)) 
KitchenStatus)) "
.))" #
Cooking))# *
;))* +
}** 	
else++ 
if++ 
(++ 
Status++ 
==++ 
KitchenStatus++ (
.++( )
Cooking++) 0
)++0 1
{,, 	
Status-- 
=-- 
KitchenStatus-- "
.--" #
Done--# '
;--' (
}.. 	
}// 
public11 

void11 
Void11 
(11 
)11 
{22 
Status33 
=33 
KitchenStatus33 
.33 
Void33 #
;33# $
}44 
}55 ”"
oC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\InventoryItem.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
InventoryItem 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
null/ 3
!3 4
;4 5
public		 

string		 
Unit		 
{		 
get		 
;		 
private		 %
set		& )
;		) *
}		+ ,
=		- .
$str		/ 5
;		5 6
public

 

decimal

 
StockQuantity

  
{

! "
get

# &
;

& '
private

( /
set

0 3
;

3 4
}

5 6
public 

decimal 
ReorderPoint 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
private 
InventoryItem 
( 
) 
{ 
} 
public 

static 
InventoryItem 
Create  &
(& '
string' -
name. 2
,2 3
string4 :
unit; ?
,? @
decimalA H
stockQuantityI V
,V W
decimalX _
reorderPoint` l
)l m
{ 
if 

( 
string 
. 
IsNullOrWhiteSpace %
(% &
name& *
)* +
)+ ,
throw- 2
new3 6
ArgumentException7 H
(H I
$strI _
)_ `
;` a
return 
new 
InventoryItem  
{ 	
Id 
= 
Guid 
. 
NewGuid 
( 
) 
,  
Name 
= 
name 
, 
Unit 
= 
unit 
, 
StockQuantity 
= 
stockQuantity )
,) *
ReorderPoint 
= 
reorderPoint '
,' (
IsActive 
= 
true 
} 	
;	 

} 
public 

void 

UpdateName 
( 
string !
name" &
)& '
{   
if!! 

(!! 
string!! 
.!! 
IsNullOrWhiteSpace!! %
(!!% &
name!!& *
)!!* +
)!!+ ,
throw!!- 2
new!!3 6
ArgumentException!!7 H
(!!H I
$str!!I _
)!!_ `
;!!` a
Name"" 
="" 
name"" 
;"" 
}## 
public%% 

void%% 

UpdateUnit%% 
(%% 
string%% !
unit%%" &
)%%& '
=>%%( *
Unit%%+ /
=%%0 1
unit%%2 6
;%%6 7
public'' 

void'' 
AdjustStock'' 
('' 
decimal'' #
quantityDelta''$ 1
)''1 2
{(( 
StockQuantity)) 
+=)) 
quantityDelta)) &
;))& '
}** 
public,, 

void,, 
SetReorderPoint,, 
(,,  
decimal,,  '
point,,( -
),,- .
{-- 
ReorderPoint.. 
=.. 
point.. 
;.. 
}// 
public11 

void11 
Activate11 
(11 
)11 
=>11 
IsActive11 &
=11' (
true11) -
;11- .
public33 

void33 

Deactivate33 
(33 
)33 
=>33 
IsActive33  (
=33) *
false33+ 0
;330 1
}44 Ñ
uC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\InventoryAdjustment.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
InventoryAdjustment  
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 
InventoryItemId 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public		 

decimal		 
QuantityDelta		  
{		! "
get		# &
;		& '
private		( /
set		0 3
;		3 4
}		5 6
public

 

string

 
Reason

 
{

 
get

 
;

 
private

  '
set

( +
;

+ ,
}

- .
=

/ 0
null

1 5
!

5 6
;

6 7
public 

DateTime 

AdjustedAt 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

Guid 
? 
UserId 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

virtual 
InventoryItem  
InventoryItem! .
{/ 0
get1 4
;4 5
private6 =
set> A
;A B
}C D
=E F
nullG K
!K L
;L M
private 
InventoryAdjustment 
(  
)  !
{" #
}$ %
public 

static 
InventoryAdjustment %
Create& ,
(, -
Guid- 1
itemId2 8
,8 9
decimal: A
deltaB G
,G H
stringI O
reasonP V
,V W
GuidX \
?\ ]
userId^ d
=e f
nullg k
)k l
{ 
if 

( 
string 
. 
IsNullOrWhiteSpace %
(% &
reason& ,
), -
)- .
throw/ 4
new5 8
ArgumentException9 J
(J K
$strK n
)n o
;o p
return 
new 
InventoryAdjustment &
{ 	
Id 
= 
Guid 
. 
NewGuid 
( 
) 
,  
InventoryItemId 
= 
itemId $
,$ %
QuantityDelta 
= 
delta !
,! "
Reason 
= 
reason 
, 

AdjustedAt 
= 
DateTime !
.! "
UtcNow" (
,( )
UserId 
= 
userId 
} 	
;	 

} 
}   ë
qC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\GroupSettlement.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
GroupSettlement 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public		 

Guid		 
MasterPaymentId		 
{		  !
get		" %
;		% &
private		' .
set		/ 2
;		2 3
}		4 5
public 

List 
< 
Guid 
> 
ChildTicketIds $
{% &
get' *
;* +
private, 3
set4 7
;7 8
}9 :
=; <
new= @
(@ A
)A B
;B C
public 

string 
Strategy 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
=1 2
$str3 ?
;? @
	protected 
GroupSettlement 
( 
) 
{  !
}" #
public 

GroupSettlement 
( 
Guid 
masterPaymentId  /
,/ 0
List1 5
<5 6
Guid6 :
>: ;
childTicketIds< J
,J K
stringL R
strategyS [
=\ ]
$str^ j
)j k
{ 
Id 

= 
Guid 
. 
NewGuid 
( 
) 
; 
MasterPaymentId 
= 
masterPaymentId )
;) *
ChildTicketIds 
= 
childTicketIds '
??( *
new+ .
List/ 3
<3 4
Guid4 8
>8 9
(9 :
): ;
;; <
Strategy 
= 
strategy 
; 
} 
} Ø
jC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Gratuity.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
Gratuity		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 
TicketId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

Money 
Amount 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

bool 
Paid 
{ 
get 
; 
private #
set$ '
;' (
}) *
public 

bool 
Refunded 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

Guid 

TerminalId 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

UserId 
OwnerId 
{ 
get 
;  
private! (
set) ,
;, -
}. /
=0 1
null2 6
!6 7
;7 8
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
private 
Gratuity 
( 
) 
{ 
Amount 
= 
Money 
. 
Zero 
( 
) 
; 
} 
public 

static 
Gratuity 
Create !
(! "
Guid 
ticketId 
, 
Money 
amount 
, 
Guid 

terminalId 
, 
UserId 
ownerId 
) 
{ 
if 

( 
amount 
< 
Money 
. 
Zero 
(  
)  !
)! "
{   	
throw!! 
new!! 

Exceptions!!  
.!!  !*
BusinessRuleViolationException!!! ?
(!!? @
$str!!@ e
)!!e f
;!!f g
}"" 	
return$$ 
new$$ 
Gratuity$$ 
{%% 	
Id&& 
=&& 
Guid&& 
.&& 
NewGuid&& 
(&& 
)&& 
,&&  
TicketId'' 
='' 
ticketId'' 
,''  
Amount(( 
=(( 
amount(( 
,(( 

TerminalId)) 
=)) 

terminalId)) #
,))# $
OwnerId** 
=** 
ownerId** 
,** 
	CreatedAt++ 
=++ 
DateTime++  
.++  !
UtcNow++! '
,++' (
Paid,, 
=,, 
false,, 
,,, 
Refunded-- 
=-- 
false-- 
}.. 	
;..	 

}// 
public11 

void11 

MarkAsPaid11 
(11 
)11 
{22 
Paid33 
=33 
true33 
;33 
}44 
public66 

void66 
MarkAsRefunded66 
(66 
)66  
{77 
Refunded88 
=88 
true88 
;88 
}99 
}:: ‚.
xC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\GiftCertificatePayment.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 "
GiftCertificatePayment		 #
:		$ %
Payment		& -
{

 
public 

string !
GiftCertificateNumber '
{( )
get* -
;- .
	protected/ 8
set9 <
;< =
}> ?
=@ A
nullB F
!F G
;G H
public 

Money 
OriginalAmount 
{  !
get" %
;% &
	protected' 0
set1 4
;4 5
}6 7
public 

Money 
RemainingBalance !
{" #
get$ '
;' (
	protected) 2
set3 6
;6 7
}8 9
	protected "
GiftCertificatePayment $
($ %
)% &
{ 
OriginalAmount 
= 
Money 
. 
Zero #
(# $
)$ %
;% &
RemainingBalance 
= 
Money  
.  !
Zero! %
(% &
)& '
;' (
} 
	protected "
GiftCertificatePayment $
($ %
Guid 
ticketId 
, 
Money 
amount 
, 
UserId 
processedBy 
, 
Guid 

terminalId 
, 
string !
giftCertificateNumber $
,$ %
Money 
originalAmount 
, 
Money 
remainingBalance 
, 
string 
? 
globalId 
= 
null 
,  
Guid 
? 
splitGroupId 
= 
null !
,! "
int 
? 
splitSequence 
= 
null !
)! "
:   	
base  
 
(   
ticketId   
,   
PaymentType   $
.  $ %
GiftCertificate  % 4
,  4 5
amount  6 <
,  < =
processedBy  > I
,  I J

terminalId  K U
,  U V
globalId  W _
,  _ `
splitGroupId  a m
,  m n
splitSequence  o |
)  | }
{!! 
if"" 

("" 
string"" 
."" 
IsNullOrWhiteSpace"" %
(""% &!
giftCertificateNumber""& ;
)""; <
)""< =
{## 	
throw$$ 
new$$ 
ArgumentException$$ '
($$' (
$str$$( Z
,$$Z [
nameof$$\ b
($$b c!
giftCertificateNumber$$c x
)$$x y
)$$y z
;$$z {
}%% 	!
GiftCertificateNumber'' 
='' !
giftCertificateNumber''  5
;''5 6
OriginalAmount(( 
=(( 
originalAmount(( '
;((' (
RemainingBalance)) 
=)) 
remainingBalance)) +
;))+ ,
IsAuthorizable** 
=** 
false** 
;** 
}++ 
public00 

static00 "
GiftCertificatePayment00 (
Create00) /
(00/ 0
Guid11 
ticketId11 
,11 
Money22 
amount22 
,22 
UserId33 
processedBy33 
,33 
Guid44 

terminalId44 
,44 
string55 !
giftCertificateNumber55 $
,55$ %
Money66 
originalAmount66 
,66 
Money77 
remainingBalance77 
,77 
string88 
?88 
globalId88 
=88 
null88 
,88  
Guid99 
?99 
splitGroupId99 
=99 
null99 !
,99! "
int:: 
?:: 
splitSequence:: 
=:: 
null:: !
)::! "
{;; 
if<< 

(<< 
amount<< 
><< 
remainingBalance<< %
)<<% &
{== 	
throw>> 
new>> 

Exceptions>>  
.>>  !*
BusinessRuleViolationException>>! ?
(>>? @
$"?? 
$str?? "
{??" #
amount??# )
}??) *
$str??* G
{??G H
remainingBalance??H X
}??X Y
$str??Y [
"??[ \
)??\ ]
;??] ^
}@@ 	
returnBB 
newBB "
GiftCertificatePaymentBB )
(BB) *
ticketIdCC 
,CC 
amountDD 
,DD 
processedByEE 
,EE 

terminalIdFF 
,FF !
giftCertificateNumberGG !
,GG! "
originalAmountHH 
,HH 
remainingBalanceII 
,II 
globalIdJJ 
,JJ 
splitGroupIdKK 
,KK 
splitSequenceLL 
)LL 
;LL 
}MM 
publicRR 

voidRR "
UpdateRemainingBalanceRR &
(RR& '
MoneyRR' ,

newBalanceRR- 7
)RR7 8
{SS 
ifTT 

(TT 

newBalanceTT 
<TT 
MoneyTT 
.TT 
ZeroTT #
(TT# $
)TT$ %
)TT% &
{UU 	
throwVV 
newVV 

ExceptionsVV  
.VV  !*
BusinessRuleViolationExceptionVV! ?
(VV? @
$strVV@ g
)VVg h
;VVh i
}WW 	
RemainingBalanceYY 
=YY 

newBalanceYY %
;YY% &
}ZZ 
}[[ £O
mC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\GameHistory.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
GameHistory 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 
	SessionId 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

Guid 
TableId 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

GameType 
GameType 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

DateTime 
	StartTime 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
EndTime 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

TimeSpan 
Duration 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

int 
PlayerCount 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

Money 
TotalCharge 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
=3 4
Money5 :
.: ;
Zero; ?
(? @
)@ A
;A B
public 

string 
? 
Winner 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 


Dictionary 
< 
string 
, 
object $
>$ %
GameData& .
{/ 0
get1 4
;4 5
private6 =
set> A
;A B
}C D
=E F
newG J
(J K
)K L
;L M
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
private 
GameHistory 
( 
) 
{ 
} 
public** 

static** 
GameHistory** 
Create** $
(**$ %
Guid++ 
	sessionId++ 
,++ 
Guid,, 
tableId,, 
,,, 
GameType-- 
gameType-- 
,-- 
DateTime.. 
	startTime.. 
,.. 
int// 
playerCount// 
)// 
{00 
if11 

(11 
	sessionId11 
==11 
Guid11 
.11 
Empty11 #
)11# $
{22 	
throw33 
new33 
ArgumentException33 '
(33' (
$str33( E
,33E F
nameof33G M
(33M N
	sessionId33N W
)33W X
)33X Y
;33Y Z
}44 	
if66 

(66 
tableId66 
==66 
Guid66 
.66 
Empty66 !
)66! "
{77 	
throw88 
new88 
ArgumentException88 '
(88' (
$str88( C
,88C D
nameof88E K
(88K L
tableId88L S
)88S T
)88T U
;88U V
}99 	
if;; 

(;; 
playerCount;; 
<=;; 
$num;; 
);; 
{<< 	
throw== 
new== 
ArgumentException== '
(==' (
$str==( Q
,==Q R
nameof==S Y
(==Y Z
playerCount==Z e
)==e f
)==f g
;==g h
}>> 	
if@@ 

(@@ 
	startTime@@ 
>@@ 
DateTime@@  
.@@  !
UtcNow@@! '
)@@' (
{AA 	
throwBB 
newBB 
ArgumentExceptionBB '
(BB' (
$strBB( M
,BBM N
nameofBBO U
(BBU V
	startTimeBBV _
)BB_ `
)BB` a
;BBa b
}CC 	
returnEE 
newEE 
GameHistoryEE 
{FF 	
IdGG 
=GG 
GuidGG 
.GG 
NewGuidGG 
(GG 
)GG 
,GG  
	SessionIdHH 
=HH 
	sessionIdHH !
,HH! "
TableIdII 
=II 
tableIdII 
,II 
GameTypeJJ 
=JJ 
gameTypeJJ 
,JJ  
	StartTimeKK 
=KK 
	startTimeKK !
,KK! "
PlayerCountLL 
=LL 
playerCountLL %
,LL% &
TotalChargeMM 
=MM 
MoneyMM 
.MM  
ZeroMM  $
(MM$ %
)MM% &
,MM& '
GameDataNN 
=NN 
newNN 

DictionaryNN %
<NN% &
stringNN& ,
,NN, -
objectNN. 4
>NN4 5
(NN5 6
)NN6 7
,NN7 8
	CreatedAtOO 
=OO 
DateTimeOO  
.OO  !
UtcNowOO! '
}PP 	
;PP	 

}QQ 
publicZZ 

voidZZ 
EndGameZZ 
(ZZ 
MoneyZZ 
totalChargeZZ )
,ZZ) *
stringZZ+ 1
?ZZ1 2
winnerZZ3 9
=ZZ: ;
nullZZ< @
)ZZ@ A
{[[ 
if\\ 

(\\ 
totalCharge\\ 
==\\ 
null\\ 
)\\  
{]] 	
throw^^ 
new^^ !
ArgumentNullException^^ +
(^^+ ,
nameof^^, 2
(^^2 3
totalCharge^^3 >
)^^> ?
)^^? @
;^^@ A
}__ 	
ifaa 

(aa 
EndTimeaa 
!=aa 
defaultaa 
)aa 
{bb 	
throwcc 
newcc 
Systemcc 
.cc %
InvalidOperationExceptioncc 6
(cc6 7
$strcc7 U
)ccU V
;ccV W
}dd 	
EndTimeff 
=ff 
DateTimeff 
.ff 
UtcNowff !
;ff! "
Durationgg 
=gg 
EndTimegg 
-gg 
	StartTimegg &
;gg& '
TotalChargehh 
=hh 
totalChargehh !
;hh! "
Winnerii 
=ii 
winnerii 
?ii 
.ii 
Trimii 
(ii 
)ii 
;ii  
}jj 
publicrr 

voidrr 
AddGameDatarr 
(rr 
stringrr "
keyrr# &
,rr& '
objectrr( .
valuerr/ 4
)rr4 5
{ss 
iftt 

(tt 
stringtt 
.tt 
IsNullOrWhiteSpacett %
(tt% &
keytt& )
)tt) *
)tt* +
{uu 	
throwvv 
newvv 
ArgumentExceptionvv '
(vv' (
$strvv( H
,vvH I
nameofvvJ P
(vvP Q
keyvvQ T
)vvT U
)vvU V
;vvV W
}ww 	
GameDatayy 
[yy 
keyyy 
.yy 
Trimyy 
(yy 
)yy 
]yy 
=yy 
valueyy $
;yy$ %
}zz 
public
ÅÅ 

object
ÅÅ 
?
ÅÅ 
GetGameData
ÅÅ 
(
ÅÅ 
string
ÅÅ %
key
ÅÅ& )
)
ÅÅ) *
{
ÇÇ 
if
ÉÉ 

(
ÉÉ 
string
ÉÉ 
.
ÉÉ  
IsNullOrWhiteSpace
ÉÉ %
(
ÉÉ% &
key
ÉÉ& )
)
ÉÉ) *
)
ÉÉ* +
{
ÑÑ 	
return
ÖÖ 
null
ÖÖ 
;
ÖÖ 
}
ÜÜ 	
return
àà 
GameData
àà 
.
àà 
TryGetValue
àà #
(
àà# $
key
àà$ '
.
àà' (
Trim
àà( ,
(
àà, -
)
àà- .
,
àà. /
out
àà0 3
var
àà4 7
value
àà8 =
)
àà= >
?
àà? @
value
ààA F
:
ààG H
null
ààI M
;
ààM N
}
ââ 
public
èè 

Money
èè 
GetRevenuePerHour
èè "
(
èè" #
)
èè# $
{
êê 
if
ëë 

(
ëë 
Duration
ëë 
.
ëë 

TotalHours
ëë 
<=
ëë  "
$num
ëë# $
)
ëë$ %
{
íí 	
return
ìì 
Money
ìì 
.
ìì 
Zero
ìì 
(
ìì 
)
ìì 
;
ìì  
}
îî 	
var
ññ 
hoursDecimal
ññ 
=
ññ 
(
ññ 
decimal
ññ #
)
ññ# $
Duration
ññ$ ,
.
ññ, -

TotalHours
ññ- 7
;
ññ7 8
return
óó 
TotalCharge
óó 
/
óó 
hoursDecimal
óó )
;
óó) *
}
òò 
public
ûû 

Money
ûû !
GetRevenuePerPlayer
ûû $
(
ûû$ %
)
ûû% &
{
üü 
if
†† 

(
†† 
PlayerCount
†† 
<=
†† 
$num
†† 
)
†† 
{
°° 	
return
¢¢ 
Money
¢¢ 
.
¢¢ 
Zero
¢¢ 
(
¢¢ 
)
¢¢ 
;
¢¢  
}
££ 	
return
•• 
TotalCharge
•• 
/
•• 
PlayerCount
•• (
;
••( )
}
¶¶ 
}ßß Â
tC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\FractionalModifier.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
FractionalModifier 
:  !
MenuModifier" .
{ 
public		 

ModifierPortion		 
Portion		 "
{		# $
get		% (
;		( )
private		* 1
set		2 5
;		5 6
}		7 8
public

 

PriceStrategy

 
PriceStrategy

 &
{

' (
get

) ,
;

, -
private

. 5
set

6 9
;

9 :
}

; <
	protected 
FractionalModifier  
(  !
)! "
:# $
base% )
() *
)* +
{, -
}. /
public 

FractionalModifier 
( 
string 
name 
, 
Money 
price 
, 
int 
	sortOrder 
, 
ModifierPortion 
portion 
,  
PriceStrategy 
priceStrategy #
)# $
: 	
base
 
( 
name 
, 
price 
, 
	sortOrder %
)% &
{ 
Portion 
= 
portion 
; 
PriceStrategy 
= 
priceStrategy %
;% &
} 
} åf
gC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Floor.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
Floor 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

string 
Description 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
string6 <
.< =
Empty= B
;B C
public 

int 
Width 
{ 
get 
; 
private #
set$ '
;' (
}) *
=+ ,
$num- 1
;1 2
public 

int 
Height 
{ 
get 
; 
private $
set% (
;( )
}* +
=, -
$num. 2
;2 3
public 

string 
BackgroundColor !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
=8 9
$str: C
;C D
public 

List 
< 
TableLayout 
> 
TableLayouts )
{* +
get, /
;/ 0
private1 8
set9 <
;< =
}> ?
=@ A
newB E
(E F
)F G
;G H
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
=/ 0
true1 5
;5 6
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

int 
Version 
{ 
get 
; 
private %
set& )
;) *
}+ ,
private 
Floor 
( 
) 
{ 
} 
public"" 

static"" 
Floor"" 
Create"" 
("" 
string"" %
name""& *
,""* +
string"", 2
description""3 >
=""? @
$str""A C
,""C D
int""E H
width""I N
=""O P
$num""Q U
,""U V
int""W Z
height""[ a
=""b c
$num""d h
)""h i
{## 
if$$ 

($$ 
string$$ 
.$$ 
IsNullOrWhiteSpace$$ %
($$% &
name$$& *
)$$* +
)$$+ ,
{%% 	
throw&& 
new&& 

Exceptions&&  
.&&  !*
BusinessRuleViolationException&&! ?
(&&? @
$str&&@ ]
)&&] ^
;&&^ _
}'' 	
if)) 

()) 
width)) 
<=)) 
$num)) 
||)) 
height))  
<=))! #
$num))$ %
)))% &
{** 	
throw++ 
new++ 

Exceptions++  
.++  !*
BusinessRuleViolationException++! ?
(++? @
$str++@ m
)++m n
;++n o
},, 	
return.. 
new.. 
Floor.. 
{// 	
Id00 
=00 
Guid00 
.00 
NewGuid00 
(00 
)00 
,00  
Name11 
=11 
name11 
,11 
Description22 
=22 
description22 %
,22% &
Width33 
=33 
width33 
,33 
Height44 
=44 
height44 
,44 
	CreatedAt55 
=55 
DateTime55  
.55  !
UtcNow55! '
,55' (
	UpdatedAt66 
=66 
DateTime66  
.66  !
UtcNow66! '
,66' (
Version77 
=77 
$num77 
}88 	
;88	 

}99 
public>> 

void>> 

UpdateName>> 
(>> 
string>> !
name>>" &
)>>& '
{?? 
if@@ 

(@@ 
string@@ 
.@@ 
IsNullOrWhiteSpace@@ %
(@@% &
name@@& *
)@@* +
)@@+ ,
{AA 	
throwBB 
newBB 

ExceptionsBB  
.BB  !*
BusinessRuleViolationExceptionBB! ?
(BB? @
$strBB@ ]
)BB] ^
;BB^ _
}CC 	
NameEE 
=EE 
nameEE 
;EE 
	UpdatedAtFF 
=FF 
DateTimeFF 
.FF 
UtcNowFF #
;FF# $
VersionGG 
++GG 
;GG 
}HH 
publicMM 

voidMM 
UpdateDescriptionMM !
(MM! "
stringMM" (
descriptionMM) 4
)MM4 5
{NN 
DescriptionOO 
=OO 
descriptionOO !
??OO" $
stringOO% +
.OO+ ,
EmptyOO, 1
;OO1 2
	UpdatedAtPP 
=PP 
DateTimePP 
.PP 
UtcNowPP #
;PP# $
VersionQQ 
++QQ 
;QQ 
}RR 
publicWW 

voidWW 
UpdateDimensionsWW  
(WW  !
intWW! $
widthWW% *
,WW* +
intWW, /
heightWW0 6
)WW6 7
{XX 
ifYY 

(YY 
widthYY 
<=YY 
$numYY 
||YY 
heightYY  
<=YY! #
$numYY$ %
)YY% &
{ZZ 	
throw[[ 
new[[ 

Exceptions[[  
.[[  !*
BusinessRuleViolationException[[! ?
([[? @
$str[[@ m
)[[m n
;[[n o
}\\ 	
Width^^ 
=^^ 
width^^ 
;^^ 
Height__ 
=__ 
height__ 
;__ 
	UpdatedAt`` 
=`` 
DateTime`` 
.`` 
UtcNow`` #
;``# $
Versionaa 
++aa 
;aa 
}bb 
publicgg 

voidgg !
UpdateBackgroundColorgg %
(gg% &
stringgg& ,
backgroundColorgg- <
)gg< =
{hh 
BackgroundColorii 
=ii 
backgroundColorii )
??ii* ,
$strii- 6
;ii6 7
	UpdatedAtjj 
=jj 
DateTimejj 
.jj 
UtcNowjj #
;jj# $
Versionkk 
++kk 
;kk 
}ll 
publicqq 

voidqq 
	AddLayoutqq 
(qq 
TableLayoutqq %
layoutqq& ,
)qq, -
{rr 
ifss 

(ss 
layoutss 
==ss 
nullss 
)ss 
{tt 	
throwuu 
newuu !
ArgumentNullExceptionuu +
(uu+ ,
nameofuu, 2
(uu2 3
layoutuu3 9
)uu9 :
)uu: ;
;uu; <
}vv 	
ifyy 

(yy 
TableLayoutsyy 
.yy 
Anyyy 
(yy 
lyy 
=>yy !
lyy" #
.yy# $
Nameyy$ (
.yy( )
Equalsyy) /
(yy/ 0
layoutyy0 6
.yy6 7
Nameyy7 ;
,yy; <
StringComparisonyy= M
.yyM N
OrdinalIgnoreCaseyyN _
)yy_ `
)yy` a
)yya b
{zz 	
throw{{ 
new{{ 

Exceptions{{  
.{{  !*
BusinessRuleViolationException{{! ?
({{? @
$"{{@ B
$str{{B O
{{{O P
layout{{P V
.{{V W
Name{{W [
}{{[ \
$str{{\ {
"{{{ |
){{| }
;{{} ~
}|| 	
TableLayouts~~ 
.~~ 
Add~~ 
(~~ 
layout~~ 
)~~  
;~~  !
	UpdatedAt 
= 
DateTime 
. 
UtcNow #
;# $
Version
ÄÄ 
++
ÄÄ 
;
ÄÄ 
}
ÅÅ 
public
ÜÜ 

void
ÜÜ 
RemoveLayout
ÜÜ 
(
ÜÜ 
Guid
ÜÜ !
layoutId
ÜÜ" *
)
ÜÜ* +
{
áá 
var
àà 
layout
àà 
=
àà 
TableLayouts
àà !
.
àà! "
FirstOrDefault
àà" 0
(
àà0 1
l
àà1 2
=>
àà3 5
l
àà6 7
.
àà7 8
Id
àà8 :
==
àà; =
layoutId
àà> F
)
ààF G
;
ààG H
if
ââ 

(
ââ 
layout
ââ 
!=
ââ 
null
ââ 
)
ââ 
{
ää 	
TableLayouts
ãã 
.
ãã 
Remove
ãã 
(
ãã  
layout
ãã  &
)
ãã& '
;
ãã' (
	UpdatedAt
åå 
=
åå 
DateTime
åå  
.
åå  !
UtcNow
åå! '
;
åå' (
Version
çç 
++
çç 
;
çç 
}
éé 	
}
èè 
public
îî 

TableLayout
îî 
?
îî 
GetActiveLayout
îî '
(
îî' (
)
îî( )
{
ïï 
return
ññ 
TableLayouts
ññ 
.
ññ 
FirstOrDefault
ññ *
(
ññ* +
l
ññ+ ,
=>
ññ- /
l
ññ0 1
.
ññ1 2
IsActive
ññ2 :
&&
ññ; =
!
ññ> ?
l
ññ? @
.
ññ@ A
IsDraft
ññA H
)
ññH I
;
ññI J
}
óó 
public
úú 

IReadOnlyList
úú 
<
úú 
Table
úú 
>
úú 
GetAllTables
úú  ,
(
úú, -
)
úú- .
{
ùù 
return
ûû 
TableLayouts
ûû 
.
ûû 

SelectMany
ûû &
(
ûû& '
l
ûû' (
=>
ûû) +
l
ûû, -
.
ûû- .
Tables
ûû. 4
)
ûû4 5
.
ûû5 6
ToList
ûû6 <
(
ûû< =
)
ûû= >
.
ûû> ?

AsReadOnly
ûû? I
(
ûûI J
)
ûûJ K
;
ûûK L
}
üü 
public
§§ 

void
§§ 
Activate
§§ 
(
§§ 
)
§§ 
{
•• 
IsActive
¶¶ 
=
¶¶ 
true
¶¶ 
;
¶¶ 
	UpdatedAt
ßß 
=
ßß 
DateTime
ßß 
.
ßß 
UtcNow
ßß #
;
ßß# $
Version
®® 
++
®® 
;
®® 
}
©© 
public
ÆÆ 

void
ÆÆ 

Deactivate
ÆÆ 
(
ÆÆ 
)
ÆÆ 
{
ØØ 
if
±± 

(
±± 
TableLayouts
±± 
.
±± 
Any
±± 
(
±± 
l
±± 
=>
±± !
l
±±" #
.
±±# $
Tables
±±$ *
.
±±* +
Any
±±+ .
(
±±. /
t
±±/ 0
=>
±±1 3
t
±±4 5
.
±±5 6
Status
±±6 <
==
±±= ?
TableStatus
±±@ K
.
±±K L
Seat
±±L P
)
±±P Q
)
±±Q R
)
±±R S
{
≤≤ 	
throw
≥≥ 
new
≥≥ 

Exceptions
≥≥  
.
≥≥  !'
InvalidOperationException
≥≥! :
(
≥≥: ;
$str
≥≥; h
)
≥≥h i
;
≥≥i j
}
¥¥ 	
IsActive
∂∂ 
=
∂∂ 
false
∂∂ 
;
∂∂ 
	UpdatedAt
∑∑ 
=
∑∑ 
DateTime
∑∑ 
.
∑∑ 
UtcNow
∑∑ #
;
∑∑# $
Version
∏∏ 
++
∏∏ 
;
∏∏ 
}
ππ 
public
ææ 

bool
ææ 
ContainsPoint
ææ 
(
ææ 
int
ææ !
x
ææ" #
,
ææ# $
int
ææ% (
y
ææ) *
)
ææ* +
{
øø 
return
¿¿ 
x
¿¿ 
>=
¿¿ 
$num
¿¿ 
&&
¿¿ 
y
¿¿ 
>=
¿¿ 
$num
¿¿ 
&&
¿¿  "
x
¿¿# $
<
¿¿% &
Width
¿¿' ,
&&
¿¿- /
y
¿¿0 1
<
¿¿2 3
Height
¿¿4 :
;
¿¿: ;
}
¡¡ 
}¬¬ Ìj
kC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Equipment.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
	Equipment 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

string 
Description 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
string6 <
.< =
Empty= B
;B C
public 

EquipmentType 
Type 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

EquipmentStatus 
Status !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 

Guid 
? 
AssignedTableId  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
public 

DateTime 
? 
LastMaintenanceDate (
{) *
get+ .
;. /
private0 7
set8 ;
;; <
}= >
public 

DateTime 
? 
NextMaintenanceDate (
{) *
get+ .
;. /
private0 7
set8 ;
;; <
}= >
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
private 
	Equipment 
( 
) 
{ 
} 
public&& 

static&& 
	Equipment&& 
Create&& "
(&&" #
string&&# )
name&&* .
,&&. /
EquipmentType&&0 =
type&&> B
,&&B C
string&&D J
description&&K V
=&&W X
$str&&Y [
)&&[ \
{'' 
if(( 

((( 
string(( 
.(( 
IsNullOrWhiteSpace(( %
(((% &
name((& *
)((* +
)((+ ,
{)) 	
throw** 
new** 
ArgumentException** '
(**' (
$str**( I
,**I J
nameof**K Q
(**Q R
name**R V
)**V W
)**W X
;**X Y
}++ 	
var-- 
now-- 
=-- 
DateTime-- 
.-- 
UtcNow-- !
;--! "
return// 
new// 
	Equipment// 
{00 	
Id11 
=11 
Guid11 
.11 
NewGuid11 
(11 
)11 
,11  
Name22 
=22 
name22 
.22 
Trim22 
(22 
)22 
,22 
Type33 
=33 
type33 
,33 
Description44 
=44 
description44 %
?44% &
.44& '
Trim44' +
(44+ ,
)44, -
??44. 0
string441 7
.447 8
Empty448 =
,44= >
Status55 
=55 
EquipmentStatus55 $
.55$ %
	Available55% .
,55. /
IsActive66 
=66 
true66 
,66 
	CreatedAt77 
=77 
now77 
,77 
	UpdatedAt88 
=88 
now88 
}99 	
;99	 

}:: 
publicAA 

voidAA 
AssignToTableAA 
(AA 
GuidAA "
tableIdAA# *
)AA* +
{BB 
ifCC 

(CC 
tableIdCC 
==CC 
GuidCC 
.CC 
EmptyCC !
)CC! "
{DD 	
throwEE 
newEE 
ArgumentExceptionEE '
(EE' (
$strEE( C
,EEC D
nameofEEE K
(EEK L
tableIdEEL S
)EES T
)EET U
;EEU V
}FF 	
ifHH 

(HH 
StatusHH 
!=HH 
EquipmentStatusHH %
.HH% &
	AvailableHH& /
)HH/ 0
{II 	
throwJJ 
newJJ *
BusinessRuleViolationExceptionJJ 4
(JJ4 5
$strJJ5 e
)JJe f
;JJf g
}KK 	
ifMM 

(MM 
!MM 
IsActiveMM 
)MM 
{NN 	
throwOO 
newOO *
BusinessRuleViolationExceptionOO 4
(OO4 5
$strOO5 `
)OO` a
;OOa b
}PP 	
AssignedTableIdRR 
=RR 
tableIdRR !
;RR! "
StatusSS 
=SS 
EquipmentStatusSS  
.SS  !
InUseSS! &
;SS& '
	UpdatedAtTT 
=TT 
DateTimeTT 
.TT 
UtcNowTT #
;TT# $
}UU 
public[[ 

void[[ 
UnassignFromTable[[ !
([[! "
)[[" #
{\\ 
if]] 

(]] 
!]] 
AssignedTableId]] 
.]] 
HasValue]] %
)]]% &
{^^ 	
throw__ 
new__ *
BusinessRuleViolationException__ 4
(__4 5
$str__5 e
)__e f
;__f g
}`` 	
AssignedTableIdbb 
=bb 
nullbb 
;bb 
Statuscc 
=cc 
EquipmentStatuscc  
.cc  !
	Availablecc! *
;cc* +
	UpdatedAtdd 
=dd 
DateTimedd 
.dd 
UtcNowdd #
;dd# $
}ee 
publicll 

voidll 
ScheduleMaintenancell #
(ll# $
DateTimell$ ,
maintenanceDatell- <
)ll< =
{mm 
ifnn 

(nn 
maintenanceDatenn 
<=nn 
DateTimenn '
.nn' (
UtcNownn( .
)nn. /
{oo 	
throwpp 
newpp 
ArgumentExceptionpp '
(pp' (
$strpp( Q
,ppQ R
nameofppS Y
(ppY Z
maintenanceDateppZ i
)ppi j
)ppj k
;ppk l
}qq 	
NextMaintenanceDatess 
=ss 
maintenanceDatess -
;ss- .
ifvv 

(vv 
maintenanceDatevv 
<=vv 
DateTimevv '
.vv' (
UtcNowvv( .
.vv. /
AddDaysvv/ 6
(vv6 7
$numvv7 8
)vv8 9
)vv9 :
{ww 	
Statusxx 
=xx 
EquipmentStatusxx $
.xx$ %
MaintenanceRequiredxx% 8
;xx8 9
if{{ 
({{ 
AssignedTableId{{ 
.{{  
HasValue{{  (
){{( )
{|| 
AssignedTableId}} 
=}}  !
null}}" &
;}}& '
}~~ 
} 	
	UpdatedAt
ÅÅ 
=
ÅÅ 
DateTime
ÅÅ 
.
ÅÅ 
UtcNow
ÅÅ #
;
ÅÅ# $
}
ÇÇ 
public
áá 

void
áá !
CompleteMaintenance
áá #
(
áá# $
)
áá$ %
{
àà !
LastMaintenanceDate
ââ 
=
ââ 
DateTime
ââ &
.
ââ& '
UtcNow
ââ' -
;
ââ- .!
NextMaintenanceDate
ää 
=
ää 
null
ää "
;
ää" #
if
çç 

(
çç 
Status
çç 
==
çç 
EquipmentStatus
çç %
.
çç% &!
MaintenanceRequired
çç& 9
)
çç9 :
{
éé 	
Status
èè 
=
èè 
EquipmentStatus
èè $
.
èè$ %
	Available
èè% .
;
èè. /
}
êê 	
	UpdatedAt
íí 
=
íí 
DateTime
íí 
.
íí 
UtcNow
íí #
;
íí# $
}
ìì 
public
ôô 

void
ôô 
TakeOutOfService
ôô  
(
ôô  !
string
ôô! '
reason
ôô( .
=
ôô/ 0
$str
ôô1 3
)
ôô3 4
{
öö 
Status
õõ 
=
õõ 
EquipmentStatus
õõ  
.
õõ  !
OutOfService
õõ! -
;
õõ- .
if
ûû 

(
ûû 
AssignedTableId
ûû 
.
ûû 
HasValue
ûû $
)
ûû$ %
{
üü 	
AssignedTableId
†† 
=
†† 
null
†† "
;
††" #
}
°° 	
	UpdatedAt
££ 
=
££ 
DateTime
££ 
.
££ 
UtcNow
££ #
;
££# $
}
§§ 
public
©© 

void
©© 
ReturnToService
©© 
(
©©  
)
©©  !
{
™™ 
if
´´ 

(
´´ 
Status
´´ 
==
´´ 
EquipmentStatus
´´ %
.
´´% &
OutOfService
´´& 2
)
´´2 3
{
¨¨ 	
Status
≠≠ 
=
≠≠ 
EquipmentStatus
≠≠ $
.
≠≠$ %
	Available
≠≠% .
;
≠≠. /
	UpdatedAt
ÆÆ 
=
ÆÆ 
DateTime
ÆÆ  
.
ÆÆ  !
UtcNow
ÆÆ! '
;
ÆÆ' (
}
ØØ 	
}
∞∞ 
public
µµ 

void
µµ 
MarkAsMissing
µµ 
(
µµ 
)
µµ 
{
∂∂ 
Status
∑∑ 
=
∑∑ 
EquipmentStatus
∑∑  
.
∑∑  !
Missing
∑∑! (
;
∑∑( )
if
∫∫ 

(
∫∫ 
AssignedTableId
∫∫ 
.
∫∫ 
HasValue
∫∫ $
)
∫∫$ %
{
ªª 	
AssignedTableId
ºº 
=
ºº 
null
ºº "
;
ºº" #
}
ΩΩ 	
	UpdatedAt
øø 
=
øø 
DateTime
øø 
.
øø 
UtcNow
øø #
;
øø# $
}
¿¿ 
public
≈≈ 

void
≈≈ 
MarkAsFound
≈≈ 
(
≈≈ 
)
≈≈ 
{
∆∆ 
if
«« 

(
«« 
Status
«« 
==
«« 
EquipmentStatus
«« %
.
««% &
Missing
««& -
)
««- .
{
»» 	
Status
…… 
=
…… 
EquipmentStatus
…… $
.
……$ %
	Available
……% .
;
……. /
	UpdatedAt
   
=
   
DateTime
    
.
    !
UtcNow
  ! '
;
  ' (
}
ÀÀ 	
}
ÃÃ 
public
—— 

void
—— 

Deactivate
—— 
(
—— 
)
—— 
{
““ 
IsActive
”” 
=
”” 
false
”” 
;
”” 
if
÷÷ 

(
÷÷ 
AssignedTableId
÷÷ 
.
÷÷ 
HasValue
÷÷ $
)
÷÷$ %
{
◊◊ 	
AssignedTableId
ÿÿ 
=
ÿÿ 
null
ÿÿ "
;
ÿÿ" #
Status
ŸŸ 
=
ŸŸ 
EquipmentStatus
ŸŸ $
.
ŸŸ$ %
	Available
ŸŸ% .
;
ŸŸ. /
}
⁄⁄ 	
	UpdatedAt
‹‹ 
=
‹‹ 
DateTime
‹‹ 
.
‹‹ 
UtcNow
‹‹ #
;
‹‹# $
}
›› 
public
‚‚ 

void
‚‚ 
Activate
‚‚ 
(
‚‚ 
)
‚‚ 
{
„„ 
IsActive
‰‰ 
=
‰‰ 
true
‰‰ 
;
‰‰ 
	UpdatedAt
ÂÂ 
=
ÂÂ 
DateTime
ÂÂ 
.
ÂÂ 
UtcNow
ÂÂ #
;
ÂÂ# $
}
ÊÊ 
public
ÓÓ 

void
ÓÓ 
UpdateDetails
ÓÓ 
(
ÓÓ 
string
ÓÓ $
name
ÓÓ% )
,
ÓÓ) *
string
ÓÓ+ 1
description
ÓÓ2 =
=
ÓÓ> ?
$str
ÓÓ@ B
)
ÓÓB C
{
ÔÔ 
if
 

(
 
string
 
.
  
IsNullOrWhiteSpace
 %
(
% &
name
& *
)
* +
)
+ ,
{
ÒÒ 	
throw
ÚÚ 
new
ÚÚ 
ArgumentException
ÚÚ '
(
ÚÚ' (
$str
ÚÚ( I
,
ÚÚI J
nameof
ÚÚK Q
(
ÚÚQ R
name
ÚÚR V
)
ÚÚV W
)
ÚÚW X
;
ÚÚX Y
}
ÛÛ 	
Name
ıı 
=
ıı 
name
ıı 
.
ıı 
Trim
ıı 
(
ıı 
)
ıı 
;
ıı 
Description
ˆˆ 
=
ˆˆ 
description
ˆˆ !
?
ˆˆ! "
.
ˆˆ" #
Trim
ˆˆ# '
(
ˆˆ' (
)
ˆˆ( )
??
ˆˆ* ,
string
ˆˆ- 3
.
ˆˆ3 4
Empty
ˆˆ4 9
;
ˆˆ9 :
	UpdatedAt
˜˜ 
=
˜˜ 
DateTime
˜˜ 
.
˜˜ 
UtcNow
˜˜ #
;
˜˜# $
}
¯¯ 
}˘˘ ∂
mC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\DrawerBleed.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
DrawerBleed		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 
CashSessionId 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

Money 
Amount 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

string 
? 
Reason 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

UserId 
ProcessedBy 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
null6 :
!: ;
;; <
public 

DateTime 
ProcessedAt 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
private 
DrawerBleed 
( 
) 
{ 
Amount 
= 
Money 
. 
Zero 
( 
) 
; 
} 
public 

static 
DrawerBleed 
Create $
($ %
Guid 
cashSessionId 
, 
Money 
amount 
, 
UserId 
processedBy 
, 
string 
? 
reason 
= 
null 
) 
{ 
if 

( 
amount 
<= 
Money 
. 
Zero  
(  !
)! "
)" #
{ 	
throw 
new 

Exceptions  
.  !*
BusinessRuleViolationException! ?
(? @
$str@ p
)p q
;q r
}   	
return"" 
new"" 
DrawerBleed"" 
{## 	
Id$$ 
=$$ 
Guid$$ 
.$$ 
NewGuid$$ 
($$ 
)$$ 
,$$  
CashSessionId%% 
=%% 
cashSessionId%% )
,%%) *
Amount&& 
=&& 
amount&& 
,&& 
Reason'' 
='' 
reason'' 
,'' 
ProcessedBy(( 
=(( 
processedBy(( %
,((% &
ProcessedAt)) 
=)) 
DateTime)) "
.))" #
UtcNow))# )
}** 	
;**	 

}++ 
},, ÜD
jC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Discount.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
Discount		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

DiscountType 
Type 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

decimal 
Value 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

Money 
? 

MinimumBuy 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

int 
? 
MinimumQuantity 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

QualificationType 
QualificationType .
{/ 0
get1 4
;4 5
private6 =
set> A
;A B
}C D
public 

ApplicationType 
ApplicationType *
{+ ,
get- 0
;0 1
private2 9
set: =
;= >
}? @
public 

bool 
	AutoApply 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

string 
? 

CouponCode 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
? 
ExpirationDate #
{$ %
get& )
;) *
private+ 2
set3 6
;6 7
}8 9
public 

bool !
RequiresAuthorization %
{& '
get( +
;+ ,
private- 4
set5 8
;8 9
}: ;
private 
Discount 
( 
) 
{ 
}   
public%% 

static%% 
Discount%% 
Create%% !
(%%! "
string&& 
name&& 
,&& 
DiscountType'' 
type'' 
,'' 
decimal(( 
value(( 
,(( 
QualificationType)) 
qualificationType)) +
,))+ ,
ApplicationType** 
applicationType** '
,**' (
Money++ 
?++ 

minimumBuy++ 
=++ 
null++  
,++  !
int,, 
?,, 
minimumQuantity,, 
=,, 
null,, #
,,,# $
bool-- 
	autoApply-- 
=-- 
false-- 
,-- 
string.. 
?.. 

couponCode.. 
=.. 
null.. !
,..! "
DateTime// 
?// 
expirationDate//  
=//! "
null//# '
,//' (
bool00 !
requiresAuthorization00 "
=00# $
false00% *
)00* +
{11 
if22 

(22 
string22 
.22 
IsNullOrWhiteSpace22 %
(22% &
name22& *
)22* +
)22+ ,
{33 	
throw44 
new44 
ArgumentException44 '
(44' (
$str44( P
,44P Q
nameof44R X
(44X Y
name44Y ]
)44] ^
)44^ _
;44_ `
}55 	
if77 

(77 
value77 
<77 
$num77 
)77 
{88 	
throw99 
new99 

Exceptions99  
.99  !*
BusinessRuleViolationException99! ?
(99? @
$str99@ d
)99d e
;99e f
}:: 	
if== 

(== 
type== 
==== 
DiscountType==  
.==  !

Percentage==! +
&&==, .
(==/ 0
value==0 5
<==6 7
$num==8 9
||==: <
value=== B
>==C D
$num==E H
)==H I
)==I J
{>> 	
throw?? 
new?? 

Exceptions??  
.??  !*
BusinessRuleViolationException??! ?
(??? @
$str??@ v
)??v w
;??w x
}@@ 	
returnBB 
newBB 
DiscountBB 
{CC 	
IdDD 
=DD 
GuidDD 
.DD 
NewGuidDD 
(DD 
)DD 
,DD  
NameEE 
=EE 
nameEE 
,EE 
TypeFF 
=FF 
typeFF 
,FF 
ValueGG 
=GG 
valueGG 
,GG 

MinimumBuyHH 
=HH 

minimumBuyHH #
,HH# $
MinimumQuantityII 
=II 
minimumQuantityII -
,II- .
QualificationTypeJJ 
=JJ 
qualificationTypeJJ  1
,JJ1 2
ApplicationTypeKK 
=KK 
applicationTypeKK -
,KK- .
	AutoApplyLL 
=LL 
	autoApplyLL !
,LL! "

CouponCodeMM 
=MM 

couponCodeMM #
,MM# $
ExpirationDateNN 
=NN 
expirationDateNN +
,NN+ ,
IsActiveOO 
=OO 
trueOO 
,OO !
RequiresAuthorizationPP !
=PP" #!
requiresAuthorizationPP$ 9
}QQ 	
;QQ	 

}RR 
publicTT 

voidTT 

DeactivateTT 
(TT 
)TT 
{UU 
IsActiveVV 
=VV 
falseVV 
;VV 
}WW 
publicYY 

voidYY 
ActivateYY 
(YY 
)YY 
{ZZ 
IsActive[[ 
=[[ 
true[[ 
;[[ 
}\\ 
publiccc 

Moneycc 
CalculateDiscountcc "
(cc" #
Moneycc# (
amountcc) /
)cc/ 0
{dd 
ifee 

(ee 
amountee 
==ee 
nullee 
||ee 
amountee $
.ee$ %
Amountee% +
<=ee, .
$numee/ 0
)ee0 1
{ff 	
returngg 
Moneygg 
.gg 
Zerogg 
(gg 
)gg 
;gg  
}hh 	
returnjj 
Typejj 
switchjj 
{kk 	
DiscountTypell 
.ll 

Percentagell #
=>ll$ &
newll' *
Moneyll+ 0
(ll0 1
amountll1 7
.ll7 8
Amountll8 >
*ll? @
(llA B
ValuellB G
/llH I
$numllJ N
)llN O
,llO P
amountllQ W
.llW X
CurrencyllX `
)ll` a
,lla b
DiscountTypemm 
.mm 
FixedAmountmm $
=>mm% '
newmm( +
Moneymm, 1
(mm1 2
Mathmm2 6
.mm6 7
Minmm7 :
(mm: ;
Valuemm; @
,mm@ A
amountmmB H
.mmH I
AmountmmI O
)mmO P
,mmP Q
amountmmR X
.mmX Y
CurrencymmY a
)mma b
,mmb c
DiscountTypenn 
.nn 
Amountnn 
=>nn  "
newnn# &
Moneynn' ,
(nn, -
Mathnn- 1
.nn1 2
Minnn2 5
(nn5 6
Valuenn6 ;
,nn; <
amountnn= C
.nnC D
AmountnnD J
)nnJ K
,nnK L
amountnnM S
.nnS T
CurrencynnT \
)nn\ ]
,nn] ^
_oo 
=>oo 
Moneyoo 
.oo 
Zerooo 
(oo 
)oo 
}pp 	
;pp	 

}qq 
}rr …D
rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\DebitCardPayment.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
DebitCardPayment		 
:		 
Payment		  '
{

 
public 

string 
? 

CardNumber 
{ 
get  #
;# $
	protected% .
set/ 2
;2 3
}4 5
public 

string 
? 
CardHolderName !
{" #
get$ '
;' (
	protected) 2
set3 6
;6 7
}8 9
public 

string 
? 
AuthorizationCode $
{% &
get' *
;* +
	protected, 5
set6 9
;9 :
}; <
public 

string 
? 
ReferenceNumber "
{# $
get% (
;( )
	protected* 3
set4 7
;7 8
}9 :
public 

string 
? 
CardType 
{ 
get !
;! "
	protected# ,
set- 0
;0 1
}2 3
public 

DateTime 
? 
AuthorizationTime &
{' (
get) ,
;, -
	protected. 7
set8 ;
;; <
}= >
public 

DateTime 
? 
CaptureTime  
{! "
get# &
;& '
	protected( 1
set2 5
;5 6
}7 8
public 

string 
? 
	PinNumber 
{ 
get "
;" #
	protected$ -
set. 1
;1 2
}3 4
	protected 
DebitCardPayment 
( 
)  
{ 
} 
	protected 
DebitCardPayment 
( 
Guid 
ticketId 
, 
Money 
amount 
, 
UserId 
processedBy 
, 
Guid 

terminalId 
, 
string 
? 

cardNumber 
= 
null !
,! "
string 
? 
cardHolderName 
=  
null! %
,% &
string 
? 
authorizationCode !
=" #
null$ (
,( )
string   
?   
referenceNumber   
=    !
null  " &
,  & '
string!! 
?!! 
cardType!! 
=!! 
null!! 
,!!  
string"" 
?"" 
	pinNumber"" 
="" 
null""  
,""  !
string## 
?## 
globalId## 
=## 
null## 
,##  
Guid$$ 
?$$ 
splitGroupId$$ 
=$$ 
null$$ !
,$$! "
int%% 
?%% 
splitSequence%% 
=%% 
null%% !
)%%! "
:&& 	
base&&
 
(&& 
ticketId&& 
,&& 
PaymentType&& $
.&&$ %
	DebitCard&&% .
,&&. /
amount&&0 6
,&&6 7
processedBy&&8 C
,&&C D

terminalId&&E O
,&&O P
globalId&&Q Y
,&&Y Z
splitGroupId&&[ g
,&&g h
splitSequence&&i v
)&&v w
{'' 

CardNumber(( 
=(( 

cardNumber(( 
;((  
CardHolderName)) 
=)) 
cardHolderName)) '
;))' (
AuthorizationCode** 
=** 
authorizationCode** -
;**- .
ReferenceNumber++ 
=++ 
referenceNumber++ )
;++) *
CardType,, 
=,, 
cardType,, 
;,, 
	PinNumber-- 
=-- 
	pinNumber-- 
;-- 
IsAuthorizable.. 
=.. 
true.. 
;.. 
}// 
public44 

static44 
DebitCardPayment44 "
Create44# )
(44) *
Guid55 
ticketId55 
,55 
Money66 
amount66 
,66 
UserId77 
processedBy77 
,77 
Guid88 

terminalId88 
,88 
string99 
?99 

cardNumber99 
=99 
null99 !
,99! "
string:: 
?:: 
cardHolderName:: 
=::  
null::! %
,::% &
string;; 
?;; 
authorizationCode;; !
=;;" #
null;;$ (
,;;( )
string<< 
?<< 
referenceNumber<< 
=<<  !
null<<" &
,<<& '
string== 
?== 
cardType== 
=== 
null== 
,==  
string>> 
?>> 
	pinNumber>> 
=>> 
null>>  
,>>  !
string?? 
??? 
globalId?? 
=?? 
null?? 
,??  
Guid@@ 
?@@ 
splitGroupId@@ 
=@@ 
null@@ !
,@@! "
intAA 
?AA 
splitSequenceAA 
=AA 
nullAA !
)AA! "
{BB 
returnCC 
newCC 
DebitCardPaymentCC #
(CC# $
ticketIdDD 
,DD 
amountEE 
,EE 
processedByFF 
,FF 

terminalIdGG 
,GG 

cardNumberHH 
,HH 
cardHolderNameII 
,II 
authorizationCodeJJ 
,JJ 
referenceNumberKK 
,KK 
cardTypeLL 
,LL 
	pinNumberMM 
,MM 
globalIdNN 
,NN 
splitGroupIdOO 
,OO 
splitSequencePP 
)PP 
;PP 
}QQ 
publicVV 

voidVV 
	AuthorizeVV 
(VV 
stringVV  
authorizationCodeVV! 2
,VV2 3
stringVV4 :
?VV: ;
referenceNumberVV< K
=VVL M
nullVVN R
)VVR S
{WW 
ifXX 

(XX 
IsVoidedXX 
)XX 
{YY 	
throwZZ 
newZZ 

ExceptionsZZ  
.ZZ  !%
InvalidOperationExceptionZZ! :
(ZZ: ;
$strZZ; _
)ZZ_ `
;ZZ` a
}[[ 	
if]] 

(]] 

IsCaptured]] 
)]] 
{^^ 	
throw__ 
new__ 

Exceptions__  
.__  !%
InvalidOperationException__! :
(__: ;
$str__; Y
)__Y Z
;__Z [
}`` 	
AuthorizationCodebb 
=bb 
authorizationCodebb -
;bb- .
ReferenceNumbercc 
=cc 
referenceNumbercc )
;cc) *
AuthorizationTimedd 
=dd 
DateTimedd $
.dd$ %
UtcNowdd% +
;dd+ ,
IsAuthorizableee 
=ee 
trueee 
;ee 
}ff 
publickk 

newkk 
voidkk 
Capturekk 
(kk 
)kk 
{ll 
ifmm 

(mm 
!mm 
IsAuthorizablemm 
)mm 
{nn 	
throwoo 
newoo 

Exceptionsoo  
.oo  !%
InvalidOperationExceptionoo! :
(oo: ;
$stroo; k
)ook l
;ool m
}pp 	
ifrr 

(rr 

IsCapturedrr 
)rr 
{ss 	
throwtt 
newtt 

Exceptionstt  
.tt  !%
InvalidOperationExceptiontt! :
(tt: ;
$strtt; Y
)ttY Z
;ttZ [
}uu 	
ifww 

(ww 
stringww 
.ww 
IsNullOrEmptyww  
(ww  !
AuthorizationCodeww! 2
)ww2 3
)ww3 4
{xx 	
throwyy 
newyy 

Exceptionsyy  
.yy  !%
InvalidOperationExceptionyy! :
(yy: ;
$stryy; g
)yyg h
;yyh i
}zz 	

IsCaptured|| 
=|| 
true|| 
;|| 
CaptureTime}} 
=}} 
DateTime}} 
.}} 
UtcNow}} %
;}}% &
}~~ 
}ÅÅ ±1
oC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\CustomPayment.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
CustomPayment		 
:		 
Payment		 $
{

 
public 

string 
PaymentName 
{ 
get  #
;# $
	protected% .
set/ 2
;2 3
}4 5
=6 7
null8 <
!< =
;= >
public 

string 
? 
ReferenceNumber "
{# $
get% (
;( )
	protected* 3
set4 7
;7 8
}9 :
public 


Dictionary 
< 
string 
, 
string $
>$ %

Properties& 0
{1 2
get3 6
;6 7
	protected8 A
setB E
;E F
}G H
=I J
newK N
(N O
)O P
;P Q
	protected 
CustomPayment 
( 
) 
{ 
} 
	protected 
CustomPayment 
( 
Guid 
ticketId 
, 
Money 
amount 
, 
UserId 
processedBy 
, 
Guid 

terminalId 
, 
string 
paymentName 
, 
string 
? 
referenceNumber 
=  !
null" &
,& '

Dictionary 
< 
string 
, 
string !
>! "
?" #

properties$ .
=/ 0
null1 5
,5 6
string 
? 
globalId 
= 
null 
,  
Guid 
? 
splitGroupId 
= 
null !
,! "
int 
? 
splitSequence 
= 
null !
)! "
: 	
base
 
( 
ticketId 
, 
PaymentType $
.$ %
CustomPayment% 2
,2 3
amount4 :
,: ;
processedBy< G
,G H

terminalIdI S
,S T
globalIdU ]
,] ^
splitGroupId_ k
,k l
splitSequencem z
)z {
{ 
if   

(   
string   
.   
IsNullOrWhiteSpace   %
(  % &
paymentName  & 1
)  1 2
)  2 3
{!! 	
throw"" 
new"" 
ArgumentException"" '
(""' (
$str""( O
,""O P
nameof""Q W
(""W X
paymentName""X c
)""c d
)""d e
;""e f
}## 	
PaymentName%% 
=%% 
paymentName%% !
;%%! "
ReferenceNumber&& 
=&& 
referenceNumber&& )
;&&) *

Properties'' 
='' 

properties'' 
??''  "
new''# &

Dictionary''' 1
<''1 2
string''2 8
,''8 9
string'': @
>''@ A
(''A B
)''B C
;''C D
IsAuthorizable(( 
=(( 
false(( 
;(( 
})) 
public.. 

static.. 
CustomPayment.. 
Create..  &
(..& '
Guid// 
ticketId// 
,// 
Money00 
amount00 
,00 
UserId11 
processedBy11 
,11 
Guid22 

terminalId22 
,22 
string33 
paymentName33 
,33 
string44 
?44 
referenceNumber44 
=44  !
null44" &
,44& '

Dictionary55 
<55 
string55 
,55 
string55 !
>55! "
?55" #

properties55$ .
=55/ 0
null551 5
,555 6
string66 
?66 
globalId66 
=66 
null66 
,66  
Guid77 
?77 
splitGroupId77 
=77 
null77 !
,77! "
int88 
?88 
splitSequence88 
=88 
null88 !
)88! "
{99 
return:: 
new:: 
CustomPayment::  
(::  !
ticketId;; 
,;; 
amount<< 
,<< 
processedBy== 
,== 

terminalId>> 
,>> 
paymentName?? 
,?? 
referenceNumber@@ 
,@@ 

propertiesAA 
,AA 
globalIdBB 
,BB 
splitGroupIdCC 
,CC 
splitSequenceDD 
)DD 
;DD 
}EE 
publicJJ 

voidJJ 
SetPropertyJJ 
(JJ 
stringJJ "
keyJJ# &
,JJ& '
stringJJ( .
valueJJ/ 4
)JJ4 5
{KK 
ifLL 

(LL 
stringLL 
.LL 
IsNullOrWhiteSpaceLL %
(LL% &
keyLL& )
)LL) *
)LL* +
{MM 	
throwNN 
newNN 
ArgumentExceptionNN '
(NN' (
$strNN( O
,NNO P
nameofNNQ W
(NNW X
keyNNX [
)NN[ \
)NN\ ]
;NN] ^
}OO 	

PropertiesQQ 
[QQ 
keyQQ 
]QQ 
=QQ 
valueQQ 
;QQ  
}RR 
publicWW 

stringWW 
?WW 
GetPropertyWW 
(WW 
stringWW %
keyWW& )
)WW) *
{XX 
returnYY 

PropertiesYY 
.YY 
TryGetValueYY %
(YY% &
keyYY& )
,YY) *
outYY+ .
varYY/ 2
valueYY3 8
)YY8 9
?YY: ;
valueYY< A
:YYB C
nullYYD H
;YYH I
}ZZ 
}[[ ëW
jC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\Customer.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
Customer 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

string 
	FirstName 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
=2 3
string4 :
.: ;
Empty; @
;@ A
public 

string 
LastName 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
=1 2
string3 9
.9 :
Empty: ?
;? @
public 

string 
FullName 
=> 
$"  
{  !
	FirstName! *
}* +
$str+ ,
{, -
LastName- 5
}5 6
"6 7
.7 8
Trim8 <
(< =
)= >
;> ?
public 

string 
? 
Email 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

string 
Phone 
{ 
get 
; 
private &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
public 

DateTime 
? 
DateOfBirth  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
public 

string 
? 
Address 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

string 
? 
City 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

string 
? 

PostalCode 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

DateTime 
? 
LastVisitAt  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
public 

int 
TotalVisits 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

Money 

TotalSpent 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
private 
Customer 
( 
) 
{ 

TotalSpent   
=   
Money   
.   
Zero   
(    
)    !
;  ! "
}!! 
public&& 

static&& 
Customer&& 
Create&& !
(&&! "
string&&" (
	firstName&&) 2
,&&2 3
string&&4 :
lastName&&; C
,&&C D
string&&E K
phone&&L Q
,&&Q R
string&&S Y
?&&Y Z
email&&[ `
=&&a b
null&&c g
)&&g h
{'' 
if(( 

((( 
string(( 
.(( 
IsNullOrWhiteSpace(( %
(((% &
	firstName((& /
)((/ 0
)((0 1
throw)) 
new)) *
BusinessRuleViolationException)) 4
())4 5
$str))5 N
)))N O
;))O P
if++ 

(++ 
string++ 
.++ 
IsNullOrWhiteSpace++ %
(++% &
lastName++& .
)++. /
)++/ 0
throw,, 
new,, *
BusinessRuleViolationException,, 4
(,,4 5
$str,,5 M
),,M N
;,,N O
if.. 

(.. 
string.. 
... 
IsNullOrWhiteSpace.. %
(..% &
phone..& +
)..+ ,
).., -
throw// 
new// *
BusinessRuleViolationException// 4
(//4 5
$str//5 P
)//P Q
;//Q R
if22 

(22 
!22 
System22 
.22 
Text22 
.22 
RegularExpressions22 +
.22+ ,
Regex22, 1
.221 2
IsMatch222 9
(229 :
phone22: ?
,22? @
$str22A V
)22V W
)22W X
throw33 
new33 *
BusinessRuleViolationException33 4
(334 5
$str335 S
)33S T
;33T U
if55 

(55 
!55 
string55 
.55 
IsNullOrWhiteSpace55 &
(55& '
email55' ,
)55, -
&&55. 0
!551 2
email552 7
.557 8
Contains558 @
(55@ A
$str55A D
)55D E
)55E F
throw66 
new66 *
BusinessRuleViolationException66 4
(664 5
$str665 L
)66L M
;66M N
return88 
new88 
Customer88 
{99 	
Id:: 
=:: 
Guid:: 
.:: 
NewGuid:: 
(:: 
):: 
,::  
	FirstName;; 
=;; 
	firstName;; !
,;;! "
LastName<< 
=<< 
lastName<< 
,<<  
Phone== 
=== 
phone== 
,== 
Email>> 
=>> 
email>> 
,>> 
	CreatedAt?? 
=?? 
DateTime??  
.??  !
UtcNow??! '
,??' (
TotalVisits@@ 
=@@ 
$num@@ 
,@@ 

TotalSpentAA 
=AA 
MoneyAA 
.AA 
ZeroAA #
(AA# $
)AA$ %
,AA% &
IsActiveBB 
=BB 
trueBB 
}CC 	
;CC	 

}DD 
publicII 

voidII 
UpdateContactInfoII !
(II! "
stringII" (
?II( )
emailII* /
,II/ 0
stringII1 7
phoneII8 =
,II= >
stringII? E
?IIE F
addressIIG N
,IIN O
stringIIP V
?IIV W
cityIIX \
=II] ^
nullII_ c
,IIc d
stringIIe k
?IIk l

postalCodeIIm w
=IIx y
nullIIz ~
)II~ 
{JJ 
ifKK 

(KK 
stringKK 
.KK 
IsNullOrWhiteSpaceKK %
(KK% &
phoneKK& +
)KK+ ,
)KK, -
throwLL 
newLL *
BusinessRuleViolationExceptionLL 4
(LL4 5
$strLL5 P
)LLP Q
;LLQ R
ifNN 

(NN 
!NN 
SystemNN 
.NN 
TextNN 
.NN 
RegularExpressionsNN +
.NN+ ,
RegexNN, 1
.NN1 2
IsMatchNN2 9
(NN9 :
phoneNN: ?
,NN? @
$strNNA V
)NNV W
)NNW X
throwOO 
newOO *
BusinessRuleViolationExceptionOO 4
(OO4 5
$strOO5 S
)OOS T
;OOT U
ifQQ 

(QQ 
!QQ 
stringQQ 
.QQ 
IsNullOrWhiteSpaceQQ &
(QQ& '
emailQQ' ,
)QQ, -
&&QQ. 0
!QQ1 2
emailQQ2 7
.QQ7 8
ContainsQQ8 @
(QQ@ A
$strQQA D
)QQD E
)QQE F
throwRR 
newRR *
BusinessRuleViolationExceptionRR 4
(RR4 5
$strRR5 L
)RRL M
;RRM N
PhoneTT 
=TT 
phoneTT 
;TT 
EmailUU 
=UU 
emailUU 
;UU 
AddressVV 
=VV 
addressVV 
;VV 
CityWW 
=WW 
cityWW 
;WW 

PostalCodeXX 
=XX 

postalCodeXX 
;XX  
}YY 
public^^ 

void^^ 
UpdateDetails^^ 
(^^ 
string^^ $
	firstName^^% .
,^^. /
string^^0 6
lastName^^7 ?
,^^? @
DateTime^^A I
?^^I J
dob^^K N
)^^N O
{__ 
if`` 

(`` 
string`` 
.`` 
IsNullOrWhiteSpace`` %
(``% &
	firstName``& /
)``/ 0
)``0 1
throwaa 
newaa *
BusinessRuleViolationExceptionaa 4
(aa4 5
$straa5 N
)aaN O
;aaO P
ifcc 

(cc 
stringcc 
.cc 
IsNullOrWhiteSpacecc %
(cc% &
lastNamecc& .
)cc. /
)cc/ 0
throwdd 
newdd *
BusinessRuleViolationExceptiondd 4
(dd4 5
$strdd5 M
)ddM N
;ddN O
	FirstNameff 
=ff 
	firstNameff 
;ff 
LastNamegg 
=gg 
lastNamegg 
;gg 
DateOfBirthhh 
=hh 
dobhh 
;hh 
}ii 
publicnn 

voidnn 
RecordVisitnn 
(nn 
DateTimenn $
	visitTimenn% .
,nn. /
Moneynn0 5
spentnn6 ;
)nn; <
{oo 
ifpp 

(pp 
spentpp 
<pp 
Moneypp 
.pp 
Zeropp 
(pp 
)pp  
)pp  !
throwqq 
newqq *
BusinessRuleViolationExceptionqq 4
(qq4 5
$strqq5 W
)qqW X
;qqX Y
TotalVisitsss 
++ss 
;ss 

TotalSpenttt 
+=tt 
spenttt 
;tt 
LastVisitAtuu 
=uu 
	visitTimeuu 
;uu  
}vv 
publicxx 

voidxx 

Deactivatexx 
(xx 
)xx 
{yy 
IsActivezz 
=zz 
falsezz 
;zz 
}{{ 
public}} 

void}} 

Reactivate}} 
(}} 
)}} 
{~~ 
IsActive 
= 
true 
; 
}
ÄÄ 
}ÅÅ ıR
sC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\CreditCardPayment.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
CreditCardPayment		 
:		  
Payment		! (
{

 
public 

string 
? 

CardNumber 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

string 
? 
CardHolderName !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 

string 
? 
AuthorizationCode $
{% &
get' *
;* +
private, 3
set4 7
;7 8
}9 :
public 

string 
? 
ReferenceNumber "
{# $
get% (
;( )
private* 1
set2 5
;5 6
}7 8
public 

string 
? 
CardType 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

DateTime 
? 
AuthorizationTime &
{' (
get) ,
;, -
private. 5
set6 9
;9 :
}; <
public 

DateTime 
? 
CaptureTime  
{! "
get# &
;& '
private( /
set0 3
;3 4
}5 6
	protected 
CreditCardPayment 
(  
)  !
{ 
} 
	protected 
CreditCardPayment 
(  
Guid 
ticketId 
, 
Money 
amount 
, 
UserId 
processedBy 
, 
Guid 

terminalId 
, 
string 
? 

cardNumber 
= 
null !
,! "
string 
? 
cardHolderName 
=  
null! %
,% &
string 
? 
authorizationCode !
=" #
null$ (
,( )
string 
? 
referenceNumber 
=  !
null" &
,& '
string   
?   
cardType   
=   
null   
,    
string!! 
?!! 
globalId!! 
=!! 
null!! 
,!!  
Guid"" 
?"" 
splitGroupId"" 
="" 
null"" !
,""! "
int## 
?## 
splitSequence## 
=## 
null## !
)##! "
:$$ 	
base$$
 
($$ 
ticketId$$ 
,$$ 
PaymentType$$ $
.$$$ %

CreditCard$$% /
,$$/ 0
amount$$1 7
,$$7 8
processedBy$$9 D
,$$D E

terminalId$$F P
,$$P Q
globalId$$R Z
,$$Z [
splitGroupId$$\ h
,$$h i
splitSequence$$j w
)$$w x
{%% 

CardNumber'' 
='' 
MaskCardNumber'' #
(''# $

cardNumber''$ .
)''. /
;''/ 0
CardHolderName(( 
=(( 
cardHolderName(( '
;((' (
AuthorizationCode)) 
=)) 
authorizationCode)) -
;))- .
ReferenceNumber** 
=** 
referenceNumber** )
;**) *
CardType++ 
=++ 
cardType++ 
;++ 
IsAuthorizable,, 
=,, 
true,, 
;,, 
}-- 
public22 

static22 
CreditCardPayment22 #
Create22$ *
(22* +
Guid33 
ticketId33 
,33 
Money44 
amount44 
,44 
UserId55 
processedBy55 
,55 
Guid66 

terminalId66 
,66 
string77 
?77 

cardNumber77 
=77 
null77 !
,77! "
string88 
?88 
cardHolderName88 
=88  
null88! %
,88% &
string99 
?99 
authorizationCode99 !
=99" #
null99$ (
,99( )
string:: 
?:: 
referenceNumber:: 
=::  !
null::" &
,::& '
string;; 
?;; 
cardType;; 
=;; 
null;; 
,;;  
string<< 
?<< 
globalId<< 
=<< 
null<< 
,<<  
Guid== 
?== 
splitGroupId== 
=== 
null== !
,==! "
int>> 
?>> 
splitSequence>> 
=>> 
null>> !
)>>! "
{?? 
return@@ 
new@@ 
CreditCardPayment@@ $
(@@$ %
ticketIdAA 
,AA 
amountBB 
,BB 
processedByCC 
,CC 

terminalIdDD 
,DD 

cardNumberEE 
,EE 
cardHolderNameFF 
,FF 
authorizationCodeGG 
,GG 
referenceNumberHH 
,HH 
cardTypeII 
,II 
globalIdJJ 
,JJ 
splitGroupIdKK 
,KK 
splitSequenceLL 
)LL 
;LL 
}MM 
publicRR 

voidRR 
	AuthorizeRR 
(RR 
stringRR  
authorizationCodeRR! 2
,RR2 3
stringRR4 :
?RR: ;
referenceNumberRR< K
=RRL M
nullRRN R
,RRR S
stringRRT Z
?RRZ [
cardTypeRR\ d
=RRe f
nullRRg k
)RRk l
{SS 
ifTT 

(TT 
IsVoidedTT 
)TT 
{UU 	
throwVV 
newVV 

ExceptionsVV  
.VV  !%
InvalidOperationExceptionVV! :
(VV: ;
$strVV; _
)VV_ `
;VV` a
}WW 	
ifYY 

(YY 

IsCapturedYY 
)YY 
{ZZ 	
throw[[ 
new[[ 

Exceptions[[  
.[[  !%
InvalidOperationException[[! :
([[: ;
$str[[; Y
)[[Y Z
;[[Z [
}\\ 	
AuthorizationCode^^ 
=^^ 
authorizationCode^^ -
;^^- .
if__ 

(__ 
referenceNumber__ 
!=__ 
null__ #
)__# $
{`` 	
ReferenceNumberaa 
=aa 
referenceNumberaa -
;aa- .
}bb 	
ifcc 

(cc 
cardTypecc 
!=cc 
nullcc 
)cc 
{dd 	
CardTypeee 
=ee 
cardTypeee 
;ee  
}ff 	
AuthorizationTimegg 
=gg 
DateTimegg $
.gg$ %
UtcNowgg% +
;gg+ ,
IsAuthorizablehh 
=hh 
truehh 
;hh 
}ii 
internalnn 
voidnn !
UpdateReferenceNumbernn '
(nn' (
stringnn( .
referenceNumbernn/ >
)nn> ?
{oo 
ReferenceNumberpp 
=pp 
referenceNumberpp )
;pp) *
}qq 
publicvv 

newvv 
voidvv 
Capturevv 
(vv 
)vv 
{ww 
ifxx 

(xx 
!xx 
IsAuthorizablexx 
)xx 
{yy 	
throwzz 
newzz 

Exceptionszz  
.zz  !%
InvalidOperationExceptionzz! :
(zz: ;
$strzz; k
)zzk l
;zzl m
}{{ 	
if}} 

(}} 

IsCaptured}} 
)}} 
{~~ 	
throw 
new 

Exceptions  
.  !%
InvalidOperationException! :
(: ;
$str; Y
)Y Z
;Z [
}
ÄÄ 	
if
ÇÇ 

(
ÇÇ 
string
ÇÇ 
.
ÇÇ 
IsNullOrEmpty
ÇÇ  
(
ÇÇ  !
AuthorizationCode
ÇÇ! 2
)
ÇÇ2 3
)
ÇÇ3 4
{
ÉÉ 	
throw
ÑÑ 
new
ÑÑ 

Exceptions
ÑÑ  
.
ÑÑ  !'
InvalidOperationException
ÑÑ! :
(
ÑÑ: ;
$str
ÑÑ; g
)
ÑÑg h
;
ÑÑh i
}
ÖÖ 	

IsCaptured
áá 
=
áá 
true
áá 
;
áá 
CaptureTime
àà 
=
àà 
DateTime
àà 
.
àà 
UtcNow
àà %
;
àà% &
}
ââ 
private
ëë 
static
ëë 
string
ëë 
?
ëë 
MaskCardNumber
ëë )
(
ëë) *
string
ëë* 0
?
ëë0 1

cardNumber
ëë2 <
)
ëë< =
{
íí 
if
ìì 

(
ìì 
string
ìì 
.
ìì  
IsNullOrWhiteSpace
ìì %
(
ìì% &

cardNumber
ìì& 0
)
ìì0 1
)
ìì1 2
return
îî 
null
îî 
;
îî 
if
óó 

(
óó 

cardNumber
óó 
.
óó 
Contains
óó 
(
óó  
$char
óó  #
)
óó# $
)
óó$ %
return
òò 

cardNumber
òò 
;
òò 
if
öö 

(
öö 

cardNumber
öö 
.
öö 
Length
öö 
<=
öö  
$num
öö! "
)
öö" #
return
õõ 

cardNumber
õõ 
;
õõ 
var
ùù 
last4
ùù 
=
ùù 

cardNumber
ùù 
.
ùù 
	Substring
ùù (
(
ùù( )

cardNumber
ùù) 3
.
ùù3 4
Length
ùù4 :
-
ùù; <
$num
ùù= >
)
ùù> ?
;
ùù? @
return
ûû 
new
ûû 
string
ûû 
(
ûû 
$char
ûû 
,
ûû 

cardNumber
ûû )
.
ûû) *
Length
ûû* 0
-
ûû1 2
$num
ûû3 4
)
ûû4 5
+
ûû6 7
last4
ûû8 =
;
ûû= >
}
üü 
}†† ˆ
pC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\ComboGroupItem.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
ComboGroupItem 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public		 

Guid		 
ComboGroupId		 
{		 
get		 "
;		" #
private		$ +
set		, /
;		/ 0
}		1 2
public

 

Guid

 

MenuItemId

 
{

 
get

  
;

  !
private

" )
set

* -
;

- .
}

/ 0
public 

Money 
Upcharge 
{ 
get 
;  
private! (
set) ,
;, -
}. /
	protected 
ComboGroupItem 
( 
) 
{ 
Upcharge 
= 
Money 
. 
Zero 
( 
) 
;  
} 
public 

ComboGroupItem 
( 
Guid 
comboGroupId +
,+ ,
Guid- 1

menuItemId2 <
,< =
Money> C
upchargeD L
)L M
{ 
Id 

= 
Guid 
. 
NewGuid 
( 
) 
; 
ComboGroupId 
= 
comboGroupId #
;# $

MenuItemId 
= 

menuItemId 
;  
Upcharge 
= 
upcharge 
; 
} 
} Ü
lC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\ComboGroup.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 

ComboGroup 
{ 
public		 

Guid		 
Id		 
{		 
get		 
;		 
private		 !
set		" %
;		% &
}		' (
public

 

Guid

 
ComboDefinitionId

 !
{

" #
get

$ '
;

' (
private

) 0
set

1 4
;

4 5
}

6 7
public 

string 
Name 
{ 
get 
; 
private %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

int 
	SortOrder 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
private 
readonly 
List 
< 
ComboGroupItem (
>( )
_items* 0
=1 2
new3 6
(6 7
)7 8
;8 9
public 

IReadOnlyCollection 
< 
ComboGroupItem -
>- .
Items/ 4
=>5 7
_items8 >
.> ?

AsReadOnly? I
(I J
)J K
;K L
	protected 

ComboGroup 
( 
) 
{ 
} 
public 


ComboGroup 
( 
Guid 
comboDefinitionId ,
,, -
string. 4
name5 9
,9 :
int; >
	sortOrder? H
)H I
{ 
Id 

= 
Guid 
. 
NewGuid 
( 
) 
; 
ComboDefinitionId 
= 
comboDefinitionId -
;- .
Name 
= 
name 
; 
	SortOrder 
= 
	sortOrder 
; 
} 
} ≈
qC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\ComboDefinition.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public 
class 
ComboDefinition 
{ 
public		 

Guid		 
Id		 
{		 
get		 
;		 
private		 !
set		" %
;		% &
}		' (
public

 

string

 
Name

 
{

 
get

 
;

 
private

 %
set

& )
;

) *
}

+ ,
=

- .
string

/ 5
.

5 6
Empty

6 ;
;

; <
public 

Money 
Price 
{ 
get 
; 
private %
set& )
;) *
}+ ,
public 

bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
private 
readonly 
List 
< 

ComboGroup $
>$ %
_groups& -
=. /
new0 3
(3 4
)4 5
;5 6
public 

IReadOnlyCollection 
< 

ComboGroup )
>) *
Groups+ 1
=>2 4
_groups5 <
.< =

AsReadOnly= G
(G H
)H I
;I J
	protected 
ComboDefinition 
( 
) 
{ 
Price 
= 
Money 
. 
Zero 
( 
) 
; 
} 
public 

ComboDefinition 
( 
string !
name" &
,& '
Money( -
price. 3
)3 4
{ 
Id 

= 
Guid 
. 
NewGuid 
( 
) 
; 
Name 
= 
name 
; 
Price 
= 
price 
; 
IsActive 
= 
true 
; 
}   
public"" 

void"" 
AddGroup"" 
("" 

ComboGroup"" #
group""$ )
)"") *
{## 
_groups$$ 
.$$ 
Add$$ 
($$ 
group$$ 
)$$ 
;$$ 
}%% 
}&& •ü
mC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\CashSession.cs
	namespace		 	
Magidesk		
 
.		 
Domain		 
.		 
Entities		 "
;		" #
public 
class 
CashSession 
{ 
private 
readonly 
List 
< 
Payment !
>! "
	_payments# ,
=- .
new/ 2
(2 3
)3 4
;4 5
private 
readonly 
List 
< 
Payout  
>  !
_payouts" *
=+ ,
new- 0
(0 1
)1 2
;2 3
private 
readonly 
List 
< 
CashDrop "
>" #

_cashDrops$ .
=/ 0
new1 4
(4 5
)5 6
;6 7
private 
readonly 
List 
< 
DrawerBleed %
>% &
_drawerBleeds' 4
=5 6
new7 :
(: ;
); <
;< =
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

UserId 
UserId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
=/ 0
null1 5
!5 6
;6 7
public 

Guid 

TerminalId 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 

Guid 
ShiftId 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

DateTime 
OpenedAt 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 

DateTime 
? 
ClosedAt 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

UserId 
? 
ClosedBy 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 

Money 
OpeningBalance 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

Money 
ExpectedCash 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

Money 
? 

ActualCash 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public   

Money   
?   

Difference   
{   
get   "
;  " #
private  $ +
set  , /
;  / 0
}  1 2
public!! 

CashSessionStatus!! 
Status!! #
{!!$ %
get!!& )
;!!) *
private!!+ 2
set!!3 6
;!!6 7
}!!8 9
public"" 

int"" 
Version"" 
{"" 
get"" 
;"" 
private"" %
set""& )
;"") *
}""+ ,
public%% 

IReadOnlyCollection%% 
<%% 
Payment%% &
>%%& '
Payments%%( 0
=>%%1 3
	_payments%%4 =
.%%= >

AsReadOnly%%> H
(%%H I
)%%I J
;%%J K
public&& 

IReadOnlyCollection&& 
<&& 
Payout&& %
>&&% &
Payouts&&' .
=>&&/ 1
_payouts&&2 :
.&&: ;

AsReadOnly&&; E
(&&E F
)&&F G
;&&G H
public'' 

IReadOnlyCollection'' 
<'' 
CashDrop'' '
>''' (
	CashDrops'') 2
=>''3 5

_cashDrops''6 @
.''@ A

AsReadOnly''A K
(''K L
)''L M
;''M N
public(( 

IReadOnlyCollection(( 
<(( 
DrawerBleed(( *
>((* +
DrawerBleeds((, 8
=>((9 ;
_drawerBleeds((< I
.((I J

AsReadOnly((J T
(((T U
)((U V
;((V W
private++ 
CashSession++ 
(++ 
)++ 
{,, 
OpeningBalance-- 
=-- 
Money-- 
.-- 
Zero-- #
(--# $
)--$ %
;--% &
ExpectedCash.. 
=.. 
Money.. 
... 
Zero.. !
(..! "
).." #
;..# $
Status// 
=// 
CashSessionStatus// "
.//" #
Open//# '
;//' (
}00 
public55 

static55 
CashSession55 
Open55 "
(55" #
UserId66 
userId66 
,66 
Guid77 

terminalId77 
,77 
Guid88 
shiftId88 
,88 
Money99 
openingBalance99 
)99 
{:: 
if;; 

(;; 
openingBalance;; 
<;; 
Money;; "
.;;" #
Zero;;# '
(;;' (
);;( )
);;) *
{<< 	
throw== 
new== *
BusinessRuleViolationException== 4
(==4 5
$str==5 Z
)==Z [
;==[ \
}>> 	
var@@ 
session@@ 
=@@ 
new@@ 
CashSession@@ %
{AA 	
IdBB 
=BB 
GuidBB 
.BB 
NewGuidBB 
(BB 
)BB 
,BB  
UserIdCC 
=CC 
userIdCC 
,CC 

TerminalIdDD 
=DD 

terminalIdDD #
,DD# $
ShiftIdEE 
=EE 
shiftIdEE 
,EE 
OpenedAtFF 
=FF 
DateTimeFF 
.FF  
UtcNowFF  &
,FF& '
OpeningBalanceGG 
=GG 
openingBalanceGG +
,GG+ ,
StatusHH 
=HH 
CashSessionStatusHH &
.HH& '
OpenHH' +
,HH+ ,
VersionII 
=II 
$numII 
}JJ 	
;JJ	 

sessionLL 
.LL !
CalculateExpectedCashLL %
(LL% &
)LL& '
;LL' (
returnMM 
sessionMM 
;MM 
}NN 
publicSS 

voidSS 
CloseSS 
(SS 
UserIdSS 
closedBySS %
,SS% &
MoneySS' ,

actualCashSS- 7
)SS7 8
{TT 
ifUU 

(UU 
StatusUU 
==UU 
CashSessionStatusUU '
.UU' (
ClosedUU( .
)UU. /
{VV 	
throwWW 
newWW 

ExceptionsWW  
.WW  !%
InvalidOperationExceptionWW! :
(WW: ;
$strWW; \
)WW\ ]
;WW] ^
}XX 	
ifZZ 

(ZZ 

actualCashZZ 
<ZZ 
MoneyZZ 
.ZZ 
ZeroZZ #
(ZZ# $
)ZZ$ %
)ZZ% &
{[[ 	
throw\\ 
new\\ *
BusinessRuleViolationException\\ 4
(\\4 5
$str\\5 V
)\\V W
;\\W X
}]] 	
Status__ 
=__ 
CashSessionStatus__ "
.__" #
Closed__# )
;__) *
ClosedAt`` 
=`` 
DateTime`` 
.`` 
UtcNow`` "
;``" #
ClosedByaa 
=aa 
closedByaa 
;aa 

ActualCashbb 
=bb 

actualCashbb 
;bb  !
CalculateExpectedCashcc 
(cc 
)cc 
;cc  

Differencedd 
=dd 

ActualCashdd 
-dd  !
ExpectedCashdd" .
;dd. /
}ee 
publickk 

voidkk !
CalculateExpectedCashkk %
(kk% &
)kk& '
{ll 
varmm 
cashReceiptsmm 
=mm 
	_paymentsmm $
.nn 
Wherenn 
(nn 
pnn 
=>nn 
pnn 
.nn 
PaymentTypenn %
==nn& (
PaymentTypenn) 4
.nn4 5
Cashnn5 9
&&nn: <
!nn= >
pnn> ?
.nn? @
IsVoidednn@ H
)nnH I
.oo 
	Aggregateoo 
(oo 
Moneyoo 
.oo 
Zerooo !
(oo! "
)oo" #
,oo# $
(oo% &
sumoo& )
,oo) *
poo+ ,
)oo, -
=>oo. 0
sumoo1 4
+oo5 6
poo7 8
.oo8 9
Amountoo9 ?
)oo? @
;oo@ A
varqq 
cashRefundsqq 
=qq 
	_paymentsqq #
.rr 
Whererr 
(rr 
prr 
=>rr 
prr 
.rr 
PaymentTyperr %
==rr& (
PaymentTyperr) 4
.rr4 5
Cashrr5 9
&&rr: <
prr= >
.rr> ?
IsVoidedrr? G
&&rrH J
prrK L
.rrL M
TransactionTyperrM \
==rr] _
TransactionTyperr` o
.rro p
Debitrrp u
)rru v
.ss 
	Aggregatess 
(ss 
Moneyss 
.ss 
Zeross !
(ss! "
)ss" #
,ss# $
(ss% &
sumss& )
,ss) *
pss+ ,
)ss, -
=>ss. 0
sumss1 4
+ss5 6
pss7 8
.ss8 9
Amountss9 ?
)ss? @
;ss@ A
varuu 
payoutsuu 
=uu 
_payoutsuu 
.uu 
	Aggregateuu (
(uu( )
Moneyuu) .
.uu. /
Zerouu/ 3
(uu3 4
)uu4 5
,uu5 6
(uu7 8
sumuu8 ;
,uu; <
puu= >
)uu> ?
=>uu@ B
sumuuC F
+uuG H
puuI J
.uuJ K
AmountuuK Q
)uuQ R
;uuR S
varvv 
	cashDropsvv 
=vv 

_cashDropsvv "
.vv" #
	Aggregatevv# ,
(vv, -
Moneyvv- 2
.vv2 3
Zerovv3 7
(vv7 8
)vv8 9
,vv9 :
(vv; <
sumvv< ?
,vv? @
dvvA B
)vvB C
=>vvD F
sumvvG J
+vvK L
dvvM N
.vvN O
AmountvvO U
)vvU V
;vvV W
varww 
bleedsww 
=ww 
_drawerBleedsww "
.ww" #
	Aggregateww# ,
(ww, -
Moneyww- 2
.ww2 3
Zeroww3 7
(ww7 8
)ww8 9
,ww9 :
(ww; <
sumww< ?
,ww? @
bwwA B
)wwB C
=>wwD F
sumwwG J
+wwK L
bwwM N
.wwN O
AmountwwO U
)wwU V
;wwV W
ExpectedCashyy 
=yy 
OpeningBalanceyy %
+yy& '
cashReceiptsyy( 4
-yy5 6
cashRefundsyy7 B
-yyC D
payoutsyyE L
-yyM N
	cashDropsyyO X
-yyY Z
bleedsyy[ a
;yya b
}zz 
public 

bool 
CanClose 
( 
) 
{
ÄÄ 
return
ÅÅ 
Status
ÅÅ 
==
ÅÅ 
CashSessionStatus
ÅÅ *
.
ÅÅ* +
Open
ÅÅ+ /
;
ÅÅ/ 0
}
ÇÇ 
public
áá 

void
áá 

AddPayment
áá 
(
áá 
Payment
áá "
payment
áá# *
)
áá* +
{
àà 
if
ââ 

(
ââ 
payment
ââ 
==
ââ 
null
ââ 
)
ââ 
{
ää 	
throw
ãã 
new
ãã #
ArgumentNullException
ãã +
(
ãã+ ,
nameof
ãã, 2
(
ãã2 3
payment
ãã3 :
)
ãã: ;
)
ãã; <
;
ãã< =
}
åå 	
if
éé 

(
éé 
Status
éé 
==
éé 
CashSessionStatus
éé '
.
éé' (
Closed
éé( .
)
éé. /
{
èè 	
throw
êê 
new
êê 

Exceptions
êê  
.
êê  !'
InvalidOperationException
êê! :
(
êê: ;
$str
êê; b
)
êêb c
;
êêc d
}
ëë 	
if
ìì 

(
ìì 
payment
ìì 
.
ìì 
PaymentType
ìì 
!=
ìì  "
PaymentType
ìì# .
.
ìì. /
Cash
ìì/ 3
)
ìì3 4
{
îî 	
throw
ïï 
new
ïï ,
BusinessRuleViolationException
ïï 4
(
ïï4 5
$str
ïï5 g
)
ïïg h
;
ïïh i
}
ññ 	
if
òò 

(
òò 
payment
òò 
.
òò 
CashSessionId
òò !
!=
òò" $
Id
òò% '
)
òò' (
{
ôô 	
throw
öö 
new
öö ,
BusinessRuleViolationException
öö 4
(
öö4 5
$str
öö5 d
)
ööd e
;
ööe f
}
õõ 	
	_payments
ùù 
.
ùù 
Add
ùù 
(
ùù 
payment
ùù 
)
ùù 
;
ùù #
CalculateExpectedCash
ûû 
(
ûû 
)
ûû 
;
ûû  
}
üü 
public
§§ 

void
§§ 
	AddPayout
§§ 
(
§§ 
Payout
§§  
payout
§§! '
)
§§' (
{
•• 
if
¶¶ 

(
¶¶ 
payout
¶¶ 
==
¶¶ 
null
¶¶ 
)
¶¶ 
{
ßß 	
throw
®® 
new
®® #
ArgumentNullException
®® +
(
®®+ ,
nameof
®®, 2
(
®®2 3
payout
®®3 9
)
®®9 :
)
®®: ;
;
®®; <
}
©© 	
if
´´ 

(
´´ 
Status
´´ 
==
´´ 
CashSessionStatus
´´ '
.
´´' (
Closed
´´( .
)
´´. /
{
¨¨ 	
throw
≠≠ 
new
≠≠ 

Exceptions
≠≠  
.
≠≠  !'
InvalidOperationException
≠≠! :
(
≠≠: ;
$str
≠≠; a
)
≠≠a b
;
≠≠b c
}
ÆÆ 	
if
∞∞ 

(
∞∞ 
payout
∞∞ 
.
∞∞ 
CashSessionId
∞∞  
!=
∞∞! #
Id
∞∞$ &
)
∞∞& '
{
±± 	
throw
≤≤ 
new
≤≤ ,
BusinessRuleViolationException
≤≤ 4
(
≤≤4 5
$str
≤≤5 c
)
≤≤c d
;
≤≤d e
}
≥≥ 	
_payouts
µµ 
.
µµ 
Add
µµ 
(
µµ 
payout
µµ 
)
µµ 
;
µµ #
CalculateExpectedCash
∂∂ 
(
∂∂ 
)
∂∂ 
;
∂∂  
}
∑∑ 
public
ºº 

void
ºº 
AddCashDrop
ºº 
(
ºº 
CashDrop
ºº $
cashDrop
ºº% -
)
ºº- .
{
ΩΩ 
if
ææ 

(
ææ 
cashDrop
ææ 
==
ææ 
null
ææ 
)
ææ 
{
øø 	
throw
¿¿ 
new
¿¿ #
ArgumentNullException
¿¿ +
(
¿¿+ ,
nameof
¿¿, 2
(
¿¿2 3
cashDrop
¿¿3 ;
)
¿¿; <
)
¿¿< =
;
¿¿= >
}
¡¡ 	
if
√√ 

(
√√ 
Status
√√ 
==
√√ 
CashSessionStatus
√√ '
.
√√' (
Closed
√√( .
)
√√. /
{
ƒƒ 	
throw
≈≈ 
new
≈≈ 

Exceptions
≈≈  
.
≈≈  !'
InvalidOperationException
≈≈! :
(
≈≈: ;
$str
≈≈; d
)
≈≈d e
;
≈≈e f
}
∆∆ 	
if
»» 

(
»» 
cashDrop
»» 
.
»» 
CashSessionId
»» "
!=
»»# %
Id
»»& (
)
»»( )
{
…… 	
throw
   
new
   ,
BusinessRuleViolationException
   4
(
  4 5
$str
  5 f
)
  f g
;
  g h
}
ÀÀ 	

_cashDrops
ÕÕ 
.
ÕÕ 
Add
ÕÕ 
(
ÕÕ 
cashDrop
ÕÕ 
)
ÕÕ  
;
ÕÕ  !#
CalculateExpectedCash
ŒŒ 
(
ŒŒ 
)
ŒŒ 
;
ŒŒ  
}
œœ 
public
‘‘ 

void
‘‘ 
AddDrawerBleed
‘‘ 
(
‘‘ 
DrawerBleed
‘‘ *
drawerBleed
‘‘+ 6
)
‘‘6 7
{
’’ 
if
÷÷ 

(
÷÷ 
drawerBleed
÷÷ 
==
÷÷ 
null
÷÷ 
)
÷÷  
{
◊◊ 	
throw
ÿÿ 
new
ÿÿ #
ArgumentNullException
ÿÿ +
(
ÿÿ+ ,
nameof
ÿÿ, 2
(
ÿÿ2 3
drawerBleed
ÿÿ3 >
)
ÿÿ> ?
)
ÿÿ? @
;
ÿÿ@ A
}
ŸŸ 	
if
€€ 

(
€€ 
Status
€€ 
==
€€ 
CashSessionStatus
€€ '
.
€€' (
Closed
€€( .
)
€€. /
{
‹‹ 	
throw
›› 
new
›› 

Exceptions
››  
.
››  !'
InvalidOperationException
››! :
(
››: ;
$str
››; g
)
››g h
;
››h i
}
ﬁﬁ 	
if
‡‡ 

(
‡‡ 
drawerBleed
‡‡ 
.
‡‡ 
CashSessionId
‡‡ %
!=
‡‡& (
Id
‡‡) +
)
‡‡+ ,
{
·· 	
throw
‚‚ 
new
‚‚ ,
BusinessRuleViolationException
‚‚ 4
(
‚‚4 5
$str
‚‚5 i
)
‚‚i j
;
‚‚j k
}
„„ 	
_drawerBleeds
ÂÂ 
.
ÂÂ 
Add
ÂÂ 
(
ÂÂ 
drawerBleed
ÂÂ %
)
ÂÂ% &
;
ÂÂ& '#
CalculateExpectedCash
ÊÊ 
(
ÊÊ 
)
ÊÊ 
;
ÊÊ  
}
ÁÁ 
private
ËË 
readonly
ËË 
List
ËË 
<
ËË !
TerminalTransaction
ËË -
>
ËË- .#
_terminalTransactions
ËË/ D
=
ËËE F
new
ËËG J
(
ËËJ K
)
ËËK L
;
ËËL M
public
ÈÈ 
!
IReadOnlyCollection
ÈÈ 
<
ÈÈ !
TerminalTransaction
ÈÈ 2
>
ÈÈ2 3"
TerminalTransactions
ÈÈ4 H
=>
ÈÈI K#
_terminalTransactions
ÈÈL a
.
ÈÈa b

AsReadOnly
ÈÈb l
(
ÈÈl m
)
ÈÈm n
;
ÈÈn o
public
ÎÎ 

void
ÎÎ 
AddTransaction
ÎÎ 
(
ÎÎ !
TerminalTransaction
ÎÎ 2
transaction
ÎÎ3 >
)
ÎÎ> ?
{
ÏÏ 
if
ÌÌ 

(
ÌÌ 
transaction
ÌÌ 
==
ÌÌ 
null
ÌÌ 
)
ÌÌ  
throw
ÌÌ! &
new
ÌÌ' *#
ArgumentNullException
ÌÌ+ @
(
ÌÌ@ A
nameof
ÌÌA G
(
ÌÌG H
transaction
ÌÌH S
)
ÌÌS T
)
ÌÌT U
;
ÌÌU V
if
ÓÓ 

(
ÓÓ 
Status
ÓÓ 
==
ÓÓ 
CashSessionStatus
ÓÓ '
.
ÓÓ' (
Closed
ÓÓ( .
)
ÓÓ. /
throw
ÓÓ0 5
new
ÓÓ6 9

Exceptions
ÓÓ: D
.
ÓÓD E'
InvalidOperationException
ÓÓE ^
(
ÓÓ^ _
$strÓÓ_ ä
)ÓÓä ã
;ÓÓã å
if
ÔÔ 

(
ÔÔ 
transaction
ÔÔ 
.
ÔÔ 
CashSessionId
ÔÔ %
!=
ÔÔ& (
Id
ÔÔ) +
)
ÔÔ+ ,
throw
ÔÔ- 2
new
ÔÔ3 6,
BusinessRuleViolationException
ÔÔ7 U
(
ÔÔU V
$strÔÔV Ñ
)ÔÔÑ Ö
;ÔÔÖ Ü#
_terminalTransactions
ÒÒ 
.
ÒÒ 
Add
ÒÒ !
(
ÒÒ! "
transaction
ÒÒ" -
)
ÒÒ- .
;
ÒÒ. /#
CalculateExpectedCash
ÚÚ 
(
ÚÚ 
)
ÚÚ 
;
ÚÚ  
}
ÛÛ 
}ÙÙ π
mC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\CashPayment.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
CashPayment		 
:		 
Payment		 "
{

 
	protected 
CashPayment 
( 
) 
{ 
} 
internal 
CashPayment 
( 
Guid 
ticketId 
, 
Money 
amount 
, 
UserId 
processedBy 
, 
Guid 

terminalId 
, 
string 
? 
globalId 
= 
null 
,  
Guid 
? 
splitGroupId 
= 
null !
,! "
int 
? 
splitSequence 
= 
null !
)! "
: 	
base
 
( 
ticketId 
, 
PaymentType $
.$ %
Cash% )
,) *
amount+ 1
,1 2
processedBy3 >
,> ?

terminalId@ J
,J K
globalIdL T
,T U
splitGroupIdV b
,b c
splitSequenced q
)q r
{ 
IsAuthorizable 
= 
false 
; 
} 
public 

static 
CashPayment 
Create $
($ %
Guid   
ticketId   
,   
Money!! 
amount!! 
,!! 
UserId"" 
processedBy"" 
,"" 
Guid## 

terminalId## 
,## 
string$$ 
?$$ 
globalId$$ 
=$$ 
null$$ 
,$$  
Guid%% 
?%% 
splitGroupId%% 
=%% 
null%% !
,%%! "
int&& 
?&& 
splitSequence&& 
=&& 
null&& !
)&&! "
{'' 
return(( 
new(( 
CashPayment(( 
((( 
ticketId(( '
,((' (
amount(() /
,((/ 0
processedBy((1 <
,((< =

terminalId((> H
,((H I
globalId((J R
,((R S
splitGroupId((T `
,((` a
splitSequence((b o
)((o p
;((p q
})) 
}** ß
jC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\CashDrop.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
CashDrop		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

Guid 
CashSessionId 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

Money 
Amount 
{ 
get 
; 
private &
set' *
;* +
}, -
public 

string 
? 
Reason 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

UserId 
ProcessedBy 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
null6 :
!: ;
;; <
public 

DateTime 
ProcessedAt 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
private 
CashDrop 
( 
) 
{ 
Amount 
= 
Money 
. 
Zero 
( 
) 
; 
} 
public 

static 
CashDrop 
Create !
(! "
Guid 
cashSessionId 
, 
Money 
amount 
, 
UserId 
processedBy 
, 
string 
? 
reason 
= 
null 
) 
{ 
if 

( 
amount 
<= 
Money 
. 
Zero  
(  !
)! "
)" #
{ 	
throw 
new 

Exceptions  
.  !*
BusinessRuleViolationException! ?
(? @
$str@ m
)m n
;n o
}   	
return"" 
new"" 
CashDrop"" 
{## 	
Id$$ 
=$$ 
Guid$$ 
.$$ 
NewGuid$$ 
($$ 
)$$ 
,$$  
CashSessionId%% 
=%% 
cashSessionId%% )
,%%) *
Amount&& 
=&& 
amount&& 
,&& 
Reason'' 
='' 
reason'' 
,'' 
ProcessedBy(( 
=(( 
processedBy(( %
,((% &
ProcessedAt)) 
=)) 
DateTime)) "
.))" #
UtcNow))# )
}** 	
;**	 

}++ 
},, ë*
lC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\AuditEvent.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 

AuditEvent		 
{

 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

AuditEventType 
	EventType #
{$ %
get& )
;) *
private+ 2
set3 6
;6 7
}8 9
public 

string 

EntityType 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
=3 4
string5 ;
.; <
Empty< A
;A B
public 

Guid 
EntityId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

Guid 
UserId 
{ 
get 
; 
private %
set& )
;) *
}+ ,
public 

DateTime 
	Timestamp 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 

string 
? 
BeforeState 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
public 

string 

AfterState 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
=3 4
string5 ;
.; <
Empty< A
;A B
public 

string 
Description 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
=4 5
string6 <
.< =
Empty= B
;B C
public 

Guid 
? 
CorrelationId 
{  
get! $
;$ %
private& -
set. 1
;1 2
}3 4
private 

AuditEvent 
( 
) 
{ 
} 
public 

static 

AuditEvent 
Create #
(# $
AuditEventType 
	eventType  
,  !
string   

entityType   
,   
Guid!! 
entityId!! 
,!! 
Guid"" 
userId"" 
,"" 
string## 

afterState## 
,## 
string$$ 
description$$ 
,$$ 
string%% 
?%% 
beforeState%% 
=%% 
null%% "
,%%" #
Guid&& 
?&& 
correlationId&& 
=&& 
null&& "
)&&" #
{'' 
if(( 

((( 
string(( 
.(( 
IsNullOrWhiteSpace(( %
(((% &

entityType((& 0
)((0 1
)((1 2
{)) 	
throw** 
new** 
ArgumentException** '
(**' (
$str**( N
,**N O
nameof**P V
(**V W

entityType**W a
)**a b
)**b c
;**c d
}++ 	
if-- 

(-- 
string-- 
.-- 
IsNullOrWhiteSpace-- %
(--% &

afterState--& 0
)--0 1
)--1 2
{.. 	
throw// 
new// 
ArgumentException// '
(//' (
$str//( N
,//N O
nameof//P V
(//V W

afterState//W a
)//a b
)//b c
;//c d
}00 	
if22 

(22 
string22 
.22 
IsNullOrWhiteSpace22 %
(22% &
description22& 1
)221 2
)222 3
{33 	
throw44 
new44 
ArgumentException44 '
(44' (
$str44( N
,44N O
nameof44P V
(44V W
description44W b
)44b c
)44c d
;44d e
}55 	
return77 
new77 

AuditEvent77 
{88 	
Id99 
=99 
Guid99 
.99 
NewGuid99 
(99 
)99 
,99  
	EventType:: 
=:: 
	eventType:: !
,::! "

EntityType;; 
=;; 

entityType;; #
,;;# $
EntityId<< 
=<< 
entityId<< 
,<<  
UserId== 
=== 
userId== 
,== 
	Timestamp>> 
=>> 
DateTime>>  
.>>  !
UtcNow>>! '
,>>' (
BeforeState?? 
=?? 
beforeState?? %
,??% &

AfterState@@ 
=@@ 

afterState@@ #
,@@# $
DescriptionAA 
=AA 
descriptionAA %
,AA% &
CorrelationIdBB 
=BB 
correlationIdBB )
}CC 	
;CC	 

}DD 
}EE ﬂ
sC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\AttendanceHistory.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public

 
class

 
AttendanceHistory

 
{ 
public 

Guid 
Id 
{ 
get 
; 
private !
set" %
;% &
}' (
public 

UserId 
UserId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

DateTime 
ClockInTime 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

DateTime 
? 
ClockOutTime !
{" #
get$ '
;' (
private) 0
set1 4
;4 5
}6 7
public 

Guid 
? 
ShiftId 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
private 
AttendanceHistory 
( 
) 
{ 
UserId 
= 
null 
! 
; 
} 
private 
AttendanceHistory 
( 
UserId $
userId% +
,+ ,
DateTime- 5
clockInTime6 A
,A B
GuidC G
?G H
shiftIdI P
)P Q
{ 
Id 

= 
Guid 
. 
NewGuid 
( 
) 
; 
UserId 
= 
userId 
; 
ClockInTime 
= 
clockInTime !
;! "
ShiftId 
= 
shiftId 
; 
} 
public   

static   
AttendanceHistory   #
Create  $ *
(  * +
UserId  + 1
userId  2 8
,  8 9
Guid  : >
?  > ?
shiftId  @ G
=  H I
null  J N
)  N O
{!! 
return"" 
new"" 
AttendanceHistory"" $
(""$ %
userId""% +
,""+ ,
DateTime""- 5
.""5 6
UtcNow""6 <
,""< =
shiftId""> E
)""E F
;""F G
}## 
public%% 

void%% 
ClockOut%% 
(%% 
)%% 
{&& 
if'' 

('' 
ClockOutTime'' 
.'' 
HasValue'' !
)''! "
{(( 	
throw)) 
new)) 

Exceptions))  
.))  !%
InvalidOperationException))! :
()): ;
$str)); Y
)))Y Z
;))Z [
}** 	
ClockOutTime,, 
=,, 
DateTime,, 
.,,  
UtcNow,,  &
;,,& '
}-- 
}.. È†
{C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\DomainServices\TicketDomainService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
DomainServices (
;( )
public 
class 
TicketDomainService  
{ 
private 
readonly 
TaxDomainService %
_taxDomainService& 7
;7 8
public 

TicketDomainService 
( 
TaxDomainService /
taxDomainService0 @
)@ A
{ 
_taxDomainService 
= 
taxDomainService ,
??- /
throw0 5
new6 9!
ArgumentNullException: O
(O P
nameofP V
(V W
taxDomainServiceW g
)g h
)h i
;i j
} 
public 

void 
CalculateTotals 
(  
Ticket  &
ticket' -
,- .
TaxGroup/ 7
?7 8
taxGroup9 A
=B C
nullD H
)H I
{ 
if 

( 
ticket 
== 
null 
) 
{ 	
throw 
new !
ArgumentNullException +
(+ ,
nameof, 2
(2 3
ticket3 9
)9 :
): ;
;; <
} 	
var 
subtotal 
= 
ticket 
. 

OrderLines (
.( )
	Aggregate) 2
(2 3
Money   
.   
Zero   
(   
)   
,   
(!! 
sum!! 
,!! 
line!! 
)!! 
=>!! 
sum!! 
+!!  
line!!! %
.!!% &
TotalAmount!!& 1
)!!1 2
;!!2 3
Money## 
	taxAmount## 
;## 
if%% 

(%% 
ticket%% 
.%% 
PriceIncludesTax%% #
)%%# $
{&& 	
var** 

baseAmount** 
=** 
_taxDomainService** .
.**. /1
%CalculateBaseAmountFromInclusivePrice**/ T
(**T U
subtotal**U ]
,**] ^
taxGroup**_ g
)**g h
;**h i
	taxAmount++ 
=++ 
subtotal++  
-++! "

baseAmount++# -
;++- .
},, 	
else-- 
{.. 	
	taxAmount00 
=00 
_taxDomainService00 )
.00) *
CalculateTax00* 6
(006 7
subtotal11 
,11 
taxGroup22 
,22 
ticket33 
.33 
IsTaxExempt33 "
)33" #
;33# $
}44 	
ticket66 
.66 "
CalculateTotalsWithTax66 %
(66% &
	taxAmount66& /
)66/ 0
;660 1
}77 
public<< 

bool<< 
CanAddPayment<< 
(<< 
Ticket<< $
ticket<<% +
,<<+ ,
Payment<<- 4
payment<<5 <
)<<< =
{== 
if>> 

(>> 
ticket>> 
==>> 
null>> 
)>> 
{?? 	
throw@@ 
new@@ !
ArgumentNullException@@ +
(@@+ ,
nameof@@, 2
(@@2 3
ticket@@3 9
)@@9 :
)@@: ;
;@@; <
}AA 	
ifCC 

(CC 
paymentCC 
==CC 
nullCC 
)CC 
{DD 	
throwEE 
newEE !
ArgumentNullExceptionEE +
(EE+ ,
nameofEE, 2
(EE2 3
paymentEE3 :
)EE: ;
)EE; <
;EE< =
}FF 	
ifHH 

(HH 
!HH 
ticketHH 
.HH 
CanAddPaymentHH !
(HH! "
paymentHH" )
)HH) *
)HH* +
{II 	
returnJJ 
falseJJ 
;JJ 
}KK 	
ifPP 

(PP 
paymentPP 
.PP 
PaymentTypePP 
!=PP  "
EnumerationsPP# /
.PP/ 0
PaymentTypePP0 ;
.PP; <
CashPP< @
)PP@ A
{QQ 	
varRR 
newPaidAmountRR 
=RR 
ticketRR  &
.RR& '

PaidAmountRR' 1
+RR2 3
paymentRR4 ;
.RR; <
AmountRR< B
;RRB C
varSS 
	toleranceSS 
=SS 
newSS 
MoneySS  %
(SS% &
$numSS& +
)SS+ ,
;SS, -
ifUU 
(UU 
newPaidAmountUU 
>UU 
ticketUU  &
.UU& '
TotalAmountUU' 2
+UU3 4
	toleranceUU5 >
)UU> ?
{VV 
throwWW 
newWW *
BusinessRuleViolationExceptionWW 8
(WW8 9
$"XX 
$strXX &
{XX& '
paymentXX' .
.XX. /
AmountXX/ 5
}XX5 6
$strXX6 S
"XXS T
+XXU V
$"YY 
$strYY $
{YY$ %
ticketYY% +
.YY+ ,

PaidAmountYY, 6
}YY6 7
$strYY7 @
{YY@ A
ticketYYA G
.YYG H
TotalAmountYYH S
}YYS T
$strYYT V
"YYV W
+YYX Y
$"ZZ 
$strZZ %
{ZZ% &
ticketZZ& ,
.ZZ, -
	DueAmountZZ- 6
}ZZ6 7
"ZZ7 8
)ZZ8 9
;ZZ9 :
}[[ 
}\\ 	
return^^ 
true^^ 
;^^ 
}__ 
publicdd 

booldd  
CanAddPartialPaymentdd $
(dd$ %
Ticketdd% +
ticketdd, 2
,dd2 3
Moneydd4 9
paymentAmountdd: G
)ddG H
{ee 
ifff 

(ff 
ticketff 
==ff 
nullff 
)ff 
{gg 	
throwhh 
newhh !
ArgumentNullExceptionhh +
(hh+ ,
nameofhh, 2
(hh2 3
tickethh3 9
)hh9 :
)hh: ;
;hh; <
}ii 	
ifkk 

(kk 
paymentAmountkk 
<=kk 
Moneykk "
.kk" #
Zerokk# '
(kk' (
)kk( )
)kk) *
{ll 	
throwmm 
newmm *
BusinessRuleViolationExceptionmm 4
(mm4 5
$strmm5 `
)mm` a
;mma b
}nn 	
ifpp 

(pp 
ticketpp 
.pp 
Statuspp 
==pp 
Enumerationspp )
.pp) *
TicketStatuspp* 6
.pp6 7
Closedpp7 =
||pp> @
ticketqq 
.qq 
Statusqq 
==qq 
Enumerationsqq )
.qq) *
TicketStatusqq* 6
.qq6 7
Voidedqq7 =
||qq> @
ticketrr 
.rr 
Statusrr 
==rr 
Enumerationsrr )
.rr) *
TicketStatusrr* 6
.rr6 7
Refundedrr7 ?
)rr? @
{ss 	
returntt 
falsett 
;tt 
}uu 	
varxx 
newPaidAmountxx 
=xx 
ticketxx "
.xx" #

PaidAmountxx# -
+xx. /
paymentAmountxx0 =
;xx= >
varyy 
	toleranceyy 
=yy 
newyy 
Moneyyy !
(yy! "
$numyy" '
)yy' (
;yy( )
return{{ 
newPaidAmount{{ 
<={{ 
ticket{{  &
.{{& '
TotalAmount{{' 2
+{{3 4
	tolerance{{5 >
;{{> ?
}|| 
public
ÅÅ 

bool
ÅÅ 
CanCloseTicket
ÅÅ 
(
ÅÅ 
Ticket
ÅÅ %
ticket
ÅÅ& ,
)
ÅÅ, -
{
ÇÇ 
if
ÉÉ 

(
ÉÉ 
ticket
ÉÉ 
==
ÉÉ 
null
ÉÉ 
)
ÉÉ 
{
ÑÑ 	
throw
ÖÖ 
new
ÖÖ #
ArgumentNullException
ÖÖ +
(
ÖÖ+ ,
nameof
ÖÖ, 2
(
ÖÖ2 3
ticket
ÖÖ3 9
)
ÖÖ9 :
)
ÖÖ: ;
;
ÖÖ; <
}
ÜÜ 	
return
àà 
ticket
àà 
.
àà 
CanClose
àà 
(
àà 
)
àà  
;
àà  !
}
ââ 
public
éé 

bool
éé 
CanVoidTicket
éé 
(
éé 
Ticket
éé $
ticket
éé% +
)
éé+ ,
{
èè 
if
êê 

(
êê 
ticket
êê 
==
êê 
null
êê 
)
êê 
{
ëë 	
throw
íí 
new
íí #
ArgumentNullException
íí +
(
íí+ ,
nameof
íí, 2
(
íí2 3
ticket
íí3 9
)
íí9 :
)
íí: ;
;
íí; <
}
ìì 	
return
ïï 
ticket
ïï 
.
ïï 
CanVoid
ïï 
(
ïï 
)
ïï 
;
ïï  
}
ññ 
public
õõ 

bool
õõ 
CanRefundTicket
õõ 
(
õõ  
Ticket
õõ  &
ticket
õõ' -
,
õõ- .
Money
õõ/ 4
refundAmount
õõ5 A
)
õõA B
{
úú 
if
ùù 

(
ùù 
ticket
ùù 
==
ùù 
null
ùù 
)
ùù 
{
ûû 	
throw
üü 
new
üü #
ArgumentNullException
üü +
(
üü+ ,
nameof
üü, 2
(
üü2 3
ticket
üü3 9
)
üü9 :
)
üü: ;
;
üü; <
}
†† 	
if
¢¢ 

(
¢¢ 
refundAmount
¢¢ 
<=
¢¢ 
Money
¢¢ !
.
¢¢! "
Zero
¢¢" &
(
¢¢& '
)
¢¢' (
)
¢¢( )
{
££ 	
throw
§§ 
new
§§ ,
BusinessRuleViolationException
§§ 4
(
§§4 5
$str
§§5 _
)
§§_ `
;
§§` a
}
•• 	
if
ßß 

(
ßß 
!
ßß 
ticket
ßß 
.
ßß 
	CanRefund
ßß 
(
ßß 
)
ßß 
)
ßß  
{
®® 	
return
©© 
false
©© 
;
©© 
}
™™ 	
if
≠≠ 

(
≠≠ 
refundAmount
≠≠ 
>
≠≠ 
ticket
≠≠ !
.
≠≠! "

PaidAmount
≠≠" ,
)
≠≠, -
{
ÆÆ 	
throw
ØØ 
new
ØØ ,
BusinessRuleViolationException
ØØ 4
(
ØØ4 5
$"
ØØ5 7
$str
ØØ7 F
{
ØØF G
refundAmount
ØØG S
}
ØØS T
$str
ØØT q
{
ØØq r
ticket
ØØr x
.
ØØx y

PaidAmountØØy É
}ØØÉ Ñ
$strØØÑ Ü
"ØØÜ á
)ØØá à
;ØØà â
}
∞∞ 	
return
≤≤ 
true
≤≤ 
;
≤≤ 
}
≥≥ 
public
∏∏ 

bool
∏∏ 
CanSplitTicket
∏∏ 
(
∏∏ 
Ticket
∏∏ %
ticket
∏∏& ,
)
∏∏, -
{
ππ 
if
∫∫ 

(
∫∫ 
ticket
∫∫ 
==
∫∫ 
null
∫∫ 
)
∫∫ 
{
ªª 	
throw
ºº 
new
ºº #
ArgumentNullException
ºº +
(
ºº+ ,
nameof
ºº, 2
(
ºº2 3
ticket
ºº3 9
)
ºº9 :
)
ºº: ;
;
ºº; <
}
ΩΩ 	
return
øø 
ticket
øø 
.
øø 
CanSplit
øø 
(
øø 
)
øø  
;
øø  !
}
¿¿ 
public
≈≈ 

Money
≈≈ 
GetRemainingDue
≈≈  
(
≈≈  !
Ticket
≈≈! '
ticket
≈≈( .
)
≈≈. /
{
∆∆ 
if
«« 

(
«« 
ticket
«« 
==
«« 
null
«« 
)
«« 
{
»» 	
throw
…… 
new
…… #
ArgumentNullException
…… +
(
……+ ,
nameof
……, 2
(
……2 3
ticket
……3 9
)
……9 :
)
……: ;
;
……; <
}
   	
return
ÃÃ 
ticket
ÃÃ 
.
ÃÃ 
GetRemainingDue
ÃÃ %
(
ÃÃ% &
)
ÃÃ& '
;
ÃÃ' (
}
ÕÕ 
public
““ 

bool
““ 
CanReopenTicket
““ 
(
““  
Ticket
““  &
ticket
““' -
)
““- .
{
”” 
if
‘‘ 

(
‘‘ 
ticket
‘‘ 
==
‘‘ 
null
‘‘ 
)
‘‘ 
{
’’ 	
throw
÷÷ 
new
÷÷ #
ArgumentNullException
÷÷ +
(
÷÷+ ,
nameof
÷÷, 2
(
÷÷2 3
ticket
÷÷3 9
)
÷÷9 :
)
÷÷: ;
;
÷÷; <
}
◊◊ 	
return
⁄⁄ 
ticket
⁄⁄ 
.
⁄⁄ 
Status
⁄⁄ 
==
⁄⁄ 
Enumerations
⁄⁄  ,
.
⁄⁄, -
TicketStatus
⁄⁄- 9
.
⁄⁄9 :
Closed
⁄⁄: @
;
⁄⁄@ A
}
€€ 
public
ﬂﬂ 

void
ﬂﬂ '
ValidateCouponApplication
ﬂﬂ )
(
ﬂﬂ) *
Ticket
ﬂﬂ* 0
ticket
ﬂﬂ1 7
,
ﬂﬂ7 8
Discount
ﬂﬂ9 A
coupon
ﬂﬂB H
)
ﬂﬂH I
{
‡‡ 
if
·· 

(
·· 
ticket
·· 
==
·· 
null
·· 
)
·· 
throw
·· !
new
··" %#
ArgumentNullException
··& ;
(
··; <
nameof
··< B
(
··B C
ticket
··C I
)
··I J
)
··J K
;
··K L
if
‚‚ 

(
‚‚ 
coupon
‚‚ 
==
‚‚ 
null
‚‚ 
)
‚‚ 
throw
‚‚ !
new
‚‚" %#
ArgumentNullException
‚‚& ;
(
‚‚; <
nameof
‚‚< B
(
‚‚B C
coupon
‚‚C I
)
‚‚I J
)
‚‚J K
;
‚‚K L
if
‰‰ 

(
‰‰ 
!
‰‰ 
coupon
‰‰ 
.
‰‰ 
IsActive
‰‰ 
)
‰‰ 
{
ÂÂ 	
throw
ÊÊ 
new
ÊÊ ,
BusinessRuleViolationException
ÊÊ 5
(
ÊÊ5 6
$"
ÊÊ6 8
$str
ÊÊ8 @
{
ÊÊ@ A
coupon
ÊÊA G
.
ÊÊG H
Name
ÊÊH L
}
ÊÊL M
$str
ÊÊM ]
"
ÊÊ] ^
)
ÊÊ^ _
;
ÊÊ_ `
}
ÁÁ 	
if
ÈÈ 

(
ÈÈ 
coupon
ÈÈ 
.
ÈÈ 
ExpirationDate
ÈÈ !
.
ÈÈ! "
HasValue
ÈÈ" *
&&
ÈÈ+ -
coupon
ÈÈ. 4
.
ÈÈ4 5
ExpirationDate
ÈÈ5 C
.
ÈÈC D
Value
ÈÈD I
<
ÈÈJ K
DateTime
ÈÈL T
.
ÈÈT U
UtcNow
ÈÈU [
)
ÈÈ[ \
{
ÍÍ 	
throw
ÎÎ 
new
ÎÎ ,
BusinessRuleViolationException
ÎÎ 5
(
ÎÎ5 6
$"
ÎÎ6 8
$str
ÎÎ8 @
{
ÎÎ@ A
coupon
ÎÎA G
.
ÎÎG H
Name
ÎÎH L
}
ÎÎL M
$str
ÎÎM [
"
ÎÎ[ \
)
ÎÎ\ ]
;
ÎÎ] ^
}
ÏÏ 	
if
ÔÔ 

(
ÔÔ 
ticket
ÔÔ 
.
ÔÔ 
	Discounts
ÔÔ 
.
ÔÔ 
Any
ÔÔ  
(
ÔÔ  !
d
ÔÔ! "
=>
ÔÔ# %
d
ÔÔ& '
.
ÔÔ' (

DiscountId
ÔÔ( 2
==
ÔÔ3 5
coupon
ÔÔ6 <
.
ÔÔ< =
Id
ÔÔ= ?
)
ÔÔ? @
)
ÔÔ@ A
{
 	
throw
ÒÒ 
new
ÒÒ ,
BusinessRuleViolationException
ÒÒ 5
(
ÒÒ5 6
$"
ÒÒ6 8
$str
ÒÒ8 @
{
ÒÒ@ A
coupon
ÒÒA G
.
ÒÒG H
Name
ÒÒH L
}
ÒÒL M
$str
ÒÒM q
"
ÒÒq r
)
ÒÒr s
;
ÒÒs t
}
ÚÚ 	
if
ˆˆ 

(
ˆˆ 
coupon
ˆˆ 
.
ˆˆ 

MinimumBuy
ˆˆ 
!=
ˆˆ  
null
ˆˆ! %
&&
ˆˆ& (
ticket
ˆˆ) /
.
ˆˆ/ 0
SubtotalAmount
ˆˆ0 >
<
ˆˆ? @
coupon
ˆˆA G
.
ˆˆG H

MinimumBuy
ˆˆH R
)
ˆˆR S
{
˜˜ 	
throw
¯¯ 
new
¯¯ ,
BusinessRuleViolationException
¯¯ 5
(
¯¯5 6
$"
¯¯6 8
$str
¯¯8 I
{
¯¯I J
ticket
¯¯J P
.
¯¯P Q
SubtotalAmount
¯¯Q _
}
¯¯_ `
$str¯¯` à
{¯¯à â
coupon¯¯â è
.¯¯è ê

MinimumBuy¯¯ê ö
}¯¯ö õ
$str¯¯õ ©
{¯¯© ™
coupon¯¯™ ∞
.¯¯∞ ±
Name¯¯± µ
}¯¯µ ∂
$str¯¯∂ ∏
"¯¯∏ π
)¯¯π ∫
;¯¯∫ ª
}
˘˘ 	
if
˝˝ 

(
˝˝ 
coupon
˝˝ 
.
˝˝ 
MinimumQuantity
˝˝ "
.
˝˝" #
HasValue
˝˝# +
)
˝˝+ ,
{
˛˛ 	
var
ˇˇ 

totalItems
ˇˇ 
=
ˇˇ 
ticket
ˇˇ #
.
ˇˇ# $

OrderLines
ˇˇ$ .
.
ˇˇ. /
Sum
ˇˇ/ 2
(
ˇˇ2 3
l
ˇˇ3 4
=>
ˇˇ5 7
l
ˇˇ8 9
.
ˇˇ9 :
	ItemCount
ˇˇ: C
)
ˇˇC D
;
ˇˇD E
if
ÄÄ 
(
ÄÄ 

totalItems
ÄÄ 
<
ÄÄ 
coupon
ÄÄ #
.
ÄÄ# $
MinimumQuantity
ÄÄ$ 3
.
ÄÄ3 4
Value
ÄÄ4 9
)
ÄÄ9 :
{
ÅÅ 
throw
ÇÇ 
new
ÇÇ ,
BusinessRuleViolationException
ÇÇ 9
(
ÇÇ9 :
$"
ÇÇ: <
$str
ÇÇ< O
{
ÇÇO P

totalItems
ÇÇP Z
}
ÇÇZ [
$str
ÇÇ[ |
{
ÇÇ| }
couponÇÇ} É
.ÇÇÉ Ñ
MinimumQuantityÇÇÑ ì
}ÇÇì î
$strÇÇî ¢
{ÇÇ¢ £
couponÇÇ£ ©
.ÇÇ© ™
NameÇÇ™ Æ
}ÇÇÆ Ø
$strÇÇØ ±
"ÇÇ± ≤
)ÇÇ≤ ≥
;ÇÇ≥ ¥
}
ÉÉ 
}
ÑÑ 	
}
ÖÖ 
public
ää 

Money
ää %
CalculateDiscountAmount
ää (
(
ää( )
Ticket
ää) /
ticket
ää0 6
,
ää6 7
Discount
ää8 @
discount
ääA I
)
ääI J
{
ãã 
if
åå 

(
åå 
ticket
åå 
==
åå 
null
åå 
)
åå 
throw
åå !
new
åå" %#
ArgumentNullException
åå& ;
(
åå; <
nameof
åå< B
(
ååB C
ticket
ååC I
)
ååI J
)
ååJ K
;
ååK L
if
çç 

(
çç 
discount
çç 
==
çç 
null
çç 
)
çç 
throw
çç #
new
çç$ '#
ArgumentNullException
çç( =
(
çç= >
nameof
çç> D
(
ççD E
discount
ççE M
)
ççM N
)
ççN O
;
ççO P
if
èè 

(
èè 
discount
èè 
.
èè 
Type
èè 
==
èè 
Enumerations
èè )
.
èè) *
DiscountType
èè* 6
.
èè6 7
Amount
èè7 =
)
èè= >
{
êê 	
return
ëë 
new
ëë 
Money
ëë 
(
ëë 
discount
ëë %
.
ëë% &
Value
ëë& +
,
ëë+ ,
ticket
ëë- 3
.
ëë3 4
SubtotalAmount
ëë4 B
.
ëëB C
Currency
ëëC K
)
ëëK L
;
ëëL M
}
íí 	
else
ìì 
if
ìì 
(
ìì 
discount
ìì 
.
ìì 
Type
ìì 
==
ìì !
Enumerations
ìì" .
.
ìì. /
DiscountType
ìì/ ;
.
ìì; <

Percentage
ìì< F
)
ììF G
{
îî 	
var
ññ 

percentage
ññ 
=
ññ 
discount
ññ %
.
ññ% &
Value
ññ& +
/
ññ, -
$num
ññ. 2
;
ññ2 3
return
óó 
ticket
óó 
.
óó 
SubtotalAmount
óó (
*
óó) *

percentage
óó+ 5
;
óó5 6
}
òò 	
else
ôô 
{
öö 	
return
ûû 
Money
ûû 
.
ûû 
Zero
ûû 
(
ûû 
ticket
ûû $
.
ûû$ %
SubtotalAmount
ûû% 3
.
ûû3 4
Currency
ûû4 <
)
ûû< =
;
ûû= >
}
üü 	
}
†† 
}°° è>
xC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\DomainServices\TaxDomainService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
DomainServices (
;( )
public		 
class		 
TaxDomainService		 
{

 
public 

Money 
CalculateTax 
( 
Money #
subtotal$ ,
,, -
TaxGroup. 6
?6 7
taxGroup8 @
,@ A
boolB F
isTaxExemptG R
=S T
falseU Z
)Z [
{ 
if 

( 
isTaxExempt 
) 
{ 	
return 
Money 
. 
Zero 
( 
) 
;  
} 	
if 

( 
taxGroup 
== 
null 
) 
{ 	
return 
Money 
. 
Zero 
( 
) 
;  
} 	
return 
taxGroup 
. 
CalculateTotalTax )
() *
subtotal* 2
)2 3
;3 4
} 
public   

Money   
CalculateTax   
(   
Money   #
subtotal  $ ,
,  , -
TaxRate  . 5
?  5 6
taxRate  7 >
,  > ?
bool  @ D
isTaxExempt  E P
=  Q R
false  S X
)  X Y
{!! 
if"" 

("" 
isTaxExempt"" 
)"" 
{## 	
return$$ 
Money$$ 
.$$ 
Zero$$ 
($$ 
)$$ 
;$$  
}%% 	
if'' 

('' 
taxRate'' 
=='' 
null'' 
)'' 
{(( 	
return)) 
Money)) 
.)) 
Zero)) 
()) 
))) 
;))  
}** 	
return,, 
taxRate,, 
.,, 
CalculateTax,, #
(,,# $
subtotal,,$ ,
),,, -
;,,- .
}-- 
public33 

Money33 1
%CalculateBaseAmountFromInclusivePrice33 6
(336 7
Money337 <
totalAmount33= H
,33H I
TaxGroup33J R
?33R S
taxGroup33T \
)33\ ]
{44 
if55 

(55 
taxGroup55 
==55 
null55 
||55 
!55  !
taxGroup55! )
.55) *
TaxRates55* 2
.552 3
Any553 6
(556 7
)557 8
)558 9
{66 	
return77 
totalAmount77 
;77 
}88 	
decimal>> 
combinedRate>> 
=>> 
taxGroup>> '
.>>' (
CombinedRate>>( 4
;>>4 5
if?? 

(?? 
combinedRate?? 
==?? 
$num?? 
)?? 
{@@ 	
returnAA 
totalAmountAA 
;AA 
}BB 	
decimalDD 

baseAmountDD 
=DD 
totalAmountDD (
.DD( )
AmountDD) /
/DD0 1
(DD2 3
$numDD3 4
+DD5 6
combinedRateDD7 C
)DDC D
;DDD E
returnEE 
newEE 
MoneyEE 
(EE 

baseAmountEE #
)EE# $
;EE$ %
}FF 
publicKK 

MoneyKK 1
%CalculateBaseAmountFromInclusivePriceKK 6
(KK6 7
MoneyKK7 <
totalAmountKK= H
,KKH I
TaxRateKKJ Q
?KKQ R
taxRateKKS Z
)KKZ [
{LL 
ifMM 

(MM 
taxRateMM 
==MM 
nullMM 
||MM 
taxRateMM &
.MM& '
RateMM' +
==MM, .
$numMM/ 0
)MM0 1
{NN 	
returnOO 
totalAmountOO 
;OO 
}PP 	
decimalRR 

baseAmountRR 
=RR 
totalAmountRR (
.RR( )
AmountRR) /
/RR0 1
(RR2 3
$numRR3 4
+RR5 6
taxRateRR7 >
.RR> ?
RateRR? C
)RRC D
;RRD E
returnSS 
newSS 
MoneySS 
(SS 

baseAmountSS #
)SS# $
;SS$ %
}TT 
publicYY 

MoneyYY '
CalculateTotalAmountWithTaxYY ,
(YY, -
MoneyYY- 2

baseAmountYY3 =
,YY= >
TaxGroupYY? G
?YYG H
taxGroupYYI Q
,YYQ R
boolYYS W
isTaxExemptYYX c
=YYd e
falseYYf k
)YYk l
{ZZ 
Money[[ 
tax[[ 
=[[ 
CalculateTax[[  
([[  !

baseAmount[[! +
,[[+ ,
taxGroup[[- 5
,[[5 6
isTaxExempt[[7 B
)[[B C
;[[C D
return\\ 

baseAmount\\ 
+\\ 
tax\\ 
;\\  
}]] 
publicbb 

Moneybb '
CalculateTotalAmountWithTaxbb ,
(bb, -
Moneybb- 2

baseAmountbb3 =
,bb= >
TaxRatebb? F
?bbF G
taxRatebbH O
,bbO P
boolbbQ U
isTaxExemptbbV a
=bbb c
falsebbd i
)bbi j
{cc 
Moneydd 
taxdd 
=dd 
CalculateTaxdd  
(dd  !

baseAmountdd! +
,dd+ ,
taxRatedd- 4
,dd4 5
isTaxExemptdd6 A
)ddA B
;ddB C
returnee 

baseAmountee 
+ee 
taxee 
;ee  
}ff 
publicll 


Dictionaryll 
<ll 
stringll 
,ll 
Moneyll #
>ll# $!
CalculateTaxBreakdownll% :
(ll: ;
Moneyll; @
subtotalllA I
,llI J
TaxGroupllK S
?llS T
taxGroupllU ]
,ll] ^
boolll_ c
isTaxExemptlld o
=llp q
falsellr w
)llw x
{mm 
varnn 
	breakdownnn 
=nn 
newnn 

Dictionarynn &
<nn& '
stringnn' -
,nn- .
Moneynn/ 4
>nn4 5
(nn5 6
)nn6 7
;nn7 8
ifpp 

(pp 
isTaxExemptpp 
||pp 
taxGrouppp #
==pp$ &
nullpp' +
)pp+ ,
{qq 	
returnrr 
	breakdownrr 
;rr 
}ss 	
Moneyuu 
totalTaxuu 
=uu 
Moneyuu 
.uu 
Zerouu #
(uu# $
)uu$ %
;uu% &
Moneyvv 
currentBasevv 
=vv 
subtotalvv $
;vv$ %
foreachxx 
(xx 
varxx 
ratexx 
inxx 
taxGroupxx %
.xx% &
TaxRatesxx& .
)xx. /
{yy 	
Moneyzz 
	taxAmountzz 
=zz 
ratezz "
.zz" #
CalculateTaxzz# /
(zz/ 0
currentBasezz0 ;
,zz; <
totalTaxzz= E
)zzE F
;zzF G
	breakdown{{ 
[{{ 
rate{{ 
.{{ 
Name{{ 
]{{  
={{! "
	taxAmount{{# ,
;{{, -
totalTax|| 
+=|| 
	taxAmount|| !
;||! "
if~~ 
(~~ 
rate~~ 
.~~ 

IsCompound~~ 
)~~  
{ 
currentBase
ÄÄ 
=
ÄÄ 
subtotal
ÄÄ &
+
ÄÄ' (
totalTax
ÄÄ) 1
;
ÄÄ1 2
}
ÅÅ 
}
ÇÇ 	
return
ÑÑ 
	breakdown
ÑÑ 
;
ÑÑ 
}
ÖÖ 
}ÜÜ ñ 
ÇC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\DomainServices\ServiceChargeDomainService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
DomainServices (
;( )
public

 
class

 &
ServiceChargeDomainService

 '
{ 
public 

Money "
CalculateServiceCharge '
(' (
Money( -
subtotal. 6
,6 7
decimal8 ?
serviceChargeRate@ Q
)Q R
{ 
if 

( 
subtotal 
== 
null 
) 
{ 	
throw 
new !
ArgumentNullException +
(+ ,
nameof, 2
(2 3
subtotal3 ;
); <
)< =
;= >
} 	
if 

( 
serviceChargeRate 
< 
$num  !
||" $
serviceChargeRate% 6
>7 8
$num9 :
): ;
{ 	
throw 
new 

Exceptions  
.  !*
BusinessRuleViolationException! ?
(? @
$str@ {
){ |
;| }
} 	
if 

( 
subtotal 
<= 
Money 
. 
Zero "
(" #
)# $
)$ %
{ 	
return   
Money   
.   
Zero   
(   
)   
;    
}!! 	
return## 
subtotal## 
*## 
serviceChargeRate## +
;##+ ,
}$$ 
public** 

Money** +
CalculateServiceChargeForTicket** 0
(**0 1
Ticket**1 7
ticket**8 >
,**> ?
decimal**@ G
serviceChargeRate**H Y
)**Y Z
{++ 
if,, 

(,, 
ticket,, 
==,, 
null,, 
),, 
{-- 	
throw.. 
new.. !
ArgumentNullException.. +
(..+ ,
nameof.., 2
(..2 3
ticket..3 9
)..9 :
)..: ;
;..; <
}// 	
var22 "
subtotalAfterDiscounts22 "
=22# $
ticket22% +
.22+ ,
SubtotalAmount22, :
-22; <
ticket22= C
.22C D
DiscountAmount22D R
;22R S
return33 "
CalculateServiceCharge33 %
(33% &"
subtotalAfterDiscounts33& <
,33< =
serviceChargeRate33> O
)33O P
;33P Q
}44 
public99 

Money99 *
CalculateServiceChargePerGuest99 /
(99/ 0
int990 3
numberOfGuests994 B
,99B C
Money99D I
chargePerGuest99J X
)99X Y
{:: 
if;; 

(;; 
numberOfGuests;; 
<=;; 
$num;; 
);;  
{<< 	
throw== 
new== 

Exceptions==  
.==  !*
BusinessRuleViolationException==! ?
(==? @
$str==@ m
)==m n
;==n o
}>> 	
if@@ 

(@@ 
chargePerGuest@@ 
==@@ 
null@@ "
)@@" #
{AA 	
throwBB 
newBB !
ArgumentNullExceptionBB +
(BB+ ,
nameofBB, 2
(BB2 3
chargePerGuestBB3 A
)BBA B
)BBB C
;BBC D
}CC 	
ifEE 

(EE 
chargePerGuestEE 
<EE 
MoneyEE "
.EE" #
ZeroEE# '
(EE' (
)EE( )
)EE) *
{FF 	
throwGG 
newGG 

ExceptionsGG  
.GG  !*
BusinessRuleViolationExceptionGG! ?
(GG? @
$strGG@ f
)GGf g
;GGg h
}HH 	
returnJJ 
chargePerGuestJJ 
*JJ 
numberOfGuestsJJ  .
;JJ. /
}KK 
}LL ˛;
|C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\DomainServices\PaymentDomainService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
DomainServices (
;( )
public 
class  
PaymentDomainService !
{ 
public 

Money 
CalculateChange  
(  !
Payment! (
payment) 0
)0 1
{ 
if 

( 
payment 
== 
null 
) 
{ 	
throw 
new !
ArgumentNullException +
(+ ,
nameof, 2
(2 3
payment3 :
): ;
); <
;< =
} 	
if 

( 
payment 
. 
PaymentType 
!=  "
Enumerations# /
./ 0
PaymentType0 ;
.; <
Cash< @
)@ A
{ 	
throw 
new *
BusinessRuleViolationException 4
(4 5
$str5 h
)h i
;i j
} 	
if 

( 
payment 
. 
TenderAmount  
<! "
payment# *
.* +
Amount+ 1
)1 2
{ 	
throw 
new *
BusinessRuleViolationException 4
(4 5
$str5 u
)u v
;v w
} 	
return!! 
payment!! 
.!! 
TenderAmount!! #
-!!$ %
payment!!& -
.!!- .
Amount!!. 4
;!!4 5
}"" 
public'' 

bool'' 
CanVoidPayment'' 
('' 
Payment'' &
payment''' .
)''. /
{(( 
if)) 

()) 
payment)) 
==)) 
null)) 
))) 
{** 	
throw++ 
new++ !
ArgumentNullException++ +
(+++ ,
nameof++, 2
(++2 3
payment++3 :
)++: ;
)++; <
;++< =
},, 	
return.. 
!.. 
payment.. 
... 
IsVoided..  
;..  !
}// 
public44 

bool44 
CanRefundPayment44  
(44  !
Payment44! (
payment44) 0
,440 1
Money442 7
refundAmount448 D
)44D E
{55 
if66 

(66 
payment66 
==66 
null66 
)66 
{77 	
throw88 
new88 !
ArgumentNullException88 +
(88+ ,
nameof88, 2
(882 3
payment883 :
)88: ;
)88; <
;88< =
}99 	
if;; 

(;; 
refundAmount;; 
<=;; 
Money;; !
.;;! "
Zero;;" &
(;;& '
);;' (
);;( )
{<< 	
throw== 
new== *
BusinessRuleViolationException== 4
(==4 5
$str==5 _
)==_ `
;==` a
}>> 	
if@@ 

(@@ 
payment@@ 
.@@ 
IsVoided@@ 
)@@ 
{AA 	
returnBB 
falseBB 
;BB 
}CC 	
ifFF 

(FF 
paymentFF 
.FF 
IsAuthorizableFF "
&&FF# %
!FF& '
paymentFF' .
.FF. /

IsCapturedFF/ 9
)FF9 :
{GG 	
returnHH 
falseHH 
;HH 
}II 	
ifLL 

(LL 
refundAmountLL 
>LL 
paymentLL "
.LL" #
AmountLL# )
)LL) *
{MM 	
throwNN 
newNN *
BusinessRuleViolationExceptionNN 4
(NN4 5
$"NN5 7
$strNN7 F
{NNF G
refundAmountNNG S
}NNS T
$strNNT t
{NNt u
paymentNNu |
.NN| }
Amount	NN} É
}
NNÉ Ñ
$str
NNÑ Ü
"
NNÜ á
)
NNá à
;
NNà â
}OO 	
returnQQ 
trueQQ 
;QQ 
}RR 
publicWW 

boolWW 
CanCapturePaymentWW !
(WW! "
PaymentWW" )
paymentWW* 1
)WW1 2
{XX 
ifYY 

(YY 
paymentYY 
==YY 
nullYY 
)YY 
{ZZ 	
throw[[ 
new[[ !
ArgumentNullException[[ +
([[+ ,
nameof[[, 2
([[2 3
payment[[3 :
)[[: ;
)[[; <
;[[< =
}\\ 	
if^^ 

(^^ 
!^^ 
payment^^ 
.^^ 
IsAuthorizable^^ #
)^^# $
{__ 	
return`` 
false`` 
;`` 
}aa 	
ifcc 

(cc 
paymentcc 
.cc 

IsCapturedcc 
)cc 
{dd 	
returnee 
falseee 
;ee 
}ff 	
ifhh 

(hh 
paymenthh 
.hh 
IsVoidedhh 
)hh 
{ii 	
returnjj 
falsejj 
;jj 
}kk 	
returnmm 
truemm 
;mm 
}nn 
publicss 

boolss 

CanAddTipsss 
(ss 
Paymentss "
paymentss# *
,ss* +
Moneyss, 1

tipsAmountss2 <
)ss< =
{tt 
ifuu 

(uu 
paymentuu 
==uu 
nulluu 
)uu 
{vv 	
throwww 
newww !
ArgumentNullExceptionww +
(ww+ ,
nameofww, 2
(ww2 3
paymentww3 :
)ww: ;
)ww; <
;ww< =
}xx 	
ifzz 

(zz 

tipsAmountzz 
<zz 
Moneyzz 
.zz 
Zerozz #
(zz# $
)zz$ %
)zz% &
{{{ 	
throw|| 
new|| *
BusinessRuleViolationException|| 4
(||4 5
$str||5 V
)||V W
;||W X
}}} 	
if 

( 
payment 
. 
IsVoided 
) 
{
ÄÄ 	
return
ÅÅ 
false
ÅÅ 
;
ÅÅ 
}
ÇÇ 	
return
ÖÖ 
true
ÖÖ 
;
ÖÖ 
}
ÜÜ 
public
ãã 

Money
ãã '
CalculateTipsExceedAmount
ãã *
(
ãã* +
Payment
ãã+ 2
payment
ãã3 :
)
ãã: ;
{
åå 
if
çç 

(
çç 
payment
çç 
==
çç 
null
çç 
)
çç 
{
éé 	
throw
èè 
new
èè #
ArgumentNullException
èè +
(
èè+ ,
nameof
èè, 2
(
èè2 3
payment
èè3 :
)
èè: ;
)
èè; <
;
èè< =
}
êê 	
if
íí 

(
íí 
payment
íí 
.
íí 

TipsAmount
íí 
<=
íí !
payment
íí" )
.
íí) *
Amount
íí* 0
)
íí0 1
{
ìì 	
return
îî 
Money
îî 
.
îî 
Zero
îî 
(
îî 
)
îî 
;
îî  
}
ïï 	
return
óó 
payment
óó 
.
óó 

TipsAmount
óó !
-
óó" #
payment
óó$ +
.
óó+ ,
Amount
óó, 2
;
óó2 3
}
òò 
}ôô ıU
}C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\DomainServices\DiscountDomainService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
DomainServices (
;( )
public 
class !
DiscountDomainService "
{ 
public 

Discount 
? 
GetMaxDiscount #
(# $
IEnumerable$ /
</ 0
Discount0 8
>8 9
	discounts: C
,C D
TicketE K
ticketL R
)R S
{ 
if 

( 
	discounts 
== 
null 
) 
{ 	
throw 
new !
ArgumentNullException +
(+ ,
nameof, 2
(2 3
	discounts3 <
)< =
)= >
;> ?
} 	
if 

( 
ticket 
== 
null 
) 
{ 	
throw 
new !
ArgumentNullException +
(+ ,
nameof, 2
(2 3
ticket3 9
)9 :
): ;
;; <
} 	
var 
eligibleDiscounts 
= 
	discounts  )
.   
Where   
(   
d   
=>   

IsEligible   "
(  " #
d  # $
,  $ %
ticket  & ,
)  , -
)  - .
.!! 
ToList!! 
(!! 
)!! 
;!! 
if## 

(## 
!## 
eligibleDiscounts## 
.## 
Any## "
(##" #
)### $
)##$ %
{$$ 	
return%% 
null%% 
;%% 
}&& 	
var)) 
discountAmounts)) 
=)) 
eligibleDiscounts)) /
.** 
Select** 
(** 
d** 
=>** 
new** 
{++ 
Discount,, 
=,, 
d,, 
,,, 
Amount-- 
=-- #
CalculateDiscountAmount-- 0
(--0 1
d--1 2
,--2 3
ticket--4 :
.--: ;
SubtotalAmount--; I
)--I J
}.. 
).. 
.// 
ToList// 
(// 
)// 
;// 
return11 
discountAmounts11 
.22 
OrderByDescending22 
(22 
x22  
=>22! #
x22$ %
.22% &
Amount22& ,
)22, -
.33 
First33 
(33 
)33 
.44 
Discount44 
;44 
}55 
public;; 

Discount;; 
?;; 
GetMaxDiscount;; #
(;;# $
IEnumerable;;$ /
<;;/ 0
Discount;;0 8
>;;8 9
	discounts;;: C
,;;C D
	OrderLine;;E N
	orderLine;;O X
);;X Y
{<< 
if== 

(== 
	discounts== 
==== 
null== 
)== 
{>> 	
throw?? 
new?? !
ArgumentNullException?? +
(??+ ,
nameof??, 2
(??2 3
	discounts??3 <
)??< =
)??= >
;??> ?
}@@ 	
ifBB 

(BB 
	orderLineBB 
==BB 
nullBB 
)BB 
{CC 	
throwDD 
newDD !
ArgumentNullExceptionDD +
(DD+ ,
nameofDD, 2
(DD2 3
	orderLineDD3 <
)DD< =
)DD= >
;DD> ?
}EE 	
varGG 
eligibleDiscountsGG 
=GG 
	discountsGG  )
.HH 
WhereHH 
(HH 
dHH 
=>HH 

IsEligibleHH "
(HH" #
dHH# $
,HH$ %
	orderLineHH& /
)HH/ 0
)HH0 1
.II 
ToListII 
(II 
)II 
;II 
ifKK 

(KK 
!KK 
eligibleDiscountsKK 
.KK 
AnyKK "
(KK" #
)KK# $
)KK$ %
{LL 	
returnMM 
nullMM 
;MM 
}NN 	
varQQ 
discountAmountsQQ 
=QQ 
eligibleDiscountsQQ /
.RR 
SelectRR 
(RR 
dRR 
=>RR 
newRR 
{SS 
DiscountTT 
=TT 
dTT 
,TT 
AmountUU 
=UU #
CalculateDiscountAmountUU 0
(UU0 1
dUU1 2
,UU2 3
	orderLineUU4 =
.UU= >
SubtotalAmountUU> L
)UUL M
}VV 
)VV 
.WW 
ToListWW 
(WW 
)WW 
;WW 
returnYY 
discountAmountsYY 
.ZZ 
OrderByDescendingZZ 
(ZZ 
xZZ  
=>ZZ! #
xZZ$ %
.ZZ% &
AmountZZ& ,
)ZZ, -
.[[ 
First[[ 
([[ 
)[[ 
.\\ 
Discount\\ 
;\\ 
}]] 
publicbb 

Moneybb #
CalculateDiscountAmountbb (
(bb( )
Discountbb) 1
discountbb2 :
,bb: ;
Moneybb< A
subtotalbbB J
)bbJ K
{cc 
ifdd 

(dd 
discountdd 
==dd 
nulldd 
)dd 
{ee 	
throwff 
newff !
ArgumentNullExceptionff +
(ff+ ,
nameofff, 2
(ff2 3
discountff3 ;
)ff; <
)ff< =
;ff= >
}gg 	
ifii 

(ii 
subtotalii 
<=ii 
Moneyii 
.ii 
Zeroii "
(ii" #
)ii# $
)ii$ %
{jj 	
returnkk 
Moneykk 
.kk 
Zerokk 
(kk 
)kk 
;kk  
}ll 	
returnnn 
discountnn 
.nn 
Typenn 
switchnn #
{oo 	
Enumerationspp 
.pp 
DiscountTypepp %
.pp% &
Amountpp& ,
=>pp- /
newpp0 3
Moneypp4 9
(pp9 :
Mathpp: >
.pp> ?
Minpp? B
(ppB C
discountppC K
.ppK L
ValueppL Q
,ppQ R
subtotalppS [
.pp[ \
Amountpp\ b
)ppb c
)ppc d
,ppd e
Enumerationsqq 
.qq 
DiscountTypeqq %
.qq% &

Percentageqq& 0
=>qq1 3
subtotalqq4 <
*qq= >
(qq? @
discountqq@ H
.qqH I
ValueqqI N
/qqO P
$numqqQ U
)qqU V
,qqV W
Enumerationsrr 
.rr 
DiscountTyperr %
.rr% &
RePricerr& -
=>rr. 0
subtotalrr1 9
-rr: ;
newrr< ?
Moneyrr@ E
(rrE F
discountrrF N
.rrN O
ValuerrO T
)rrT U
,rrU V
Enumerationsss 
.ss 
DiscountTypess %
.ss% &
AltPricess& .
=>ss/ 1
subtotalss2 :
-ss; <
newss= @
MoneyssA F
(ssF G
discountssG O
.ssO P
ValuessP U
)ssU V
,ssV W
_tt 
=>tt 
Moneytt 
.tt 
Zerott 
(tt 
)tt 
}uu 	
;uu	 

}vv 
public{{ 

bool{{ 

IsEligible{{ 
({{ 
Discount{{ #
discount{{$ ,
,{{, -
Ticket{{. 4
ticket{{5 ;
){{; <
{|| 
if}} 

(}} 
discount}} 
==}} 
null}} 
)}} 
{~~ 	
throw 
new !
ArgumentNullException +
(+ ,
nameof, 2
(2 3
discount3 ;
); <
)< =
;= >
}
ÄÄ 	
if
ÇÇ 

(
ÇÇ 
ticket
ÇÇ 
==
ÇÇ 
null
ÇÇ 
)
ÇÇ 
{
ÉÉ 	
throw
ÑÑ 
new
ÑÑ #
ArgumentNullException
ÑÑ +
(
ÑÑ+ ,
nameof
ÑÑ, 2
(
ÑÑ2 3
ticket
ÑÑ3 9
)
ÑÑ9 :
)
ÑÑ: ;
;
ÑÑ; <
}
ÖÖ 	
if
áá 

(
áá 
!
áá 
discount
áá 
.
áá 
IsActive
áá 
)
áá 
{
àà 	
return
ââ 
false
ââ 
;
ââ 
}
ää 	
if
çç 

(
çç 
discount
çç 
.
çç 

MinimumBuy
çç 
!=
çç  "
null
çç# '
&&
çç( *
ticket
çç+ 1
.
çç1 2
SubtotalAmount
çç2 @
<
ççA B
discount
ççC K
.
ççK L

MinimumBuy
ççL V
)
ççV W
{
éé 	
return
èè 
false
èè 
;
èè 
}
êê 	
return
íí 
true
íí 
;
íí 
}
ìì 
public
òò 

bool
òò 

IsEligible
òò 
(
òò 
Discount
òò #
discount
òò$ ,
,
òò, -
	OrderLine
òò. 7
	orderLine
òò8 A
)
òòA B
{
ôô 
if
öö 

(
öö 
discount
öö 
==
öö 
null
öö 
)
öö 
{
õõ 	
throw
úú 
new
úú #
ArgumentNullException
úú +
(
úú+ ,
nameof
úú, 2
(
úú2 3
discount
úú3 ;
)
úú; <
)
úú< =
;
úú= >
}
ùù 	
if
üü 

(
üü 
	orderLine
üü 
==
üü 
null
üü 
)
üü 
{
†† 	
throw
°° 
new
°° #
ArgumentNullException
°° +
(
°°+ ,
nameof
°°, 2
(
°°2 3
	orderLine
°°3 <
)
°°< =
)
°°= >
;
°°> ?
}
¢¢ 	
if
§§ 

(
§§ 
!
§§ 
discount
§§ 
.
§§ 
IsActive
§§ 
)
§§ 
{
•• 	
return
¶¶ 
false
¶¶ 
;
¶¶ 
}
ßß 	
if
™™ 

(
™™ 
discount
™™ 
.
™™ 
MinimumQuantity
™™ $
.
™™$ %
HasValue
™™% -
&&
™™. 0
	orderLine
™™1 :
.
™™: ;
	ItemCount
™™; D
<
™™E F
discount
™™G O
.
™™O P
MinimumQuantity
™™P _
.
™™_ `
Value
™™` e
)
™™e f
{
´´ 	
return
¨¨ 
false
¨¨ 
;
¨¨ 
}
≠≠ 	
return
ØØ 
true
ØØ 
;
ØØ 
}
∞∞ 
}±± €
ÄC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\DomainServices\CashSessionDomainService.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
DomainServices (
;( )
public

 
class

 $
CashSessionDomainService

 %
{ 
public 

void !
CalculateExpectedCash %
(% &
CashSession& 1
cashSession2 =
)= >
{ 
if 

( 
cashSession 
== 
null 
)  
{ 	
throw 
new !
ArgumentNullException +
(+ ,
nameof, 2
(2 3
cashSession3 >
)> ?
)? @
;@ A
} 	
cashSession 
. !
CalculateExpectedCash )
() *
)* +
;+ ,
} 
public 

bool 
CanCloseSession 
(  
CashSession  +
cashSession, 7
)7 8
{ 
if 

( 
cashSession 
== 
null 
)  
{   	
throw!! 
new!! !
ArgumentNullException!! +
(!!+ ,
nameof!!, 2
(!!2 3
cashSession!!3 >
)!!> ?
)!!? @
;!!@ A
}"" 	
return$$ 
cashSession$$ 
.$$ 
CanClose$$ #
($$# $
)$$$ %
;$$% &
}%% 
}&& ≈P
rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\DomainEvents\TicketEvents.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
DomainEvents &
;& '
public		 
sealed		 
class		 
TicketCreated		 !
:		" #
DomainEventBase		$ 3
{

 
public 

Guid 
TicketId 
{ 
get 
; 
}  !
public 

int 
TicketNumber 
{ 
get !
;! "
}# $
public 

UserId 
	CreatedBy 
{ 
get !
;! "
}# $
public 

TicketCreated 
( 
Guid 
ticketId &
,& '
int( +
ticketNumber, 8
,8 9
UserId: @
	createdByA J
,J K
GuidL P
?P Q
correlationIdR _
=` a
nullb f
)f g
: 	
base
 
( 
correlationId 
) 
{ 
TicketId 
= 
ticketId 
; 
TicketNumber 
= 
ticketNumber #
;# $
	CreatedBy 
= 
	createdBy 
; 
} 
} 
public 
sealed 
class 
TicketOpened  
:! "
DomainEventBase# 2
{ 
public 

Guid 
TicketId 
{ 
get 
; 
}  !
public 

TicketOpened 
( 
Guid 
ticketId %
,% &
Guid' +
?+ ,
correlationId- :
=; <
null= A
)A B
:   	
base  
 
(   
correlationId   
)   
{!! 
TicketId"" 
="" 
ticketId"" 
;"" 
}## 
}$$ 
public)) 
sealed)) 
class)) 
OrderLineAdded)) "
:))# $
DomainEventBase))% 4
{** 
public++ 

Guid++ 
TicketId++ 
{++ 
get++ 
;++ 
}++  !
public,, 

Guid,, 
OrderLineId,, 
{,, 
get,, !
;,,! "
},,# $
public.. 

OrderLineAdded.. 
(.. 
Guid.. 
ticketId.. '
,..' (
Guid..) -
orderLineId... 9
,..9 :
Guid..; ?
?..? @
correlationId..A N
=..O P
null..Q U
)..U V
:// 	
base//
 
(// 
correlationId// 
)// 
{00 
TicketId11 
=11 
ticketId11 
;11 
OrderLineId22 
=22 
orderLineId22 !
;22! "
}33 
}44 
public99 
sealed99 
class99 
OrderLineRemoved99 $
:99% &
DomainEventBase99' 6
{:: 
public;; 

Guid;; 
TicketId;; 
{;; 
get;; 
;;; 
};;  !
public<< 

Guid<< 
OrderLineId<< 
{<< 
get<< !
;<<! "
}<<# $
public>> 

OrderLineRemoved>> 
(>> 
Guid>>  
ticketId>>! )
,>>) *
Guid>>+ /
orderLineId>>0 ;
,>>; <
Guid>>= A
?>>A B
correlationId>>C P
=>>Q R
null>>S W
)>>W X
:?? 	
base??
 
(?? 
correlationId?? 
)?? 
{@@ 
TicketIdAA 
=AA 
ticketIdAA 
;AA 
OrderLineIdBB 
=BB 
orderLineIdBB !
;BB! "
}CC 
}DD 
publicII 
sealedII 
classII 
PaymentAddedII  
:II! "
DomainEventBaseII# 2
{JJ 
publicKK 

GuidKK 
TicketIdKK 
{KK 
getKK 
;KK 
}KK  !
publicLL 

GuidLL 
	PaymentIdLL 
{LL 
getLL 
;LL  
}LL! "
publicMM 

MoneyMM 
AmountMM 
{MM 
getMM 
;MM 
}MM  
publicOO 

PaymentAddedOO 
(OO 
GuidOO 
ticketIdOO %
,OO% &
GuidOO' +
	paymentIdOO, 5
,OO5 6
MoneyOO7 <
amountOO= C
,OOC D
GuidOOE I
?OOI J
correlationIdOOK X
=OOY Z
nullOO[ _
)OO_ `
:PP 	
basePP
 
(PP 
correlationIdPP 
)PP 
{QQ 
TicketIdRR 
=RR 
ticketIdRR 
;RR 
	PaymentIdSS 
=SS 
	paymentIdSS 
;SS 
AmountTT 
=TT 
amountTT 
;TT 
}UU 
}VV 
public[[ 
sealed[[ 
class[[ 

TicketPaid[[ 
:[[  
DomainEventBase[[! 0
{\\ 
public]] 

Guid]] 
TicketId]] 
{]] 
get]] 
;]] 
}]]  !
public^^ 

Money^^ 
TotalAmount^^ 
{^^ 
get^^ "
;^^" #
}^^$ %
public__ 

Money__ 

PaidAmount__ 
{__ 
get__ !
;__! "
}__# $
publicaa 


TicketPaidaa 
(aa 
Guidaa 
ticketIdaa #
,aa# $
Moneyaa% *
totalAmountaa+ 6
,aa6 7
Moneyaa8 =

paidAmountaa> H
,aaH I
GuidaaJ N
?aaN O
correlationIdaaP ]
=aa^ _
nullaa` d
)aad e
:bb 	
basebb
 
(bb 
correlationIdbb 
)bb 
{cc 
TicketIddd 
=dd 
ticketIddd 
;dd 
TotalAmountee 
=ee 
totalAmountee !
;ee! "

PaidAmountff 
=ff 

paidAmountff 
;ff  
}gg 
}hh 
publicmm 
sealedmm 
classmm 
TicketClosedmm  
:mm! "
DomainEventBasemm# 2
{nn 
publicoo 

Guidoo 
TicketIdoo 
{oo 
getoo 
;oo 
}oo  !
publicpp 

UserIdpp 
ClosedBypp 
{pp 
getpp  
;pp  !
}pp" #
publicrr 

TicketClosedrr 
(rr 
Guidrr 
ticketIdrr %
,rr% &
UserIdrr' -
closedByrr. 6
,rr6 7
Guidrr8 <
?rr< =
correlationIdrr> K
=rrL M
nullrrN R
)rrR S
:ss 	
basess
 
(ss 
correlationIdss 
)ss 
{tt 
TicketIduu 
=uu 
ticketIduu 
;uu 
ClosedByvv 
=vv 
closedByvv 
;vv 
}ww 
}xx 
public}} 
sealed}} 
class}} 
TicketVoided}}  
:}}! "
DomainEventBase}}# 2
{~~ 
public 

Guid 
TicketId 
{ 
get 
; 
}  !
public
ÄÄ 

UserId
ÄÄ 
VoidedBy
ÄÄ 
{
ÄÄ 
get
ÄÄ  
;
ÄÄ  !
}
ÄÄ" #
public
ÇÇ 

TicketVoided
ÇÇ 
(
ÇÇ 
Guid
ÇÇ 
ticketId
ÇÇ %
,
ÇÇ% &
UserId
ÇÇ' -
voidedBy
ÇÇ. 6
,
ÇÇ6 7
Guid
ÇÇ8 <
?
ÇÇ< =
correlationId
ÇÇ> K
=
ÇÇL M
null
ÇÇN R
)
ÇÇR S
:
ÉÉ 	
base
ÉÉ
 
(
ÉÉ 
correlationId
ÉÉ 
)
ÉÉ 
{
ÑÑ 
TicketId
ÖÖ 
=
ÖÖ 
ticketId
ÖÖ 
;
ÖÖ 
VoidedBy
ÜÜ 
=
ÜÜ 
voidedBy
ÜÜ 
;
ÜÜ 
}
áá 
}àà 
publicçç 
sealed
çç 
class
çç 
TicketRefunded
çç "
:
çç# $
DomainEventBase
çç% 4
{éé 
public
èè 

Guid
èè 
TicketId
èè 
{
èè 
get
èè 
;
èè 
}
èè  !
public
êê 

Money
êê 
RefundAmount
êê 
{
êê 
get
êê  #
;
êê# $
}
êê% &
public
íí 

TicketRefunded
íí 
(
íí 
Guid
íí 
ticketId
íí '
,
íí' (
Money
íí) .
refundAmount
íí/ ;
,
íí; <
Guid
íí= A
?
ííA B
correlationId
ííC P
=
ííQ R
null
ííS W
)
ííW X
:
ìì 	
base
ìì
 
(
ìì 
correlationId
ìì 
)
ìì 
{
îî 
TicketId
ïï 
=
ïï 
ticketId
ïï 
;
ïï 
RefundAmount
ññ 
=
ññ 
refundAmount
ññ #
;
ññ# $
}
óó 
}òò 
publicùù 
sealed
ùù 
class
ùù 
TicketReopened
ùù "
:
ùù# $
DomainEventBase
ùù% 4
{ûû 
public
üü 

Guid
üü 
TicketId
üü 
{
üü 
get
üü 
;
üü 
}
üü  !
public
°° 

TicketReopened
°° 
(
°° 
Guid
°° 
ticketId
°° '
,
°°' (
Guid
°°) -
?
°°- .
correlationId
°°/ <
=
°°= >
null
°°? C
)
°°C D
:
¢¢ 	
base
¢¢
 
(
¢¢ 
correlationId
¢¢ 
)
¢¢ 
{
££ 
TicketId
§§ 
=
§§ 
ticketId
§§ 
;
§§ 
}
•• 
}¶¶ ã/
sC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\DomainEvents\PaymentEvents.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
DomainEvents &
;& '
public		 
sealed		 
class		 
PaymentProcessed		 $
:		% &
DomainEventBase		' 6
{

 
public 

Guid 
	PaymentId 
{ 
get 
;  
}! "
public 

Guid 
TicketId 
{ 
get 
; 
}  !
public 

Money 
Amount 
{ 
get 
; 
}  
public 

PaymentProcessed 
( 
Guid  
	paymentId! *
,* +
Guid, 0
ticketId1 9
,9 :
Money; @
amountA G
,G H
GuidI M
?M N
correlationIdO \
=] ^
null_ c
)c d
: 	
base
 
( 
correlationId 
) 
{ 
	PaymentId 
= 
	paymentId 
; 
TicketId 
= 
ticketId 
; 
Amount 
= 
amount 
; 
} 
} 
public 
sealed 
class 
PaymentAuthorized %
:& '
DomainEventBase( 7
{ 
public 

Guid 
	PaymentId 
{ 
get 
;  
}! "
public 

Guid 
TicketId 
{ 
get 
; 
}  !
public 

Money 
Amount 
{ 
get 
; 
}  
public!! 

PaymentAuthorized!! 
(!! 
Guid!! !
	paymentId!!" +
,!!+ ,
Guid!!- 1
ticketId!!2 :
,!!: ;
Money!!< A
amount!!B H
,!!H I
Guid!!J N
?!!N O
correlationId!!P ]
=!!^ _
null!!` d
)!!d e
:"" 	
base""
 
("" 
correlationId"" 
)"" 
{## 
	PaymentId$$ 
=$$ 
	paymentId$$ 
;$$ 
TicketId%% 
=%% 
ticketId%% 
;%% 
Amount&& 
=&& 
amount&& 
;&& 
}'' 
}(( 
public-- 
sealed-- 
class-- 
PaymentCaptured-- #
:--$ %
DomainEventBase--& 5
{.. 
public// 

Guid// 
	PaymentId// 
{// 
get// 
;//  
}//! "
public00 

Guid00 
TicketId00 
{00 
get00 
;00 
}00  !
public11 

Money11 
Amount11 
{11 
get11 
;11 
}11  
public33 

PaymentCaptured33 
(33 
Guid33 
	paymentId33  )
,33) *
Guid33+ /
ticketId330 8
,338 9
Money33: ?
amount33@ F
,33F G
Guid33H L
?33L M
correlationId33N [
=33\ ]
null33^ b
)33b c
:44 	
base44
 
(44 
correlationId44 
)44 
{55 
	PaymentId66 
=66 
	paymentId66 
;66 
TicketId77 
=77 
ticketId77 
;77 
Amount88 
=88 
amount88 
;88 
}99 
}:: 
public?? 
sealed?? 
class?? 
PaymentVoided?? !
:??" #
DomainEventBase??$ 3
{@@ 
publicAA 

GuidAA 
	PaymentIdAA 
{AA 
getAA 
;AA  
}AA! "
publicBB 

GuidBB 
TicketIdBB 
{BB 
getBB 
;BB 
}BB  !
publicDD 

PaymentVoidedDD 
(DD 
GuidDD 
	paymentIdDD '
,DD' (
GuidDD) -
ticketIdDD. 6
,DD6 7
GuidDD8 <
?DD< =
correlationIdDD> K
=DDL M
nullDDN R
)DDR S
:EE 	
baseEE
 
(EE 
correlationIdEE 
)EE 
{FF 
	PaymentIdGG 
=GG 
	paymentIdGG 
;GG 
TicketIdHH 
=HH 
ticketIdHH 
;HH 
}II 
}JJ 
publicOO 
sealedOO 
classOO 
PaymentRefundedOO #
:OO$ %
DomainEventBaseOO& 5
{PP 
publicQQ 

GuidQQ 
	PaymentIdQQ 
{QQ 
getQQ 
;QQ  
}QQ! "
publicRR 

GuidRR 
TicketIdRR 
{RR 
getRR 
;RR 
}RR  !
publicSS 

MoneySS 
RefundAmountSS 
{SS 
getSS  #
;SS# $
}SS% &
publicUU 

PaymentRefundedUU 
(UU 
GuidUU 
	paymentIdUU  )
,UU) *
GuidUU+ /
ticketIdUU0 8
,UU8 9
MoneyUU: ?
refundAmountUU@ L
,UUL M
GuidUUN R
?UUR S
correlationIdUUT a
=UUb c
nullUUd h
)UUh i
:VV 	
baseVV
 
(VV 
correlationIdVV 
)VV 
{WW 
	PaymentIdXX 
=XX 
	paymentIdXX 
;XX 
TicketIdYY 
=YY 
ticketIdYY 
;YY 
RefundAmountZZ 
=ZZ 
refundAmountZZ #
;ZZ# $
}[[ 
}\\ ≤

uC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\DomainEvents\DomainEventBase.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
DomainEvents &
;& '
public 
abstract 
class 
DomainEventBase %
:& '
IDomainEvent( 4
{ 
public 

DateTime 

OccurredAt 
{  
get! $
;$ %
	protected& /
set0 3
;3 4
}5 6
public		 

Guid		 
?		 
CorrelationId		 
{		  
get		! $
;		$ %
	protected		& /
set		0 3
;		3 4
}		5 6
	protected 
DomainEventBase 
( 
) 
{ 

OccurredAt 
= 
DateTime 
. 
UtcNow $
;$ %
} 
	protected 
DomainEventBase 
( 
Guid "
?" #
correlationId$ 1
)1 2
:3 4
this5 9
(9 :
): ;
{ 
CorrelationId 
= 
correlationId %
;% &
} 
} Ω
rC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\DomainEvents\IDomainEvent.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
DomainEvents &
;& '
public 
	interface 
IDomainEvent 
{ 
DateTime 

OccurredAt 
{ 
get 
; 
}  
Guid 
? 	
CorrelationId
 
{ 
get 
; 
}  
} «
wC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\DomainEvents\CashSessionEvents.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
DomainEvents &
;& '
public		 
sealed		 
class		 
CashSessionOpened		 %
:		& '
DomainEventBase		( 7
{

 
public 

Guid 
CashSessionId 
{ 
get  #
;# $
}% &
public 

UserId 
UserId 
{ 
get 
; 
}  !
public 

Money 
OpeningBalance 
{  !
get" %
;% &
}' (
public 

CashSessionOpened 
( 
Guid !
cashSessionId" /
,/ 0
UserId1 7
userId8 >
,> ?
Money@ E
openingBalanceF T
,T U
GuidV Z
?Z [
correlationId\ i
=j k
nulll p
)p q
: 	
base
 
( 
correlationId 
) 
{ 
CashSessionId 
= 
cashSessionId %
;% &
UserId 
= 
userId 
; 
OpeningBalance 
= 
openingBalance '
;' (
} 
} 
public 
sealed 
class 
CashSessionClosed %
:& '
DomainEventBase( 7
{ 
public 

Guid 
CashSessionId 
{ 
get  #
;# $
}% &
public 

UserId 
ClosedBy 
{ 
get  
;  !
}" #
public 

Money 
ExpectedCash 
{ 
get  #
;# $
}% &
public   

Money   

ActualCash   
{   
get   !
;  ! "
}  # $
public!! 

Money!! 

Difference!! 
{!! 
get!! !
;!!! "
}!!# $
public## 

CashSessionClosed## 
(## 
Guid$$ 
cashSessionId$$ 
,$$ 
UserId%% 
closedBy%% 
,%% 
Money&& 
expectedCash&& 
,&& 
Money'' 

actualCash'' 
,'' 
Money(( 

difference(( 
,(( 
Guid)) 
?)) 
correlationId)) 
=)) 
null)) "
)))" #
:** 	
base**
 
(** 
correlationId** 
)** 
{++ 
CashSessionId,, 
=,, 
cashSessionId,, %
;,,% &
ClosedBy-- 
=-- 
closedBy-- 
;-- 
ExpectedCash.. 
=.. 
expectedCash.. #
;..# $

ActualCash// 
=// 

actualCash// 
;//  

Difference00 
=00 

difference00 
;00  
}11 
}22 