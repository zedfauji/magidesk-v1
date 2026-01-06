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
}DD û
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
}FF  
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
} ≥c
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
}√√ ˛
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
} £
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
} Õ
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
}JJ †
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
 ÿ
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
} Í
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
}## ∑
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
} ·
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
} Ã
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
} µ
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
} é
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
} ü
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
Paid 
=	 

$num 
, 
Closed 

= 
$num 
, 
Voided 

= 
$num 
, 
Refunded$$ 
=$$ 
$num$$ 
,$$ 
	Scheduled)) 
=)) 
$num)) 
}** Ã
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
}   “
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
 ”
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
}		 ⁄
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
} ä
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
 Ê
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
}++ µ1
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
,//Y Z
IsActive00 
=00 
true00 
}11 	
;11	 

}22 
public44 

void44 
SetRole44 
(44 
Guid44 
roleId44 #
)44# $
{55 
RoleId66 
=66 
roleId66 
;66 
}77 
public99 

void99 

Deactivate99 
(99 
)99 
{:: 
IsActive;; 
=;; 
false;; 
;;; 
}<< 
public>> 

void>> 
Activate>> 
(>> 
)>> 
{?? 
IsActive@@ 
=@@ 
true@@ 
;@@ 
}AA 
publicCC 

voidCC 
UpdateDetailsCC 
(CC 
stringCC $
	firstNameCC% .
,CC. /
stringCC0 6
lastNameCC7 ?
)CC? @
{DD 
ifEE 

(EE 
stringEE 
.EE 
IsNullOrWhiteSpaceEE %
(EE% &
	firstNameEE& /
)EE/ 0
)EE0 1
throwEE2 7
newEE8 ;
ArgumentExceptionEE< M
(EEM N
$strEEN c
)EEc d
;EEd e
	FirstNameFF 
=FF 
	firstNameFF 
;FF 
LastNameGG 
=GG 
lastNameGG 
;GG 
}JJ 
publicLL 

voidLL 
SetPinLL 
(LL 
stringLL 
encryptedPinLL *
)LL* +
{MM 
EncryptedPinNN 
=NN 
encryptedPinNN #
;NN# $
}OO 
}PP ⁄
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
}2 3
private 
TicketDiscount 
( 
) 
{ 
Amount 
= 
Money 
. 
Zero 
( 
) 
; 
} 
public 

static 
TicketDiscount  
Create! '
(' (
Guid 
ticketId 
, 
Guid 

discountId 
, 
string 
name 
, 
DiscountType 
type 
, 
decimal   
value   
,   
Money!! 
amount!! 
,!! 
Money"" 
?"" 
minimumAmount"" 
="" 
null"" #
)""# $
{## 
return$$ 
new$$ 
TicketDiscount$$ !
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
,''  

DiscountId(( 
=(( 

discountId(( #
,((# $
Name)) 
=)) 
name)) 
,)) 
Type** 
=** 
type** 
,** 
Value++ 
=++ 
value++ 
,++ 
Amount,, 
=,, 
amount,, 
,,, 
MinimumAmount-- 
=-- 
minimumAmount-- )
,--) *
	AppliedAt.. 
=.. 
DateTime..  
...  !
UtcNow..! '
}// 	
;//	 

}00 
}11 Î
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
} ∫î
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
?,, 
AssignedDriverId,, !
{,," #
get,,$ '
;,,' (
private,,) 0
set,,1 4
;,,4 5
},,6 7
public// 

IReadOnlyList// 
<// 
int// 
>// 
TableNumbers// *
=>//+ -
_tableNumbers//. ;
.//; <

AsReadOnly//< F
(//F G
)//G H
;//H I
public00 

int00 
NumberOfGuests00 
{00 
get00  #
;00# $
private00% ,
set00- 0
;000 1
}002 3
public33 

Money33 
SubtotalAmount33 
{33  !
get33" %
;33% &
private33' .
set33/ 2
;332 3
}334 5
public44 

Money44 
DiscountAmount44 
{44  !
get44" %
;44% &
private44' .
set44/ 2
;442 3
}444 5
public55 

Money55 
	TaxAmount55 
{55 
get55  
;55  !
private55" )
set55* -
;55- .
}55/ 0
public66 

Money66 
ServiceChargeAmount66 $
{66% &
get66' *
;66* +
private66, 3
set664 7
;667 8
}669 :
public77 

Money77  
DeliveryChargeAmount77 %
{77& '
get77( +
;77+ ,
private77- 4
set775 8
;778 9
}77: ;
public88 

Money88 
AdjustmentAmount88 !
{88" #
get88$ '
;88' (
private88) 0
set881 4
;884 5
}886 7
public99 

Money99 
TotalAmount99 
{99 
get99 "
;99" #
private99$ +
set99, /
;99/ 0
}991 2
public:: 

Money:: 

PaidAmount:: 
{:: 
get:: !
;::! "
private::# *
set::+ .
;::. /
}::0 1
public;; 

Money;; 
	DueAmount;; 
{;; 
get;;  
;;;  !
private;;" )
set;;* -
;;;- .
};;/ 0
public<< 

Money<< 
AdvanceAmount<< 
{<<  
get<<! $
;<<$ %
private<<& -
set<<. 1
;<<1 2
}<<3 4
public?? 

bool?? 
IsTaxExempt?? 
{?? 
get?? !
;??! "
private??# *
set??+ .
;??. /
}??0 1
public@@ 

bool@@ 
PriceIncludesTax@@  
{@@! "
get@@# &
;@@& '
private@@( /
set@@0 3
;@@3 4
}@@5 6
publicAA 

boolAA 
IsBarTabAA 
{AA 
getAA 
;AA 
privateAA  '
setAA( +
;AA+ ,
}AA- .
publicBB 

boolBB 

IsReOpenedBB 
{BB 
getBB  
;BB  !
privateBB" )
setBB* -
;BB- .
}BB/ 0
publicEE 

stringEE 
?EE 
DeliveryAddressEE "
{EE# $
getEE% (
;EE( )
privateEE* 1
setEE2 5
;EE5 6
}EE7 8
publicFF 

stringFF 
?FF 
ExtraDeliveryInfoFF $
{FF% &
getFF' *
;FF* +
privateFF, 3
setFF4 7
;FF7 8
}FF9 :
publicGG 

boolGG 
CustomerWillPickupGG "
{GG# $
getGG% (
;GG( )
privateGG* 1
setGG2 5
;GG5 6
}GG7 8
publicHH 

DateTimeHH 
?HH 
DispatchedTimeHH #
{HH$ %
getHH& )
;HH) *
privateHH+ 2
setHH3 6
;HH6 7
}HH8 9
publicII 

DateTimeII 
?II 
	ReadyTimeII 
{II  
getII! $
;II$ %
privateII& -
setII. 1
;II1 2
}II3 4
publicLL 

IReadOnlyCollectionLL 
<LL 
	OrderLineLL (
>LL( )

OrderLinesLL* 4
=>LL5 7
_orderLinesLL8 C
.LLC D

AsReadOnlyLLD N
(LLN O
)LLO P
;LLP Q
publicMM 

IReadOnlyCollectionMM 
<MM 
PaymentMM &
>MM& '
PaymentsMM( 0
=>MM1 3
	_paymentsMM4 =
.MM= >

AsReadOnlyMM> H
(MMH I
)MMI J
;MMJ K
publicNN 

IReadOnlyCollectionNN 
<NN 
TicketDiscountNN -
>NN- .
	DiscountsNN/ 8
=>NN9 ;

_discountsNN< F
.NNF G

AsReadOnlyNNG Q
(NNQ R
)NNR S
;NNS T
publicOO 

GratuityOO 
?OO 
GratuityOO 
{OO 
getOO  #
;OO# $
privateOO% ,
setOO- 0
;OO0 1
}OO2 3
publicRR 

intRR 
VersionRR 
{RR 
getRR 
;RR 
privateRR %
setRR& )
;RR) *
}RR+ ,
publicVV 

IReadOnlyDictionaryVV 
<VV 
stringVV %
,VV% &
stringVV' -
>VV- .

PropertiesVV/ 9
=>VV: <
_propertiesVV= H
.VVH I

AsReadOnlyVVI S
(VVS T
)VVT U
;VVU V
publicYY 

stringYY 
?YY 
NoteYY 
{YY 
getYY 
;YY 
privateYY &
setYY' *
;YY* +
}YY, -
private\\ 
Ticket\\ 
(\\ 
)\\ 
{]] 
SubtotalAmount^^ 
=^^ 
Money^^ 
.^^ 
Zero^^ #
(^^# $
)^^$ %
;^^% &
DiscountAmount__ 
=__ 
Money__ 
.__ 
Zero__ #
(__# $
)__$ %
;__% &
	TaxAmount`` 
=`` 
Money`` 
.`` 
Zero`` 
(`` 
)``  
;``  !
ServiceChargeAmountaa 
=aa 
Moneyaa #
.aa# $
Zeroaa$ (
(aa( )
)aa) *
;aa* + 
DeliveryChargeAmountbb 
=bb 
Moneybb $
.bb$ %
Zerobb% )
(bb) *
)bb* +
;bb+ ,
AdjustmentAmountcc 
=cc 
Moneycc  
.cc  !
Zerocc! %
(cc% &
)cc& '
;cc' (
TotalAmountdd 
=dd 
Moneydd 
.dd 
Zerodd  
(dd  !
)dd! "
;dd" #

PaidAmountee 
=ee 
Moneyee 
.ee 
Zeroee 
(ee  
)ee  !
;ee! "
	DueAmountff 
=ff 
Moneyff 
.ff 
Zeroff 
(ff 
)ff  
;ff  !
AdvanceAmountgg 
=gg 
Moneygg 
.gg 
Zerogg "
(gg" #
)gg# $
;gg$ %
NumberOfGuestshh 
=hh 
$numhh 
;hh 
}ii 
publicnn 

staticnn 
Ticketnn 
Createnn 
(nn  
intoo 
ticketNumberoo 
,oo 
UserIdpp 
	createdBypp 
,pp 
Guidqq 

terminalIdqq 
,qq 
Guidrr 
shiftIdrr 
,rr 
Guidss 
orderTypeIdss 
,ss 
stringtt 
?tt 
globalIdtt 
=tt 
nulltt 
)tt  
{uu 
returnvv 
newvv 
Ticketvv 
{ww 	
Idxx 
=xx 
Guidxx 
.xx 
NewGuidxx 
(xx 
)xx 
,xx  
TicketNumberyy 
=yy 
ticketNumberyy '
,yy' (
GlobalIdzz 
=zz 
globalIdzz 
,zz  
	CreatedBy{{ 
={{ 
	createdBy{{ !
,{{! "

TerminalId|| 
=|| 

terminalId|| #
,||# $
ShiftId}} 
=}} 
shiftId}} 
,}} 
OrderTypeId~~ 
=~~ 
orderTypeId~~ %
,~~% &
Status 
= 
TicketStatus !
.! "
Draft" '
,' (
	CreatedAt
ÄÄ 
=
ÄÄ 
DateTime
ÄÄ  
.
ÄÄ  !
UtcNow
ÄÄ! '
,
ÄÄ' (

ActiveDate
ÅÅ 
=
ÅÅ 
DateTime
ÅÅ !
.
ÅÅ! "
UtcNow
ÅÅ" (
,
ÅÅ( )
Version
ÇÇ 
=
ÇÇ 
$num
ÇÇ 
}
ÉÉ 	
;
ÉÉ	 

}
ÑÑ 
public
ââ 

void
ââ 
Open
ââ 
(
ââ 
)
ââ 
{
ää 
if
ãã 

(
ãã 
Status
ãã 
!=
ãã 
TicketStatus
ãã "
.
ãã" #
Draft
ãã# (
)
ãã( )
{
åå 	
throw
çç 
new
çç -
DomainInvalidOperationException
çç 5
(
çç5 6
$"
çç6 8
$str
çç8 N
{
ççN O
Status
ççO U
}
ççU V
$str
ççV ^
"
çç^ _
)
çç_ `
;
çç` a
}
éé 	
Status
êê 
=
êê 
TicketStatus
êê 
.
êê 
Open
êê "
;
êê" #
OpenedAt
ëë 
=
ëë 
DateTime
ëë 
.
ëë 
UtcNow
ëë "
;
ëë" #

ActiveDate
íí 
=
íí 
DateTime
íí 
.
íí 
UtcNow
íí $
;
íí$ %
IncrementVersion
ìì 
(
ìì 
)
ìì 
;
ìì 
}
îî 
public
ôô 

void
ôô 
AddOrderLine
ôô 
(
ôô 
	OrderLine
ôô &
	orderLine
ôô' 0
)
ôô0 1
{
öö 
if
õõ 

(
õõ 
	orderLine
õõ 
==
õõ 
null
õõ 
)
õõ 
{
úú 	
throw
ùù 
new
ùù #
ArgumentNullException
ùù +
(
ùù+ ,
nameof
ùù, 2
(
ùù2 3
	orderLine
ùù3 <
)
ùù< =
)
ùù= >
;
ùù> ?
}
ûû 	
if
†† 

(
†† 
Status
†† 
==
†† 
TicketStatus
†† "
.
††" #
Closed
††# )
||
††* ,
Status
††- 3
==
††4 6
TicketStatus
††7 C
.
††C D
Voided
††D J
||
††K M
Status
††N T
==
††U W
TicketStatus
††X d
.
††d e
Refunded
††e m
)
††m n
{
°° 	
throw
¢¢ 
new
¢¢ -
DomainInvalidOperationException
¢¢ 5
(
¢¢5 6
$"
¢¢6 8
$str
¢¢8 V
{
¢¢V W
Status
¢¢W ]
}
¢¢] ^
$str
¢¢^ f
"
¢¢f g
)
¢¢g h
;
¢¢h i
}
££ 	
if
•• 

(
•• 
	orderLine
•• 
.
•• 
TicketId
•• 
!=
•• !
Id
••" $
)
••$ %
{
¶¶ 	
throw
ßß 
new
ßß ,
BusinessRuleViolationException
ßß 4
(
ßß4 5
$str
ßß5 `
)
ßß` a
;
ßßa b
}
®® 	
_orderLines
™™ 
.
™™ 
Add
™™ 
(
™™ 
	orderLine
™™ !
)
™™! "
;
™™" #

ActiveDate
´´ 
=
´´ 
DateTime
´´ 
.
´´ 
UtcNow
´´ $
;
´´$ %
IncrementVersion
¨¨ 
(
¨¨ 
)
¨¨ 
;
¨¨ 
if
ØØ 

(
ØØ 
Status
ØØ 
==
ØØ 
TicketStatus
ØØ "
.
ØØ" #
Draft
ØØ# (
)
ØØ( )
{
∞∞ 	
Open
±± 
(
±± 
)
±± 
;
±± 
}
≤≤ 	
CalculateTotals
¥¥ 
(
¥¥ 
)
¥¥ 
;
¥¥ 
}
µµ 
public
∫∫ 

void
∫∫ 
RemoveOrderLine
∫∫ 
(
∫∫  
Guid
∫∫  $
orderLineId
∫∫% 0
)
∫∫0 1
{
ªª 
if
ºº 

(
ºº 
Status
ºº 
==
ºº 
TicketStatus
ºº "
.
ºº" #
Closed
ºº# )
||
ºº* ,
Status
ºº- 3
==
ºº4 6
TicketStatus
ºº7 C
.
ººC D
Voided
ººD J
||
ººK M
Status
ººN T
==
ººU W
TicketStatus
ººX d
.
ººd e
Refunded
ººe m
)
ººm n
{
ΩΩ 	
throw
ææ 
new
ææ -
DomainInvalidOperationException
ææ 5
(
ææ5 6
$"
ææ6 8
$str
ææ8 [
{
ææ[ \
Status
ææ\ b
}
ææb c
$str
ææc k
"
ææk l
)
ææl m
;
ææm n
}
øø 	
var
¡¡ 
	orderLine
¡¡ 
=
¡¡ 
_orderLines
¡¡ #
.
¡¡# $
FirstOrDefault
¡¡$ 2
(
¡¡2 3
ol
¡¡3 5
=>
¡¡6 8
ol
¡¡9 ;
.
¡¡; <
Id
¡¡< >
==
¡¡? A
orderLineId
¡¡B M
)
¡¡M N
;
¡¡N O
if
¬¬ 

(
¬¬ 
	orderLine
¬¬ 
==
¬¬ 
null
¬¬ 
)
¬¬ 
{
√√ 	
throw
ƒƒ 
new
ƒƒ ,
BusinessRuleViolationException
ƒƒ 4
(
ƒƒ4 5
$"
ƒƒ5 7
$str
ƒƒ7 A
{
ƒƒA B
orderLineId
ƒƒB M
}
ƒƒM N
$str
ƒƒN Y
"
ƒƒY Z
)
ƒƒZ [
;
ƒƒ[ \
}
≈≈ 	
_orderLines
«« 
.
«« 
Remove
«« 
(
«« 
	orderLine
«« $
)
««$ %
;
««% &

ActiveDate
»» 
=
»» 
DateTime
»» 
.
»» 
UtcNow
»» $
;
»»$ %
IncrementVersion
…… 
(
…… 
)
…… 
;
…… 
CalculateTotals
   
(
   
)
   
;
   
}
ÀÀ 
public
–– 

void
–– 

AddPayment
–– 
(
–– 
Payment
–– "
payment
––# *
)
––* +
{
—— 
if
““ 

(
““ 
payment
““ 
==
““ 
null
““ 
)
““ 
{
”” 	
throw
‘‘ 
new
‘‘ #
ArgumentNullException
‘‘ +
(
‘‘+ ,
nameof
‘‘, 2
(
‘‘2 3
payment
‘‘3 :
)
‘‘: ;
)
‘‘; <
;
‘‘< =
}
’’ 	
if
◊◊ 

(
◊◊ 
Status
◊◊ 
==
◊◊ 
TicketStatus
◊◊ "
.
◊◊" #
Closed
◊◊# )
||
◊◊* ,
Status
◊◊- 3
==
◊◊4 6
TicketStatus
◊◊7 C
.
◊◊C D
Voided
◊◊D J
||
◊◊K M
Status
◊◊N T
==
◊◊U W
TicketStatus
◊◊X d
.
◊◊d e
Refunded
◊◊e m
)
◊◊m n
{
ÿÿ 	
throw
ŸŸ 
new
ŸŸ -
DomainInvalidOperationException
ŸŸ 5
(
ŸŸ5 6
$"
ŸŸ6 8
$str
ŸŸ8 X
{
ŸŸX Y
Status
ŸŸY _
}
ŸŸ_ `
$str
ŸŸ` h
"
ŸŸh i
)
ŸŸi j
;
ŸŸj k
}
⁄⁄ 	
if
‹‹ 

(
‹‹ 
payment
‹‹ 
.
‹‹ 
TicketId
‹‹ 
!=
‹‹ 
Id
‹‹  "
)
‹‹" #
{
›› 	
throw
ﬁﬁ 
new
ﬁﬁ ,
BusinessRuleViolationException
ﬁﬁ 4
(
ﬁﬁ4 5
$str
ﬁﬁ5 ^
)
ﬁﬁ^ _
;
ﬁﬁ_ `
}
ﬂﬂ 	
	_payments
·· 
.
·· 
Add
·· 
(
·· 
payment
·· 
)
·· 
;
·· 

ActiveDate
‚‚ 
=
‚‚ 
DateTime
‚‚ 
.
‚‚ 
UtcNow
‚‚ $
;
‚‚$ %
IncrementVersion
„„ 
(
„„ 
)
„„ 
;
„„ 
if
ÊÊ 

(
ÊÊ 
Status
ÊÊ 
==
ÊÊ 
TicketStatus
ÊÊ "
.
ÊÊ" #
Draft
ÊÊ# (
)
ÊÊ( )
{
ÁÁ 	
Open
ËË 
(
ËË 
)
ËË 
;
ËË 
}
ÈÈ 	#
RecalculatePaidAmount
ÎÎ 
(
ÎÎ 
)
ÎÎ 
;
ÎÎ  
	DueAmount
ÔÔ 
=
ÔÔ 

PaidAmount
ÔÔ 
>=
ÔÔ !
TotalAmount
ÔÔ" -
?
 
Money
 
.
 
Zero
 
(
 
TotalAmount
 $
.
$ %
Currency
% -
)
- .
:
ÒÒ 
TotalAmount
ÒÒ 
-
ÒÒ 

PaidAmount
ÒÒ &
;
ÒÒ& '
if
ÙÙ 

(
ÙÙ 

PaidAmount
ÙÙ 
>=
ÙÙ 
TotalAmount
ÙÙ %
&&
ÙÙ& (
Status
ÙÙ) /
==
ÙÙ0 2
TicketStatus
ÙÙ3 ?
.
ÙÙ? @
Open
ÙÙ@ D
)
ÙÙD E
{
ıı 	
Status
ˆˆ 
=
ˆˆ 
TicketStatus
ˆˆ !
.
ˆˆ! "
Paid
ˆˆ" &
;
ˆˆ& '
}
˜˜ 	
}
¯¯ 
public
˝˝ 

bool
˝˝ 
CanAddPayment
˝˝ 
(
˝˝ 
Payment
˝˝ %
payment
˝˝& -
)
˝˝- .
{
˛˛ 
if
ˇˇ 

(
ˇˇ 
payment
ˇˇ 
==
ˇˇ 
null
ˇˇ 
)
ˇˇ 
{
ÄÄ 	
throw
ÅÅ 
new
ÅÅ #
ArgumentNullException
ÅÅ +
(
ÅÅ+ ,
nameof
ÅÅ, 2
(
ÅÅ2 3
payment
ÅÅ3 :
)
ÅÅ: ;
)
ÅÅ; <
;
ÅÅ< =
}
ÇÇ 	
if
ÑÑ 

(
ÑÑ 
Status
ÑÑ 
==
ÑÑ 
TicketStatus
ÑÑ "
.
ÑÑ" #
Closed
ÑÑ# )
||
ÑÑ* ,
Status
ÑÑ- 3
==
ÑÑ4 6
TicketStatus
ÑÑ7 C
.
ÑÑC D
Voided
ÑÑD J
||
ÑÑK M
Status
ÑÑN T
==
ÑÑU W
TicketStatus
ÑÑX d
.
ÑÑd e
Refunded
ÑÑe m
)
ÑÑm n
{
ÖÖ 	
return
ÜÜ 
false
ÜÜ 
;
ÜÜ 
}
áá 	
if
ââ 

(
ââ 
payment
ââ 
.
ââ 
TicketId
ââ 
!=
ââ 
Id
ââ  "
)
ââ" #
{
ää 	
return
ãã 
false
ãã 
;
ãã 
}
åå 	
return
éé 
true
éé 
;
éé 
}
èè 
public
îî 

void
îî 
Close
îî 
(
îî 
UserId
îî 
closedBy
îî %
)
îî% &
{
ïï 
if
ññ 

(
ññ 
!
ññ 
CanClose
ññ 
(
ññ 
)
ññ 
)
ññ 
{
óó 	
throw
òò 
new
òò -
DomainInvalidOperationException
òò 5
(
òò5 6
$"
òò6 8
$str
òò8 O
{
òòO P
Status
òòP V
}
òòV W
$str
òòW _
"
òò_ `
)
òò` a
;
òòa b
}
ôô 	
Status
õõ 
=
õõ 
TicketStatus
õõ 
.
õõ 
Closed
õõ $
;
õõ$ %
ClosedAt
úú 
=
úú 
DateTime
úú 
.
úú 
UtcNow
úú "
;
úú" #
ClosedBy
ùù 
=
ùù 
closedBy
ùù 
;
ùù 

ActiveDate
ûû 
=
ûû 
DateTime
ûû 
.
ûû 
UtcNow
ûû $
;
ûû$ %
IncrementVersion
üü 
(
üü 
)
üü 
;
üü 
}
†† 
public
•• 

bool
•• 
CanClose
•• 
(
•• 
)
•• 
{
¶¶ 
if
ßß 

(
ßß 
Status
ßß 
!=
ßß 
TicketStatus
ßß "
.
ßß" #
Paid
ßß# '
)
ßß' (
{
®® 	
return
©© 
false
©© 
;
©© 
}
™™ 	
if
¨¨ 

(
¨¨ 
	DueAmount
¨¨ 
>
¨¨ 
Money
¨¨ 
.
¨¨ 
Zero
¨¨ "
(
¨¨" #
)
¨¨# $
)
¨¨$ %
{
≠≠ 	
return
ÆÆ 
false
ÆÆ 
;
ÆÆ 
}
ØØ 	
return
±± 
true
±± 
;
±± 
}
≤≤ 
public
∑∑ 

void
∑∑ 
Void
∑∑ 
(
∑∑ 
UserId
∑∑ 
voidedBy
∑∑ $
,
∑∑$ %
string
∑∑& ,
reason
∑∑- 3
,
∑∑3 4
bool
∑∑5 9
waste
∑∑: ?
)
∑∑? @
{
∏∏ 
if
ππ 

(
ππ 
!
ππ 
CanVoid
ππ 
(
ππ 
)
ππ 
)
ππ 
{
∫∫ 	
throw
ªª 
new
ªª -
DomainInvalidOperationException
ªª 5
(
ªª5 6
$"
ªª6 8
$str
ªª8 N
{
ªªN O
Status
ªªO U
}
ªªU V
$str
ªªV ^
"
ªª^ _
)
ªª_ `
;
ªª` a
}
ºº 	
Status
ææ 
=
ææ 
TicketStatus
ææ 
.
ææ 
Voided
ææ $
;
ææ$ %
VoidedBy
øø 
=
øø 
voidedBy
øø 
;
øø 
_properties
¿¿ 
[
¿¿ 
$str
¿¿  
]
¿¿  !
=
¿¿" #
reason
¿¿$ *
;
¿¿* +
_properties
¡¡ 
[
¡¡ 
$str
¡¡ 
]
¡¡ 
=
¡¡  !
waste
¡¡" '
.
¡¡' (
ToString
¡¡( 0
(
¡¡0 1
)
¡¡1 2
;
¡¡2 3

ActiveDate
¬¬ 
=
¬¬ 
DateTime
¬¬ 
.
¬¬ 
UtcNow
¬¬ $
;
¬¬$ %
IncrementVersion
√√ 
(
√√ 
)
√√ 
;
√√ 
}
ƒƒ 
public
…… 

bool
…… 
CanVoid
…… 
(
…… 
)
…… 
{
   
if
ÀÀ 

(
ÀÀ 
Status
ÀÀ 
==
ÀÀ 
TicketStatus
ÀÀ "
.
ÀÀ" #
Closed
ÀÀ# )
||
ÀÀ* ,
Status
ÀÀ- 3
==
ÀÀ4 6
TicketStatus
ÀÀ7 C
.
ÀÀC D
Refunded
ÀÀD L
)
ÀÀL M
{
ÃÃ 	
return
ÕÕ 
false
ÕÕ 
;
ÕÕ 
}
ŒŒ 	
if
—— 

(
—— 
	_payments
—— 
.
—— 
Any
—— 
(
—— 
p
—— 
=>
—— 
!
——  
p
——  !
.
——! "
IsVoided
——" *
)
——* +
)
——+ ,
{
““ 	
return
”” 
false
”” 
;
”” 
}
‘‘ 	
return
÷÷ 
true
÷÷ 
;
÷÷ 
}
◊◊ 
public
‹‹ 

bool
‹‹ 
	CanRefund
‹‹ 
(
‹‹ 
)
‹‹ 
{
›› 
return
ﬂﬂ 
Status
ﬂﬂ 
==
ﬂﬂ 
TicketStatus
ﬂﬂ %
.
ﬂﬂ% &
Closed
ﬂﬂ& ,
;
ﬂﬂ, -
}
‡‡ 
public
ÊÊ 

void
ÊÊ 
ProcessRefund
ÊÊ 
(
ÊÊ 
Payment
ÊÊ %
refundPayment
ÊÊ& 3
)
ÊÊ3 4
{
ÁÁ 
if
ËË 

(
ËË 
refundPayment
ËË 
==
ËË 
null
ËË !
)
ËË! "
{
ÈÈ 	
throw
ÍÍ 
new
ÍÍ #
ArgumentNullException
ÍÍ +
(
ÍÍ+ ,
nameof
ÍÍ, 2
(
ÍÍ2 3
refundPayment
ÍÍ3 @
)
ÍÍ@ A
)
ÍÍA B
;
ÍÍB C
}
ÎÎ 	
if
ÌÌ 

(
ÌÌ 
!
ÌÌ 
	CanRefund
ÌÌ 
(
ÌÌ 
)
ÌÌ 
)
ÌÌ 
{
ÓÓ 	
throw
ÔÔ 
new
ÔÔ -
DomainInvalidOperationException
ÔÔ 5
(
ÔÔ5 6
$"
ÔÔ6 8
$str
ÔÔ8 P
{
ÔÔP Q
Status
ÔÔQ W
}
ÔÔW X
$str
ÔÔX `
"
ÔÔ` a
)
ÔÔa b
;
ÔÔb c
}
 	
if
ÚÚ 

(
ÚÚ 
refundPayment
ÚÚ 
.
ÚÚ 
TicketId
ÚÚ "
!=
ÚÚ# %
Id
ÚÚ& (
)
ÚÚ( )
{
ÛÛ 	
throw
ÙÙ 
new
ÙÙ ,
BusinessRuleViolationException
ÙÙ 4
(
ÙÙ4 5
$str
ÙÙ5 e
)
ÙÙe f
;
ÙÙf g
}
ıı 	
if
˜˜ 

(
˜˜ 
refundPayment
˜˜ 
.
˜˜ 
TransactionType
˜˜ )
!=
˜˜* ,
TransactionType
˜˜- <
.
˜˜< =
Debit
˜˜= B
)
˜˜B C
{
¯¯ 	
throw
˘˘ 
new
˘˘ ,
BusinessRuleViolationException
˘˘ 4
(
˘˘4 5
$str
˘˘5 f
)
˘˘f g
;
˘˘g h
}
˙˙ 	
	_payments
˝˝ 
.
˝˝ 
Add
˝˝ 
(
˝˝ 
refundPayment
˝˝ #
)
˝˝# $
;
˝˝$ %

ActiveDate
˛˛ 
=
˛˛ 
DateTime
˛˛ 
.
˛˛ 
UtcNow
˛˛ $
;
˛˛$ %
IncrementVersion
ˇˇ 
(
ˇˇ 
)
ˇˇ 
;
ˇˇ #
RecalculatePaidAmount
ÄÄ 
(
ÄÄ 
)
ÄÄ 
;
ÄÄ  
	DueAmount
ÉÉ 
=
ÉÉ 

PaidAmount
ÉÉ 
>=
ÉÉ !
TotalAmount
ÉÉ" -
?
ÑÑ 
Money
ÑÑ 
.
ÑÑ 
Zero
ÑÑ 
(
ÑÑ 
TotalAmount
ÑÑ $
.
ÑÑ$ %
Currency
ÑÑ% -
)
ÑÑ- .
:
ÖÖ 
TotalAmount
ÖÖ 
-
ÖÖ 

PaidAmount
ÖÖ &
;
ÖÖ& '
if
àà 

(
àà 

PaidAmount
àà 
<=
àà 
Money
àà 
.
àà  
Zero
àà  $
(
àà$ %
)
àà% &
)
àà& '
{
ââ 	
Status
ää 
=
ää 
TicketStatus
ää !
.
ää! "
Refunded
ää" *
;
ää* +
}
ãã 	
}
åå 
public
ëë 

bool
ëë 
CanSplit
ëë 
(
ëë 
)
ëë 
{
íí 
return
îî 
Status
îî 
==
îî 
TicketStatus
îî %
.
îî% &
Open
îî& *
;
îî* +
}
ïï 
public
öö 

Money
öö 
GetRemainingDue
öö  
(
öö  !
)
öö! "
{
õõ 
return
úú 
TotalAmount
úú 
-
úú 

PaidAmount
úú '
;
úú' (
}
ùù 
public
¢¢ 

void
¢¢ 
Reopen
¢¢ 
(
¢¢ 
)
¢¢ 
{
££ 
if
§§ 

(
§§ 
Status
§§ 
!=
§§ 
TicketStatus
§§ "
.
§§" #
Closed
§§# )
)
§§) *
{
•• 	
throw
¶¶ 
new
¶¶ -
DomainInvalidOperationException
¶¶ 5
(
¶¶5 6
$"
¶¶6 8
$str
¶¶8 P
{
¶¶P Q
Status
¶¶Q W
}
¶¶W X
$str
¶¶X `
"
¶¶` a
)
¶¶a b
;
¶¶b c
}
ßß 	
Status
©© 
=
©© 
TicketStatus
©© 
.
©© 
Open
©© "
;
©©" #

IsReOpened
™™ 
=
™™ 
true
™™ 
;
™™ 
ClosedAt
´´ 
=
´´ 
null
´´ 
;
´´ 
ClosedBy
¨¨ 
=
¨¨ 
null
¨¨ 
;
¨¨ 

ActiveDate
≠≠ 
=
≠≠ 
DateTime
≠≠ 
.
≠≠ 
UtcNow
≠≠ $
;
≠≠$ %
IncrementVersion
ÆÆ 
(
ÆÆ 
)
ÆÆ 
;
ÆÆ 
}
ØØ 
public
¥¥ 

void
¥¥ 
ApplyDiscount
¥¥ 
(
¥¥ 
TicketDiscount
¥¥ ,
discount
¥¥- 5
)
¥¥5 6
{
µµ 
if
∂∂ 

(
∂∂ 
discount
∂∂ 
==
∂∂ 
null
∂∂ 
)
∂∂ 
{
∑∑ 	
throw
∏∏ 
new
∏∏ #
ArgumentNullException
∏∏ +
(
∏∏+ ,
nameof
∏∏, 2
(
∏∏2 3
discount
∏∏3 ;
)
∏∏; <
)
∏∏< =
;
∏∏= >
}
ππ 	
if
ªª 

(
ªª 
Status
ªª 
==
ªª 
TicketStatus
ªª "
.
ªª" #
Closed
ªª# )
||
ªª* ,
Status
ªª- 3
==
ªª4 6
TicketStatus
ªª7 C
.
ªªC D
Voided
ªªD J
||
ªªK M
Status
ªªN T
==
ªªU W
TicketStatus
ªªX d
.
ªªd e
Refunded
ªªe m
)
ªªm n
{
ºº 	
throw
ΩΩ 
new
ΩΩ -
DomainInvalidOperationException
ΩΩ 5
(
ΩΩ5 6
$"
ΩΩ6 8
$str
ΩΩ8 [
{
ΩΩ[ \
Status
ΩΩ\ b
}
ΩΩb c
$str
ΩΩc k
"
ΩΩk l
)
ΩΩl m
;
ΩΩm n
}
ææ 	
if
¿¿ 

(
¿¿ 
discount
¿¿ 
.
¿¿ 
TicketId
¿¿ 
!=
¿¿  
Id
¿¿! #
)
¿¿# $
{
¡¡ 	
throw
¬¬ 
new
¬¬ ,
BusinessRuleViolationException
¬¬ 4
(
¬¬4 5
$str
¬¬5 _
)
¬¬_ `
;
¬¬` a
}
√√ 	

_discounts
≈≈ 
.
≈≈ 
Add
≈≈ 
(
≈≈ 
discount
≈≈ 
)
≈≈  
;
≈≈  !

ActiveDate
∆∆ 
=
∆∆ 
DateTime
∆∆ 
.
∆∆ 
UtcNow
∆∆ $
;
∆∆$ %
IncrementVersion
«« 
(
«« 
)
«« 
;
«« 
CalculateTotals
»» 
(
»» 
)
»» 
;
»» 
}
…… 
public
ŒŒ 

void
ŒŒ 
ApplyLineDiscount
ŒŒ !
(
ŒŒ! "
Guid
ŒŒ" &
orderLineId
ŒŒ' 2
,
ŒŒ2 3
OrderLineDiscount
ŒŒ4 E
discount
ŒŒF N
)
ŒŒN O
{
œœ 
if
–– 

(
–– 
discount
–– 
==
–– 
null
–– 
)
–– 
throw
–– #
new
––$ '#
ArgumentNullException
––( =
(
––= >
nameof
––> D
(
––D E
discount
––E M
)
––M N
)
––N O
;
––O P
if
““ 

(
““ 
Status
““ 
==
““ 
TicketStatus
““ "
.
““" #
Closed
““# )
||
““* ,
Status
““- 3
==
““4 6
TicketStatus
““7 C
.
““C D
Voided
““D J
||
““K M
Status
““N T
==
““U W
TicketStatus
““X d
.
““d e
Refunded
““e m
)
““m n
{
”” 	
throw
‘‘ 
new
‘‘ -
DomainInvalidOperationException
‘‘ 5
(
‘‘5 6
$"
‘‘6 8
$str
‘‘8 [
{
‘‘[ \
Status
‘‘\ b
}
‘‘b c
$str
‘‘c k
"
‘‘k l
)
‘‘l m
;
‘‘m n
}
’’ 	
var
◊◊ 
line
◊◊ 
=
◊◊ 
_orderLines
◊◊ 
.
◊◊ 
FirstOrDefault
◊◊ -
(
◊◊- .
x
◊◊. /
=>
◊◊0 2
x
◊◊3 4
.
◊◊4 5
Id
◊◊5 7
==
◊◊8 :
orderLineId
◊◊; F
)
◊◊F G
;
◊◊G H
if
ÿÿ 

(
ÿÿ 
line
ÿÿ 
==
ÿÿ 
null
ÿÿ 
)
ÿÿ 
throw
ŸŸ 
new
ŸŸ ,
BusinessRuleViolationException
ŸŸ 4
(
ŸŸ4 5
$"
ŸŸ5 7
$str
ŸŸ7 A
{
ŸŸA B
orderLineId
ŸŸB M
}
ŸŸM N
$str
ŸŸN h
"
ŸŸh i
)
ŸŸi j
;
ŸŸj k
line
€€ 
.
€€ 
ApplyDiscount
€€ 
(
€€ 
discount
€€ #
)
€€# $
;
€€$ %

ActiveDate
‹‹ 
=
‹‹ 
DateTime
‹‹ 
.
‹‹ 
UtcNow
‹‹ $
;
‹‹$ %
IncrementVersion
›› 
(
›› 
)
›› 
;
›› 
CalculateTotals
ﬁﬁ 
(
ﬁﬁ 
)
ﬁﬁ 
;
ﬁﬁ 
}
ﬂﬂ 
public
‰‰ 

void
‰‰ 
Schedule
‰‰ 
(
‰‰ 
DateTime
‰‰ !
deliveryDate
‰‰" .
)
‰‰. /
{
ÂÂ 
if
ÊÊ 

(
ÊÊ 
deliveryDate
ÊÊ 
<=
ÊÊ 
DateTime
ÊÊ $
.
ÊÊ$ %
UtcNow
ÊÊ% +
)
ÊÊ+ ,
{
ÁÁ 	
throw
ËË 
new
ËË ,
BusinessRuleViolationException
ËË 4
(
ËË4 5
$str
ËË5 g
)
ËËg h
;
ËËh i
}
ÈÈ 	
if
ÎÎ 

(
ÎÎ 
Status
ÎÎ 
!=
ÎÎ 
TicketStatus
ÎÎ "
.
ÎÎ" #
Draft
ÎÎ# (
&&
ÎÎ) +
Status
ÎÎ, 2
!=
ÎÎ3 5
TicketStatus
ÎÎ6 B
.
ÎÎB C
Open
ÎÎC G
)
ÎÎG H
{
ÏÏ 	
throw
ÌÌ 
new
ÌÌ -
DomainInvalidOperationException
ÌÌ 6
(
ÌÌ6 7
$"
ÌÌ7 9
$str
ÌÌ9 S
{
ÌÌS T
Status
ÌÌT Z
}
ÌÌZ [
$str
ÌÌ[ c
"
ÌÌc d
)
ÌÌd e
;
ÌÌe f
}
ÓÓ 	
DeliveryDate
 
=
 
deliveryDate
 #
;
# $
Status
ÒÒ 
=
ÒÒ 
TicketStatus
ÒÒ 
.
ÒÒ 
	Scheduled
ÒÒ '
;
ÒÒ' (

ActiveDate
ÚÚ 
=
ÚÚ 
DateTime
ÚÚ 
.
ÚÚ 
UtcNow
ÚÚ $
;
ÚÚ$ %
IncrementVersion
ÛÛ 
(
ÛÛ 
)
ÛÛ 
;
ÛÛ 
}
ÙÙ 
public
˘˘ 

void
˘˘ 
Fire
˘˘ 
(
˘˘ 
)
˘˘ 
{
˙˙ 
if
˚˚ 

(
˚˚ 
Status
˚˚ 
!=
˚˚ 
TicketStatus
˚˚ "
.
˚˚" #
	Scheduled
˚˚# ,
)
˚˚, -
{
¸¸ 	
throw
˝˝ 
new
˝˝ -
DomainInvalidOperationException
˝˝ 5
(
˝˝5 6
$"
˝˝6 8
$str
˝˝8 o
{
˝˝o p
Status
˝˝p v
}
˝˝v w
$str
˝˝w x
"
˝˝x y
)
˝˝y z
;
˝˝z {
}
˛˛ 	
Status
ÄÄ 
=
ÄÄ 
TicketStatus
ÄÄ 
.
ÄÄ 
Open
ÄÄ "
;
ÄÄ" #

ActiveDate
ÅÅ 
=
ÅÅ 
DateTime
ÅÅ 
.
ÅÅ 
UtcNow
ÅÅ $
;
ÅÅ$ %
IncrementVersion
ÉÉ 
(
ÉÉ 
)
ÉÉ 
;
ÉÉ 
}
ÑÑ 
public
ââ 

void
ââ 
ChangeOrderType
ââ 
(
ââ  
	OrderType
ââ  )
	orderType
ââ* 3
)
ââ3 4
{
ää 
if
ãã 

(
ãã 
	orderType
ãã 
==
ãã 
null
ãã 
)
ãã 
throw
ãã $
new
ãã% (#
ArgumentNullException
ãã) >
(
ãã> ?
nameof
ãã? E
(
ããE F
	orderType
ããF O
)
ããO P
)
ããP Q
;
ããQ R
if
åå 

(
åå 
	orderType
åå 
.
åå 
Id
åå 
==
åå 
OrderTypeId
åå '
)
åå' (
return
åå) /
;
åå/ 0
if
èè 

(
èè 
	orderType
èè 
.
èè 
Name
èè 
.
èè 
Contains
èè #
(
èè# $
$str
èè$ .
,
èè. /
StringComparison
èè0 @
.
èè@ A
OrdinalIgnoreCase
èèA R
)
èèR S
)
èèS T
{
êê 	
if
ëë 
(
ëë 

CustomerId
ëë 
==
ëë 
null
ëë #
)
ëë# $
throw
íí 
new
íí ,
BusinessRuleViolationException
íí 9
(
íí9 :
$str
íí: _
)
íí_ `
;
íí` a
if
ìì 
(
ìì 
string
ìì 
.
ìì  
IsNullOrWhiteSpace
ìì *
(
ìì* +
DeliveryAddress
ìì+ :
)
ìì: ;
)
ìì; <
throw
îî 
new
îî ,
BusinessRuleViolationException
îî 9
(
îî9 :
$str
îî: g
)
îîg h
;
îîh i
}
ïï 	
OrderTypeId
óó 
=
óó 
	orderType
óó 
.
óó  
Id
óó  "
;
óó" #
IsBarTab
òò 
=
òò 
	orderType
òò 
.
òò 
IsBarTab
òò %
;
òò% &

ActiveDate
ôô 
=
ôô 
DateTime
ôô 
.
ôô 
UtcNow
ôô $
;
ôô$ %
IncrementVersion
öö 
(
öö 
)
öö 
;
öö 
}
õõ 
public
†† 

void
†† 
SetCustomer
†† 
(
†† 
Guid
††  
?
††  !

customerId
††" ,
,
††, -
string
††. 4
?
††4 5
address
††6 =
=
††> ?
null
††@ D
,
††D E
string
††F L
?
††L M
	extraInfo
††N W
=
††X Y
null
††Z ^
)
††^ _
{
°° 

CustomerId
¢¢ 
=
¢¢ 

customerId
¢¢ 
;
¢¢  
DeliveryAddress
££ 
=
££ 
address
££ !
;
££! "
ExtraDeliveryInfo
§§ 
=
§§ 
	extraInfo
§§ %
;
§§% &

ActiveDate
•• 
=
•• 
DateTime
•• 
.
•• 
UtcNow
•• $
;
••$ %
IncrementVersion
¶¶ 
(
¶¶ 
)
¶¶ 
;
¶¶ 
if
©© 

(
©© 

CustomerId
©© 
==
©© 
null
©© 
&&
©© !
!
©©" #
string
©©# )
.
©©) * 
IsNullOrWhiteSpace
©©* <
(
©©< =
address
©©= D
)
©©D E
)
©©E F
{
™™ 	
}
≠≠ 	
}
ÆÆ 
public
µµ 

void
µµ 
CalculateTotals
µµ 
(
µµ  
)
µµ  !
{
∂∂ 
SubtotalAmount
∏∏ 
=
∏∏ 
_orderLines
∏∏ $
.
∏∏$ %
	Aggregate
∏∏% .
(
∏∏. /
Money
ππ 
.
ππ 
Zero
ππ 
(
ππ 
)
ππ 
,
ππ 
(
∫∫ 
sum
∫∫ 
,
∫∫ 
line
∫∫ 
)
∫∫ 
=>
∫∫ 
sum
∫∫ 
+
∫∫  
line
∫∫! %
.
∫∫% &
TotalAmount
∫∫& 1
)
∫∫1 2
;
∫∫2 3
	TaxAmount
ææ 
=
ææ 
IsTaxExempt
ææ 
?
øø 
Money
øø 
.
øø 
Zero
øø 
(
øø 
)
øø 
:
¿¿ 
SubtotalAmount
¿¿ 
*
¿¿ 
$num
¿¿ $
;
¿¿$ %
DiscountAmount
√√ 
=
√√ 

_discounts
√√ #
.
√√# $
	Aggregate
√√$ -
(
√√- .
Money
ƒƒ 
.
ƒƒ 
Zero
ƒƒ 
(
ƒƒ 
)
ƒƒ 
,
ƒƒ 
(
≈≈ 
sum
≈≈ 
,
≈≈ 
d
≈≈ 
)
≈≈ 
=>
≈≈ 
sum
≈≈ 
+
≈≈ 
d
≈≈ 
.
≈≈  
Amount
≈≈  &
)
≈≈& '
;
≈≈' (
if
   

(
   
PriceIncludesTax
   
)
   
{
ÀÀ 	
TotalAmount
ÕÕ 
=
ÕÕ 
SubtotalAmount
ÕÕ (
+
ŒŒ !
ServiceChargeAmount
ŒŒ %
+
œœ "
DeliveryChargeAmount
œœ &
+
–– 
AdjustmentAmount
–– "
-
—— 
DiscountAmount
——  
;
——  !
}
““ 	
else
”” 
{
‘‘ 	
TotalAmount
÷÷ 
=
÷÷ 
SubtotalAmount
÷÷ (
+
◊◊ 
	TaxAmount
◊◊ 
+
ÿÿ !
ServiceChargeAmount
ÿÿ %
+
ŸŸ "
DeliveryChargeAmount
ŸŸ &
+
⁄⁄ 
AdjustmentAmount
⁄⁄ "
-
€€ 
DiscountAmount
€€  
;
€€  !
}
‹‹ 	
if
ﬂﬂ 

(
ﬂﬂ 
Gratuity
ﬂﬂ 
!=
ﬂﬂ 
null
ﬂﬂ 
&&
ﬂﬂ 
Gratuity
ﬂﬂ  (
.
ﬂﬂ( )
Paid
ﬂﬂ) -
)
ﬂﬂ- .
{
‡‡ 	
TotalAmount
·· 
=
·· 
TotalAmount
·· %
+
··& '
Gratuity
··( 0
.
··0 1
Amount
··1 7
;
··7 8
}
‚‚ 	#
RecalculatePaidAmount
ÂÂ 
(
ÂÂ 
)
ÂÂ 
;
ÂÂ  
	DueAmount
ÊÊ 
=
ÊÊ 
TotalAmount
ÊÊ 
-
ÊÊ  !

PaidAmount
ÊÊ" ,
;
ÊÊ, -
}
ÁÁ 
internal
ÌÌ 
void
ÌÌ $
CalculateTotalsWithTax
ÌÌ (
(
ÌÌ( )
Money
ÌÌ) .
	taxAmount
ÌÌ/ 8
)
ÌÌ8 9
{
ÓÓ 
SubtotalAmount
 
=
 
_orderLines
 $
.
$ %
	Aggregate
% .
(
. /
Money
ÒÒ 
.
ÒÒ 
Zero
ÒÒ 
(
ÒÒ 
)
ÒÒ 
,
ÒÒ 
(
ÚÚ 
sum
ÚÚ 
,
ÚÚ 
line
ÚÚ 
)
ÚÚ 
=>
ÚÚ 
sum
ÚÚ 
+
ÚÚ  
line
ÚÚ! %
.
ÚÚ% &
TotalAmount
ÚÚ& 1
)
ÚÚ1 2
;
ÚÚ2 3
	TaxAmount
ıı 
=
ıı 
	taxAmount
ıı 
;
ıı 
DiscountAmount
¯¯ 
=
¯¯ 

_discounts
¯¯ #
.
¯¯# $
	Aggregate
¯¯$ -
(
¯¯- .
Money
˘˘ 
.
˘˘ 
Zero
˘˘ 
(
˘˘ 
)
˘˘ 
,
˘˘ 
(
˙˙ 
sum
˙˙ 
,
˙˙ 
d
˙˙ 
)
˙˙ 
=>
˙˙ 
sum
˙˙ 
+
˙˙ 
d
˙˙ 
.
˙˙  
Amount
˙˙  &
)
˙˙& '
;
˙˙' (
if
ˇˇ 

(
ˇˇ 
PriceIncludesTax
ˇˇ 
)
ˇˇ 
{
ÄÄ 	
TotalAmount
ÇÇ 
=
ÇÇ 
SubtotalAmount
ÇÇ (
+
ÉÉ !
ServiceChargeAmount
ÉÉ %
+
ÑÑ "
DeliveryChargeAmount
ÑÑ &
+
ÖÖ 
AdjustmentAmount
ÖÖ "
-
ÜÜ 
DiscountAmount
ÜÜ  
;
ÜÜ  !
}
áá 	
else
àà 
{
ââ 	
TotalAmount
ãã 
=
ãã 
SubtotalAmount
ãã (
+
åå 
	TaxAmount
åå 
+
çç !
ServiceChargeAmount
çç %
+
éé "
DeliveryChargeAmount
éé &
+
èè 
AdjustmentAmount
èè "
-
êê 
DiscountAmount
êê  
;
êê  !
}
ëë 	
if
îî 

(
îî 
Gratuity
îî 
!=
îî 
null
îî 
&&
îî 
Gratuity
îî  (
.
îî( )
Paid
îî) -
)
îî- .
{
ïï 	
TotalAmount
ññ 
=
ññ 
TotalAmount
ññ %
+
ññ& '
Gratuity
ññ( 0
.
ññ0 1
Amount
ññ1 7
;
ññ7 8
}
óó 	#
RecalculatePaidAmount
öö 
(
öö 
)
öö 
;
öö  
	DueAmount
õõ 
=
õõ 
TotalAmount
õõ 
-
õõ  !

PaidAmount
õõ" ,
;
õõ, -
}
úú 
private
¢¢ 
void
¢¢ #
RecalculatePaidAmount
¢¢ &
(
¢¢& '
)
¢¢' (
{
££ 

PaidAmount
§§ 
=
§§ 
	_payments
§§ 
.
•• 
Where
•• 
(
•• 
p
•• 
=>
•• 
!
•• 
p
•• 
.
•• 
IsVoided
•• #
)
••# $
.
¶¶ 
	Aggregate
¶¶ 
(
¶¶ 
Money
¶¶ 
.
¶¶ 
Zero
¶¶ !
(
¶¶! "
)
¶¶" #
,
¶¶# $
(
¶¶% &
sum
¶¶& )
,
¶¶) *
p
¶¶+ ,
)
¶¶, -
=>
¶¶. 0
p
ßß 
.
ßß 
TransactionType
ßß !
==
ßß" $
TransactionType
ßß% 4
.
ßß4 5
Credit
ßß5 ;
?
®® 
sum
®® 
+
®® 
p
®® 
.
®® 
Amount
®® $
:
©© 
sum
©© 
-
©© 
p
©© 
.
©© 
Amount
©© $
)
©©$ %
;
©©% &
}
™™ 
public
ØØ 

void
ØØ 
AddTableNumber
ØØ 
(
ØØ 
int
ØØ "
tableNumber
ØØ# .
)
ØØ. /
{
∞∞ 
if
±± 

(
±± 
tableNumber
±± 
<=
±± 
$num
±± 
)
±± 
{
≤≤ 	
throw
≥≥ 
new
≥≥ ,
BusinessRuleViolationException
≥≥ 4
(
≥≥4 5
$str
≥≥5 ^
)
≥≥^ _
;
≥≥_ `
}
¥¥ 	
if
∂∂ 

(
∂∂ 
!
∂∂ 
_tableNumbers
∂∂ 
.
∂∂ 
Contains
∂∂ #
(
∂∂# $
tableNumber
∂∂$ /
)
∂∂/ 0
)
∂∂0 1
{
∑∑ 	
_tableNumbers
∏∏ 
.
∏∏ 
Add
∏∏ 
(
∏∏ 
tableNumber
∏∏ )
)
∏∏) *
;
∏∏* +
IncrementVersion
ππ 
(
ππ 
)
ππ 
;
ππ 
}
∫∫ 	
}
ªª 
public
¿¿ 

void
¿¿ 
RemoveTableNumber
¿¿ !
(
¿¿! "
int
¿¿" %
tableNumber
¿¿& 1
)
¿¿1 2
{
¡¡ 
if
¬¬ 

(
¬¬ 
_tableNumbers
¬¬ 
.
¬¬ 
Remove
¬¬  
(
¬¬  !
tableNumber
¬¬! ,
)
¬¬, -
)
¬¬- .
{
√√ 	
IncrementVersion
ƒƒ 
(
ƒƒ 
)
ƒƒ 
;
ƒƒ 
}
≈≈ 	
}
∆∆ 
public
ÀÀ 

void
ÀÀ 
AssignTable
ÀÀ 
(
ÀÀ 
int
ÀÀ 
tableNumber
ÀÀ  +
)
ÀÀ+ ,
{
ÃÃ 
if
ÕÕ 

(
ÕÕ 
tableNumber
ÕÕ 
<=
ÕÕ 
$num
ÕÕ 
)
ÕÕ 
{
ŒŒ 	
throw
œœ 
new
œœ ,
BusinessRuleViolationException
œœ 4
(
œœ4 5
$str
œœ5 ^
)
œœ^ _
;
œœ_ `
}
–– 	
if
““ 

(
““ 
_tableNumbers
““ 
.
““ 
Count
““ 
==
““  "
$num
““# $
&&
““% '
_tableNumbers
““( 5
[
““5 6
$num
““6 7
]
““7 8
==
““9 ;
tableNumber
““< G
)
““G H
{
”” 	
return
‘‘ 
;
‘‘ 
}
’’ 	
_tableNumbers
◊◊ 
.
◊◊ 
Clear
◊◊ 
(
◊◊ 
)
◊◊ 
;
◊◊ 
_tableNumbers
ÿÿ 
.
ÿÿ 
Add
ÿÿ 
(
ÿÿ 
tableNumber
ÿÿ %
)
ÿÿ% &
;
ÿÿ& '
IncrementVersion
ŸŸ 
(
ŸŸ 
)
ŸŸ 
;
ŸŸ 
}
⁄⁄ 
public
ﬂﬂ 

void
ﬂﬂ 
AddGratuity
ﬂﬂ 
(
ﬂﬂ 
Gratuity
ﬂﬂ $
gratuity
ﬂﬂ% -
)
ﬂﬂ- .
{
‡‡ 
if
·· 

(
·· 
gratuity
·· 
==
·· 
null
·· 
)
·· 
{
‚‚ 	
throw
„„ 
new
„„ #
ArgumentNullException
„„ +
(
„„+ ,
nameof
„„, 2
(
„„2 3
gratuity
„„3 ;
)
„„; <
)
„„< =
;
„„= >
}
‰‰ 	
if
ÊÊ 

(
ÊÊ 
gratuity
ÊÊ 
.
ÊÊ 
TicketId
ÊÊ 
!=
ÊÊ  
Id
ÊÊ! #
)
ÊÊ# $
{
ÁÁ 	
throw
ËË 
new
ËË ,
BusinessRuleViolationException
ËË 4
(
ËË4 5
$str
ËË5 _
)
ËË_ `
;
ËË` a
}
ÈÈ 	
Gratuity
ÎÎ 
=
ÎÎ 
gratuity
ÎÎ 
;
ÎÎ 
CalculateTotals
ÏÏ 
(
ÏÏ 
)
ÏÏ 
;
ÏÏ 
}
ÌÌ 
public
ÚÚ 

void
ÚÚ  
MarkGratuityAsPaid
ÚÚ "
(
ÚÚ" #
)
ÚÚ# $
{
ÛÛ 
if
ÙÙ 

(
ÙÙ 
Gratuity
ÙÙ 
==
ÙÙ 
null
ÙÙ 
)
ÙÙ 
{
ıı 	
throw
ˆˆ 
new
ˆˆ -
DomainInvalidOperationException
ˆˆ 5
(
ˆˆ5 6
$str
ˆˆ6 T
)
ˆˆT U
;
ˆˆU V
}
˜˜ 	
Gratuity
˘˘ 
.
˘˘ 

MarkAsPaid
˘˘ 
(
˘˘ 
)
˘˘ 
;
˘˘ 
CalculateTotals
˙˙ 
(
˙˙ 
)
˙˙ 
;
˙˙ 
}
˚˚ 
public
ÄÄ 

void
ÄÄ $
MarkGratuityAsRefunded
ÄÄ &
(
ÄÄ& '
)
ÄÄ' (
{
ÅÅ 
if
ÇÇ 

(
ÇÇ 
Gratuity
ÇÇ 
==
ÇÇ 
null
ÇÇ 
)
ÇÇ 
{
ÉÉ 	
throw
ÑÑ 
new
ÑÑ -
DomainInvalidOperationException
ÑÑ 5
(
ÑÑ5 6
$str
ÑÑ6 X
)
ÑÑX Y
;
ÑÑY Z
}
ÖÖ 	
Gratuity
áá 
.
áá 
MarkAsRefunded
áá 
(
áá  
)
áá  !
;
áá! "
CalculateTotals
àà 
(
àà 
)
àà 
;
àà 
}
ââ 
public
èè 

void
èè 
SetServiceCharge
èè  
(
èè  !
Money
èè! &
amount
èè' -
)
èè- .
{
êê 
if
ëë 

(
ëë 
amount
ëë 
<
ëë 
Money
ëë 
.
ëë 
Zero
ëë 
(
ëë  
)
ëë  !
)
ëë! "
{
íí 	
throw
ìì 
new
ìì ,
BusinessRuleViolationException
ìì 4
(
ìì4 5
$str
ìì5 Y
)
ììY Z
;
ììZ [
}
îî 	
if
ññ 

(
ññ 
Status
ññ 
==
ññ 
TicketStatus
ññ "
.
ññ" #
Closed
ññ# )
||
ññ* ,
Status
ññ- 3
==
ññ4 6
TicketStatus
ññ7 C
.
ññC D
Voided
ññD J
||
ññK M
Status
ññN T
==
ññU W
TicketStatus
ññX d
.
ññd e
Refunded
ññe m
)
ññm n
{
óó 	
throw
òò 
new
òò -
DomainInvalidOperationException
òò 5
(
òò5 6
$"
òò6 8
$str
òò8 b
{
òòb c
Status
òòc i
}
òòi j
$str
òòj r
"
òòr s
)
òòs t
;
òòt u
}
ôô 	!
ServiceChargeAmount
õõ 
=
õõ 
amount
õõ $
;
õõ$ %

ActiveDate
úú 
=
úú 
DateTime
úú 
.
úú 
UtcNow
úú $
;
úú$ %
CalculateTotals
ùù 
(
ùù 
)
ùù 
;
ùù 
}
ûû 
public
££ 

void
££ 
SetDeliveryCharge
££ !
(
££! "
Money
££" '
amount
££( .
)
££. /
{
§§ 
if
•• 

(
•• 
amount
•• 
<
•• 
Money
•• 
.
•• 
Zero
•• 
(
••  
)
••  !
)
••! "
{
¶¶ 	
throw
ßß 
new
ßß ,
BusinessRuleViolationException
ßß 4
(
ßß4 5
$str
ßß5 Z
)
ßßZ [
;
ßß[ \
}
®® 	
if
™™ 

(
™™ 
Status
™™ 
==
™™ 
TicketStatus
™™ "
.
™™" #
Closed
™™# )
||
™™* ,
Status
™™- 3
==
™™4 6
TicketStatus
™™7 C
.
™™C D
Voided
™™D J
||
™™K M
Status
™™N T
==
™™U W
TicketStatus
™™X d
.
™™d e
Refunded
™™e m
)
™™m n
{
´´ 	
throw
¨¨ 
new
¨¨ -
DomainInvalidOperationException
¨¨ 5
(
¨¨5 6
$"
¨¨6 8
$str
¨¨8 c
{
¨¨c d
Status
¨¨d j
}
¨¨j k
$str
¨¨k s
"
¨¨s t
)
¨¨t u
;
¨¨u v
}
≠≠ 	"
DeliveryChargeAmount
ØØ 
=
ØØ 
amount
ØØ %
;
ØØ% &

ActiveDate
∞∞ 
=
∞∞ 
DateTime
∞∞ 
.
∞∞ 
UtcNow
∞∞ $
;
∞∞$ %
CalculateTotals
±± 
(
±± 
)
±± 
;
±± 
}
≤≤ 
public
∏∏ 

void
∏∏ 
SetTaxExempt
∏∏ 
(
∏∏ 
bool
∏∏ !
isTaxExempt
∏∏" -
)
∏∏- .
{
ππ 
if
∫∫ 

(
∫∫ 
Status
∫∫ 
==
∫∫ 
TicketStatus
∫∫ "
.
∫∫" #
Closed
∫∫# )
||
∫∫* ,
Status
∫∫- 3
==
∫∫4 6
TicketStatus
∫∫7 C
.
∫∫C D
Voided
∫∫D J
||
∫∫K M
Status
∫∫N T
==
∫∫U W
TicketStatus
∫∫X d
.
∫∫d e
Refunded
∫∫e m
)
∫∫m n
{
ªª 	
throw
ºº 
new
ºº -
DomainInvalidOperationException
ºº 5
(
ºº5 6
$"
ºº6 8
$str
ºº8 e
{
ººe f
Status
ººf l
}
ººl m
$str
ººm u
"
ººu v
)
ººv w
;
ººw x
}
ΩΩ 	
IsTaxExempt
øø 
=
øø 
isTaxExempt
øø !
;
øø! "

ActiveDate
¿¿ 
=
¿¿ 
DateTime
¿¿ 
.
¿¿ 
UtcNow
¿¿ $
;
¿¿$ %
CalculateTotals
¡¡ 
(
¡¡ 
)
¡¡ 
;
¡¡ 
}
¬¬ 
public
«« 

void
«« 
SetNote
«« 
(
«« 
string
«« 
?
«« 
note
««  $
)
««$ %
{
»» 
if
…… 

(
…… 
Status
…… 
==
…… 
TicketStatus
…… "
.
……" #
Closed
……# )
||
……* ,
Status
……- 3
==
……4 6
TicketStatus
……7 C
.
……C D
Voided
……D J
||
……K M
Status
……N T
==
……U W
TicketStatus
……X d
.
……d e
Refunded
……e m
)
……m n
{
   	
throw
ÀÀ 
new
ÀÀ -
DomainInvalidOperationException
ÀÀ 6
(
ÀÀ6 7
$"
ÀÀ7 9
$str
ÀÀ9 Y
{
ÀÀY Z
Status
ÀÀZ `
}
ÀÀ` a
$str
ÀÀa i
"
ÀÀi j
)
ÀÀj k
;
ÀÀk l
}
ÃÃ 	
Note
ŒŒ 
=
ŒŒ 
note
ŒŒ 
;
ŒŒ 

ActiveDate
œœ 
=
œœ 
DateTime
œœ 
.
œœ 
UtcNow
œœ $
;
œœ$ %
IncrementVersion
–– 
(
–– 
)
–– 
;
–– 
}
—— 
public
ÿÿ 

void
ÿÿ 
SetNumberOfGuests
ÿÿ !
(
ÿÿ! "
int
ÿÿ" %
numberOfGuests
ÿÿ& 4
)
ÿÿ4 5
{
ŸŸ 
if
⁄⁄ 

(
⁄⁄ 
numberOfGuests
⁄⁄ 
<
⁄⁄ 
$num
⁄⁄ 
)
⁄⁄ 
{
€€ 	
throw
‹‹ 
new
‹‹ ,
BusinessRuleViolationException
‹‹ 5
(
‹‹5 6
$str
‹‹6 \
)
‹‹\ ]
;
‹‹] ^
}
›› 	
NumberOfGuests
„„ 
=
„„ 
numberOfGuests
„„ '
;
„„' (

ActiveDate
‰‰ 
=
‰‰ 
DateTime
‰‰ 
.
‰‰ 
UtcNow
‰‰ $
;
‰‰$ %
IncrementVersion
ÂÂ 
(
ÂÂ 
)
ÂÂ 
;
ÂÂ 
}
ÊÊ 
public
ÔÔ 

void
ÔÔ 
SetAdjustment
ÔÔ 
(
ÔÔ 
Money
ÔÔ #
amount
ÔÔ$ *
)
ÔÔ* +
{
 
if
ÒÒ 

(
ÒÒ 
amount
ÒÒ 
<
ÒÒ 
Money
ÒÒ 
.
ÒÒ 
Zero
ÒÒ 
(
ÒÒ  
)
ÒÒ  !
)
ÒÒ! "
{
ÚÚ 	
throw
ÛÛ 
new
ÛÛ ,
BusinessRuleViolationException
ÛÛ 4
(
ÛÛ4 5
$strÛÛ5 Ä
)ÛÛÄ Å
;ÛÛÅ Ç
}
ÙÙ 	
if
ˆˆ 

(
ˆˆ 
Status
ˆˆ 
==
ˆˆ 
TicketStatus
ˆˆ "
.
ˆˆ" #
Closed
ˆˆ# )
||
ˆˆ* ,
Status
ˆˆ- 3
==
ˆˆ4 6
TicketStatus
ˆˆ7 C
.
ˆˆC D
Voided
ˆˆD J
||
ˆˆK M
Status
ˆˆN T
==
ˆˆU W
TicketStatus
ˆˆX d
.
ˆˆd e
Refunded
ˆˆe m
)
ˆˆm n
{
˜˜ 	
throw
¯¯ 
new
¯¯ -
DomainInvalidOperationException
¯¯ 5
(
¯¯5 6
$"
¯¯6 8
$str
¯¯8 ^
{
¯¯^ _
Status
¯¯_ e
}
¯¯e f
$str
¯¯f n
"
¯¯n o
)
¯¯o p
;
¯¯p q
}
˘˘ 	
AdjustmentAmount
˚˚ 
=
˚˚ 
amount
˚˚ !
;
˚˚! "

ActiveDate
¸¸ 
=
¸¸ 
DateTime
¸¸ 
.
¸¸ 
UtcNow
¸¸ $
;
¸¸$ %
CalculateTotals
˝˝ 
(
˝˝ 
)
˝˝ 
;
˝˝ 
}
˛˛ 
public
ÉÉ 

void
ÉÉ 
SetAdvancePayment
ÉÉ !
(
ÉÉ! "
Money
ÉÉ" '
amount
ÉÉ( .
)
ÉÉ. /
{
ÑÑ 
if
ÖÖ 

(
ÖÖ 
amount
ÖÖ 
<
ÖÖ 
Money
ÖÖ 
.
ÖÖ 
Zero
ÖÖ 
(
ÖÖ  
)
ÖÖ  !
)
ÖÖ! "
{
ÜÜ 	
throw
áá 
new
áá ,
BusinessRuleViolationException
áá 4
(
áá4 5
$str
áá5 Z
)
ááZ [
;
áá[ \
}
àà 	
if
ää 

(
ää 
Status
ää 
==
ää 
TicketStatus
ää "
.
ää" #
Closed
ää# )
||
ää* ,
Status
ää- 3
==
ää4 6
TicketStatus
ää7 C
.
ääC D
Voided
ääD J
||
ääK M
Status
ääN T
==
ääU W
TicketStatus
ääX d
.
ääd e
Refunded
ääe m
)
ääm n
{
ãã 	
throw
åå 
new
åå -
DomainInvalidOperationException
åå 5
(
åå5 6
$"
åå6 8
$str
åå8 c
{
ååc d
Status
ååd j
}
ååj k
$str
ååk s
"
åås t
)
ååt u
;
ååu v
}
çç 	
AdvanceAmount
èè 
=
èè 
amount
èè 
;
èè 

ActiveDate
êê 
=
êê 
DateTime
êê 
.
êê 
UtcNow
êê $
;
êê$ %
CalculateTotals
ëë 
(
ëë 
)
ëë 
;
ëë 
}
íí 
public
óó 

void
óó 
MarkAsReady
óó 
(
óó 
)
óó 
{
òò 
if
ôô 

(
ôô 
Status
ôô 
==
ôô 
TicketStatus
ôô "
.
ôô" #
Closed
ôô# )
||
ôô* ,
Status
ôô- 3
==
ôô4 6
TicketStatus
ôô7 C
.
ôôC D
Voided
ôôD J
||
ôôK M
Status
ôôN T
==
ôôU W
TicketStatus
ôôX d
.
ôôd e
Refunded
ôôe m
)
ôôm n
{
öö 	
throw
õõ 
new
õõ -
DomainInvalidOperationException
õõ 5
(
õõ5 6
$"
õõ6 8
$str
õõ8 W
{
õõW X
Status
õõX ^
}
õõ^ _
$str
õõ_ g
"
õõg h
)
õõh i
;
õõi j
}
úú 	
	ReadyTime
ûû 
=
ûû 
DateTime
ûû 
.
ûû 
UtcNow
ûû #
;
ûû# $

ActiveDate
üü 
=
üü 
DateTime
üü 
.
üü 
UtcNow
üü $
;
üü$ %
IncrementVersion
†† 
(
†† 
)
†† 
;
†† 
}
°° 
public
¶¶ 

void
¶¶ 
MarkAsDispatched
¶¶  
(
¶¶  !
Guid
¶¶! %
?
¶¶% &
driverId
¶¶' /
)
¶¶/ 0
{
ßß 
if
®® 

(
®® 
Status
®® 
==
®® 
TicketStatus
®® "
.
®®" #
Closed
®®# )
||
®®* ,
Status
®®- 3
==
®®4 6
TicketStatus
®®7 C
.
®®C D
Voided
®®D J
||
®®K M
Status
®®N T
==
®®U W
TicketStatus
®®X d
.
®®d e
Refunded
®®e m
)
®®m n
{
©© 	
throw
™™ 
new
™™ -
DomainInvalidOperationException
™™ 5
(
™™5 6
$"
™™6 8
$str
™™8 \
{
™™\ ]
Status
™™] c
}
™™c d
$str
™™d l
"
™™l m
)
™™m n
;
™™n o
}
´´ 	
if
≠≠ 

(
≠≠  
CustomerWillPickup
≠≠ 
)
≠≠ 
{
ÆÆ 	
throw
ØØ 
new
ØØ -
DomainInvalidOperationException
ØØ 6
(
ØØ6 7
$str
ØØ7 Y
)
ØØY Z
;
ØØZ [
}
∞∞ 	
DispatchedTime
≤≤ 
=
≤≤ 
DateTime
≤≤ !
.
≤≤! "
UtcNow
≤≤" (
;
≤≤( )
AssignedDriverId
≥≥ 
=
≥≥ 
driverId
≥≥ #
;
≥≥# $

ActiveDate
¥¥ 
=
¥¥ 
DateTime
¥¥ 
.
¥¥ 
UtcNow
¥¥ $
;
¥¥$ %
IncrementVersion
µµ 
(
µµ 
)
µµ 
;
µµ 
}
∂∂ 
public
ªª 

void
ªª 
Transfer
ªª 
(
ªª 
UserId
ªª 
newOwner
ªª  (
)
ªª( )
{
ºº 
if
ΩΩ 

(
ΩΩ 
Status
ΩΩ 
==
ΩΩ 
TicketStatus
ΩΩ "
.
ΩΩ" #
Closed
ΩΩ# )
||
ΩΩ* ,
Status
ΩΩ- 3
==
ΩΩ4 6
TicketStatus
ΩΩ7 C
.
ΩΩC D
Voided
ΩΩD J
||
ΩΩK M
Status
ΩΩN T
==
ΩΩU W
TicketStatus
ΩΩX d
.
ΩΩd e
Refunded
ΩΩe m
)
ΩΩm n
{
ææ 	
throw
øø 
new
øø -
DomainInvalidOperationException
øø 5
(
øø5 6
$"
øø6 8
$str
øø8 R
{
øøR S
Status
øøS Y
}
øøY Z
$str
øøZ b
"
øøb c
)
øøc d
;
øød e
}
¿¿ 	
if
¬¬ 

(
¬¬ 
newOwner
¬¬ 
==
¬¬ 
null
¬¬ 
)
¬¬ 
{
√√ 	
throw
ƒƒ 
new
ƒƒ #
ArgumentNullException
ƒƒ +
(
ƒƒ+ ,
nameof
ƒƒ, 2
(
ƒƒ2 3
newOwner
ƒƒ3 ;
)
ƒƒ; <
)
ƒƒ< =
;
ƒƒ= >
}
≈≈ 	
	CreatedBy
«« 
=
«« 
newOwner
«« 
;
«« 

ActiveDate
»» 
=
»» 
DateTime
»» 
.
»» 
UtcNow
»» $
;
»»$ %
IncrementVersion
…… 
(
…… 
)
…… 
;
…… 
}
   
private
““ 
void
““ 
IncrementVersion
““ !
(
““! "
)
““" #
{
”” 
Version
‘‘ 
++
‘‘ 
;
‘‘ 
}
’’ 
}÷÷ Ü
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
}>> ¬g
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
}ππ îb
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
}ƒƒ ÛÄ
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
bool 
IsActive 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
=/ 0
true1 5
;5 6
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
}2 3
public 

int 
Version 
{ 
get 
; 
private %
set& )
;) *
}+ ,
private 
Table 
( 
) 
{   
}!! 
public&& 

static&& 
Table&& 
Create&& 
(&& 
int'' 
tableNumber'' 
,'' 
int(( 
capacity(( 
,(( 
double)) 
x)) 
=)) 
$num)) 
,)) 
double** 
y** 
=** 
$num** 
,** 
Guid++ 
?++ 
floorId++ 
=++ 
null++ 
,++ 
Guid,, 
?,, 
layoutId,, 
=,, 
null,, 
,,, 
bool-- 
isActive-- 
=-- 
true-- 
,-- 
TableShapeType.. 
shape.. 
=.. 
TableShapeType.. -
...- .
	Rectangle... 7
,..7 8
double// 
width// 
=// 
$num// 
,// 
double00 
height00 
=00 
$num00 
)00 
{11 
if22 

(22 
tableNumber22 
<=22 
$num22 
)22 
{33 	
throw44 
new44 

Exceptions44  
.44  !*
BusinessRuleViolationException44! ?
(44? @
$str44@ i
)44i j
;44j k
}55 	
if77 

(77 
capacity77 
<=77 
$num77 
)77 
{88 	
throw99 
new99 

Exceptions99  
.99  !*
BusinessRuleViolationException99! ?
(99? @
$str99@ k
)99k l
;99l m
}:: 	
return<< 
new<< 
Table<< 
{== 	
Id>> 
=>> 
Guid>> 
.>> 
NewGuid>> 
(>> 
)>> 
,>>  
TableNumber?? 
=?? 
tableNumber?? %
,??% &
FloorId@@ 
=@@ 
floorId@@ 
,@@ 
CapacityAA 
=AA 
capacityAA 
,AA  
XBB 
=BB 
xBB 
,BB 
YCC 
=CC 
yCC 
,CC 
WidthDD 
=DD 
widthDD 
,DD 
HeightEE 
=EE 
heightEE 
,EE 
ShapeFF 
=FF 
shapeFF 
,FF 
StatusGG 
=GG 
TableStatusGG  
.GG  !
	AvailableGG! *
,GG* +
CurrentTicketIdHH 
=HH 
nullHH "
,HH" #
LayoutIdII 
=II 
layoutIdII 
,II  
IsActiveJJ 
=JJ 
isActiveJJ 
,JJ  
VersionKK 
=KK 
$numKK 
}LL 	
;LL	 

}MM 
publicRR 

voidRR 
UpdateTableNumberRR !
(RR! "
intRR" %
tableNumberRR& 1
)RR1 2
{SS 
ifTT 

(TT 
tableNumberTT 
<=TT 
$numTT 
)TT 
{UU 	
throwVV 
newVV 

ExceptionsVV  
.VV  !*
BusinessRuleViolationExceptionVV! ?
(VV? @
$strVV@ i
)VVi j
;VVj k
}WW 	
TableNumberYY 
=YY 
tableNumberYY !
;YY! "
}ZZ 
public__ 

void__ 
UpdateCapacity__ 
(__ 
int__ "
capacity__# +
)__+ ,
{`` 
ifaa 

(aa 
capacityaa 
<=aa 
$numaa 
)aa 
{bb 	
throwcc 
newcc 

Exceptionscc  
.cc  !*
BusinessRuleViolationExceptioncc! ?
(cc? @
$strcc@ k
)cck l
;ccl m
}dd 	
Capacityff 
=ff 
capacityff 
;ff 
}gg 
publicll 

voidll 
UpdatePositionll 
(ll 
doublell %
xll& '
,ll' (
doublell) /
yll0 1
)ll1 2
{mm 
Xnn 	
=nn
 
xnn 
;nn 
Yoo 	
=oo
 
yoo 
;oo 
}pp 
publicuu 

voiduu 
UpdateGeometryuu 
(uu 
doubleuu %
xuu& '
,uu' (
doubleuu) /
yuu0 1
,uu1 2
TableShapeTypeuu3 A
shapeuuB G
,uuG H
doubleuuI O
widthuuP U
,uuU V
doubleuuW ]
heightuu^ d
)uud e
{vv 
Xww 	
=ww
 
xww 
;ww 
Yxx 	
=xx
 
yxx 
;xx 
Shapeyy 
=yy 
shapeyy 
;yy 
Widthzz 
=zz 
widthzz 
;zz 
Height{{ 
={{ 
height{{ 
;{{ 
}|| 
public
ÅÅ 

void
ÅÅ 
UpdateFloor
ÅÅ 
(
ÅÅ 
Guid
ÅÅ  
?
ÅÅ  !
floorId
ÅÅ" )
)
ÅÅ) *
{
ÇÇ 
FloorId
ÉÉ 
=
ÉÉ 
floorId
ÉÉ 
;
ÉÉ 
}
ÑÑ 
public
ââ 

void
ââ 
AssignTicket
ââ 
(
ââ 
Guid
ââ !
ticketId
ââ" *
)
ââ* +
{
ää 
if
åå 

(
åå 
CurrentTicketId
åå 
.
åå 
HasValue
åå $
&&
åå% '
CurrentTicketId
åå( 7
.
åå7 8
Value
åå8 =
==
åå> @
ticketId
ååA I
)
ååI J
{
çç 	
return
éé 
;
éé 
}
èè 	
if
ëë 

(
ëë 
Status
ëë 
!=
ëë 
TableStatus
ëë !
.
ëë! "
	Available
ëë" +
&&
ëë, .
Status
ëë/ 5
!=
ëë6 8
TableStatus
ëë9 D
.
ëëD E
Booked
ëëE K
)
ëëK L
{
íí 	
throw
ìì 
new
ìì 

Exceptions
ìì  
.
ìì  !'
InvalidOperationException
ìì! :
(
ìì: ;
$"
ìì; =
$str
ìì= g
{
ììg h
Status
ììh n
}
ììn o
$str
ììo p
"
ììp q
)
ììq r
;
ììr s
}
îî 	
if
ññ 

(
ññ 
CurrentTicketId
ññ 
.
ññ 
HasValue
ññ $
&&
ññ% '
CurrentTicketId
ññ( 7
.
ññ7 8
Value
ññ8 =
!=
ññ> @
ticketId
ññA I
)
ññI J
{
óó 	
throw
òò 
new
òò 

Exceptions
òò  
.
òò  !'
InvalidOperationException
òò! :
(
òò: ;
$str
òò; b
)
òòb c
;
òòc d
}
ôô 	
CurrentTicketId
õõ 
=
õõ 
ticketId
õõ "
;
õõ" #
Status
úú 
=
úú 
TableStatus
úú 
.
úú 
Seat
úú !
;
úú! "
}
ùù 
public
¢¢ 

void
¢¢ 
ReleaseTicket
¢¢ 
(
¢¢ 
)
¢¢ 
{
££ 
if
§§ 

(
§§ 
!
§§ 
CurrentTicketId
§§ 
.
§§ 
HasValue
§§ %
)
§§% &
{
•• 	
throw
¶¶ 
new
¶¶ 

Exceptions
¶¶  
.
¶¶  !'
InvalidOperationException
¶¶! :
(
¶¶: ;
$str
¶¶; d
)
¶¶d e
;
¶¶e f
}
ßß 	
CurrentTicketId
©© 
=
©© 
null
©© 
;
©© 
Status
™™ 
=
™™ 
TableStatus
™™ 
.
™™ 
	Available
™™ &
;
™™& '
}
´´ 
public
∞∞ 

void
∞∞ 
Book
∞∞ 
(
∞∞ 
)
∞∞ 
{
±± 
if
≤≤ 

(
≤≤ 
Status
≤≤ 
!=
≤≤ 
TableStatus
≤≤ !
.
≤≤! "
	Available
≤≤" +
)
≤≤+ ,
{
≥≥ 	
throw
¥¥ 
new
¥¥ 

Exceptions
¥¥  
.
¥¥  !'
InvalidOperationException
¥¥! :
(
¥¥: ;
$"
¥¥; =
$str
¥¥= [
{
¥¥[ \
Status
¥¥\ b
}
¥¥b c
$str
¥¥c d
"
¥¥d e
)
¥¥e f
;
¥¥f g
}
µµ 	
Status
∑∑ 
=
∑∑ 
TableStatus
∑∑ 
.
∑∑ 
Booked
∑∑ #
;
∑∑# $
}
∏∏ 
public
ΩΩ 

void
ΩΩ 
	MarkDirty
ΩΩ 
(
ΩΩ 
)
ΩΩ 
{
ææ 
if
øø 

(
øø 
Status
øø 
!=
øø 
TableStatus
øø !
.
øø! "
	Available
øø" +
)
øø+ ,
{
¿¿ 	
throw
¡¡ 
new
¡¡ 

Exceptions
¡¡  
.
¡¡  !'
InvalidOperationException
¡¡! :
(
¡¡: ;
$"
¡¡; =
$str
¡¡= d
{
¡¡d e
Status
¡¡e k
}
¡¡k l
$str
¡¡l m
"
¡¡m n
)
¡¡n o
;
¡¡o p
}
¬¬ 	
Status
ƒƒ 
=
ƒƒ 
TableStatus
ƒƒ 
.
ƒƒ 
Dirty
ƒƒ "
;
ƒƒ" #
}
≈≈ 
public
   

void
   
	MarkClean
   
(
   
)
   
{
ÀÀ 
if
ÃÃ 

(
ÃÃ 
Status
ÃÃ 
!=
ÃÃ 
TableStatus
ÃÃ !
.
ÃÃ! "
Dirty
ÃÃ" '
)
ÃÃ' (
{
ÕÕ 	
throw
ŒŒ 
new
ŒŒ 

Exceptions
ŒŒ  
.
ŒŒ  !'
InvalidOperationException
ŒŒ! :
(
ŒŒ: ;
$"
ŒŒ; =
$str
ŒŒ= d
{
ŒŒd e
Status
ŒŒe k
}
ŒŒk l
$str
ŒŒl m
"
ŒŒm n
)
ŒŒn o
;
ŒŒo p
}
œœ 	
Status
—— 
=
—— 
TableStatus
—— 
.
—— 
	Available
—— &
;
——& '
}
““ 
public
◊◊ 

void
◊◊ 
Disable
◊◊ 
(
◊◊ 
)
◊◊ 
{
ÿÿ 
if
ŸŸ 

(
ŸŸ 
Status
ŸŸ 
==
ŸŸ 
TableStatus
ŸŸ !
.
ŸŸ! "
Seat
ŸŸ" &
)
ŸŸ& '
{
⁄⁄ 	
throw
€€ 
new
€€ 

Exceptions
€€  
.
€€  !'
InvalidOperationException
€€! :
(
€€: ;
$str
€€; e
)
€€e f
;
€€f g
}
‹‹ 	
Status
ﬁﬁ 
=
ﬁﬁ 
TableStatus
ﬁﬁ 
.
ﬁﬁ 
Disable
ﬁﬁ $
;
ﬁﬁ$ %
}
ﬂﬂ 
public
‰‰ 

void
‰‰ 
Enable
‰‰ 
(
‰‰ 
)
‰‰ 
{
ÂÂ 
if
ÊÊ 

(
ÊÊ 
Status
ÊÊ 
!=
ÊÊ 
TableStatus
ÊÊ !
.
ÊÊ! "
Disable
ÊÊ" )
)
ÊÊ) *
{
ÁÁ 	
throw
ËË 
new
ËË 

Exceptions
ËË  
.
ËË  !'
InvalidOperationException
ËË! :
(
ËË: ;
$"
ËË; =
$str
ËË= ]
{
ËË] ^
Status
ËË^ d
}
ËËd e
$str
ËËe f
"
ËËf g
)
ËËg h
;
ËËh i
}
ÈÈ 	
Status
ÎÎ 
=
ÎÎ 
TableStatus
ÎÎ 
.
ÎÎ 
	Available
ÎÎ &
;
ÎÎ& '
}
ÏÏ 
public
ÒÒ 

void
ÒÒ 
Activate
ÒÒ 
(
ÒÒ 
)
ÒÒ 
{
ÚÚ 
IsActive
ÛÛ 
=
ÛÛ 
true
ÛÛ 
;
ÛÛ 
}
ÙÙ 
public
˘˘ 

void
˘˘ 

Deactivate
˘˘ 
(
˘˘ 
)
˘˘ 
{
˙˙ 
if
˚˚ 

(
˚˚ 
Status
˚˚ 
==
˚˚ 
TableStatus
˚˚ !
.
˚˚! "
Seat
˚˚" &
)
˚˚& '
{
¸¸ 	
throw
˝˝ 
new
˝˝ 

Exceptions
˝˝  
.
˝˝  !'
InvalidOperationException
˝˝! :
(
˝˝: ;
$str
˝˝; h
)
˝˝h i
;
˝˝i j
}
˛˛ 	
IsActive
ÄÄ 
=
ÄÄ 
false
ÄÄ 
;
ÄÄ 
}
ÅÅ 
public
ÜÜ 

bool
ÜÜ 
IsAvailable
ÜÜ 
(
ÜÜ 
)
ÜÜ 
{
áá 
return
àà 
IsActive
àà 
&&
àà 
Status
àà !
==
àà" $
TableStatus
àà% 0
.
àà0 1
	Available
àà1 :
;
àà: ;
}
ââ 
}ää Ò1
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
}ÎÎ ≤
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
}ss §&
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
}$$ Ê†
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
publicBB 

voidBB 

SetBatchIdBB 
(BB 
GuidBB 
batchIdBB  '
)BB' (
{CC 
BatchIdDD 
=DD 
batchIdDD 
;DD 
}EE 
	protectedGG 
PaymentGG 
(GG 
)GG 
{HH 
AmountII 
=II 
MoneyII 
.II 
ZeroII 
(II 
)II 
;II 

TipsAmountJJ 
=JJ 
MoneyJJ 
.JJ 
ZeroJJ 
(JJ  
)JJ  !
;JJ! "
TipsExceedAmountKK 
=KK 
MoneyKK  
.KK  !
ZeroKK! %
(KK% &
)KK& '
;KK' (
TenderAmountLL 
=LL 
MoneyLL 
.LL 
ZeroLL !
(LL! "
)LL" #
;LL# $
ChangeAmountMM 
=MM 
MoneyMM 
.MM 
ZeroMM !
(MM! "
)MM" #
;MM# $
}NN 
	protectedPP 
PaymentPP 
(PP 
GuidQQ 
ticketIdQQ 
,QQ 
PaymentTypeRR 
paymentTypeRR 
,RR  
MoneySS 
amountSS 
,SS 
UserIdTT 
processedByTT 
,TT 
GuidUU 

terminalIdUU 
,UU 
stringVV 
?VV 
globalIdVV 
=VV 
nullVV 
)VV  
{WW 
IdXX 

=XX 
GuidXX 
.XX 
NewGuidXX 
(XX 
)XX 
;XX 
GlobalIdYY 
=YY 
globalIdYY 
;YY 
TicketIdZZ 
=ZZ 
ticketIdZZ 
;ZZ 
PaymentType[[ 
=[[ 
paymentType[[ !
;[[! "
TransactionType\\ 
=\\ 
TransactionType\\ )
.\\) *
Credit\\* 0
;\\0 1
Amount]] 
=]] 
amount]] 
;]] 

TipsAmount^^ 
=^^ 
Money^^ 
.^^ 
Zero^^ 
(^^  
)^^  !
;^^! "
TipsExceedAmount__ 
=__ 
Money__  
.__  !
Zero__! %
(__% &
)__& '
;__' (
TenderAmount`` 
=`` 
Money`` 
.`` 
Zero`` !
(``! "
)``" #
;``# $
ChangeAmountaa 
=aa 
Moneyaa 
.aa 
Zeroaa !
(aa! "
)aa" #
;aa# $
TransactionTimebb 
=bb 
DateTimebb "
.bb" #
UtcNowbb# )
;bb) *
ProcessedBycc 
=cc 
processedBycc !
;cc! "

TerminalIddd 
=dd 

terminalIddd 
;dd  

IsCapturedee 
=ee 
falseee 
;ee 
IsVoidedff 
=ff 
falseff 
;ff 
IsAuthorizablegg 
=gg 
falsegg 
;gg 
}hh 
publicnn 

staticnn 
Paymentnn 
Createnn  
(nn  !
Guidoo 
ticketIdoo 
,oo 
PaymentTypepp 
paymentTypepp 
,pp  
Moneyqq 
amountqq 
,qq 
UserIdrr 
processedByrr 
,rr 
Guidss 

terminalIdss 
,ss 
stringtt 
?tt 
globalIdtt 
=tt 
nulltt 
)tt  
{uu 
ifvv 

(vv 
paymentTypevv 
!=vv 
PaymentTypevv &
.vv& '
Cashvv' +
)vv+ ,
{ww 	
throwxx 
newxx %
InvalidOperationExceptionxx /
(xx/ 0
$"yy 
$stryy ?
{yy? @
paymentTypeyy@ K
}yyK L
$stryyL N
"yyN O
+yyP Q
$"zz 
$strzz 5
"zz5 6
)zz6 7
;zz7 8
}{{ 	
return}} 
CashPayment}} 
.}} 
Create}} !
(}}! "
ticketId}}" *
,}}* +
amount}}, 2
,}}2 3
processedBy}}4 ?
,}}? @

terminalId}}A K
,}}K L
globalId}}M U
)}}U V
;}}V W
}~~ 
public
ÄÄ 

void
ÄÄ 
Void
ÄÄ 
(
ÄÄ 
)
ÄÄ 
{
ÅÅ 
if
ÇÇ 

(
ÇÇ 
IsVoided
ÇÇ 
)
ÇÇ 
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
ÑÑ; W
)
ÑÑW X
;
ÑÑX Y
}
ÖÖ 	
IsVoided
áá 
=
áá 
true
áá 
;
áá 
}
àà 
public
ää 

void
ää 
Capture
ää 
(
ää 
)
ää 
{
ãã 
if
åå 

(
åå 
!
åå 
IsAuthorizable
åå 
)
åå 
{
çç 	
throw
éé 
new
éé 

Exceptions
éé  
.
éé  !'
InvalidOperationException
éé! :
(
éé: ;
$str
éé; k
)
éék l
;
éél m
}
èè 	
if
ëë 

(
ëë 

IsCaptured
ëë 
)
ëë 
{
íí 	
throw
ìì 
new
ìì 

Exceptions
ìì  
.
ìì  !'
InvalidOperationException
ìì! :
(
ìì: ;
$str
ìì; Y
)
ììY Z
;
ììZ [
}
îî 	

IsCaptured
ññ 
=
ññ 
true
ññ 
;
ññ 
}
óó 
public
úú 

virtual
úú 
void
úú 
AddTips
úú 
(
úú  
Money
úú  %

tipsAmount
úú& 0
)
úú0 1
{
ùù 
if
ûû 

(
ûû 

tipsAmount
ûû 
<
ûû 
Money
ûû 
.
ûû 
Zero
ûû #
(
ûû# $
)
ûû$ %
)
ûû% &
{
üü 	
throw
†† 
new
†† 

Exceptions
††  
.
††  !,
BusinessRuleViolationException
††! ?
(
††? @
$str
††@ a
)
††a b
;
††b c
}
°° 	
if
££ 

(
££ 
IsVoided
££ 
)
££ 
{
§§ 	
throw
•• 
new
•• 

Exceptions
••  
.
••  !'
InvalidOperationException
••! :
(
••: ;
$str
••; a
)
••a b
;
••b c
}
¶¶ 	
SetTipsAmount
®® 
(
®® 

tipsAmount
®®  
)
®®  !
;
®®! "
}
©© 
public
ØØ 

static
ØØ 
Payment
ØØ 
CreateRefund
ØØ &
(
ØØ& '
Payment
∞∞ 
originalPayment
∞∞ 
,
∞∞  
Money
±± 
refundAmount
±± 
,
±± 
UserId
≤≤ 
processedBy
≤≤ 
,
≤≤ 
Guid
≥≥ 

terminalId
≥≥ 
,
≥≥ 
string
¥¥ 
?
¥¥ 
reason
¥¥ 
=
¥¥ 
null
¥¥ 
,
¥¥ 
string
µµ 
?
µµ 
globalId
µµ 
=
µµ 
null
µµ 
)
µµ  
{
∂∂ 
if
∑∑ 

(
∑∑ 
originalPayment
∑∑ 
==
∑∑ 
null
∑∑ #
)
∑∑# $
{
∏∏ 	
throw
ππ 
new
ππ #
ArgumentNullException
ππ +
(
ππ+ ,
nameof
ππ, 2
(
ππ2 3
originalPayment
ππ3 B
)
ππB C
)
ππC D
;
ππD E
}
∫∫ 	
if
ºº 

(
ºº 
refundAmount
ºº 
<=
ºº 
Money
ºº !
.
ºº! "
Zero
ºº" &
(
ºº& '
)
ºº' (
)
ºº( )
{
ΩΩ 	
throw
ææ 
new
ææ 

Exceptions
ææ  
.
ææ  !,
BusinessRuleViolationException
ææ! ?
(
ææ? @
$str
ææ@ j
)
ææj k
;
ææk l
}
øø 	
if
¡¡ 

(
¡¡ 
refundAmount
¡¡ 
>
¡¡ 
originalPayment
¡¡ *
.
¡¡* +
Amount
¡¡+ 1
)
¡¡1 2
{
¬¬ 	
throw
√√ 
new
√√ 

Exceptions
√√  
.
√√  !,
BusinessRuleViolationException
√√! ?
(
√√? @
$"
ƒƒ 
$str
ƒƒ !
{
ƒƒ! "
refundAmount
ƒƒ" .
}
ƒƒ. /
$str
ƒƒ/ X
{
ƒƒX Y
originalPayment
ƒƒY h
.
ƒƒh i
Amount
ƒƒi o
}
ƒƒo p
$str
ƒƒp r
"
ƒƒr s
)
ƒƒs t
;
ƒƒt u
}
≈≈ 	
if
«« 

(
«« 
originalPayment
«« 
.
«« 
IsVoided
«« $
)
««$ %
{
»» 	
throw
…… 
new
…… 

Exceptions
……  
.
……  !'
InvalidOperationException
……! :
(
……: ;
$str
……; \
)
……\ ]
;
……] ^
}
   	
Payment
ŒŒ 
refundPayment
ŒŒ 
=
ŒŒ 
originalPayment
ŒŒ  /
.
ŒŒ/ 0
PaymentType
ŒŒ0 ;
switch
ŒŒ< B
{
œœ 	
PaymentType
–– 
.
–– 
Cash
–– 
=>
–– 
CashPayment
––  +
.
––+ ,
Create
––, 2
(
––2 3
originalPayment
—— 
.
——  
TicketId
——  (
,
——( )
refundAmount
““ 
,
““ 
processedBy
”” 
,
”” 

terminalId
‘‘ 
,
‘‘ 
globalId
’’ 
)
’’ 
,
’’ 
PaymentType
÷÷ 
.
÷÷ 

CreditCard
÷÷ "
=>
÷÷# %
CreditCardPayment
÷÷& 7
.
÷÷7 8
Create
÷÷8 >
(
÷÷> ?
originalPayment
◊◊ 
.
◊◊  
TicketId
◊◊  (
,
◊◊( )
refundAmount
ÿÿ 
,
ÿÿ 
processedBy
ŸŸ 
,
ŸŸ 

terminalId
⁄⁄ 
,
⁄⁄ 
globalId
€€ 
:
€€ 
globalId
€€ "
)
€€" #
,
€€# $
PaymentType
‹‹ 
.
‹‹ 
	DebitCard
‹‹ !
=>
‹‹" $
DebitCardPayment
‹‹% 5
.
‹‹5 6
Create
‹‹6 <
(
‹‹< =
originalPayment
›› 
.
››  
TicketId
››  (
,
››( )
refundAmount
ﬁﬁ 
,
ﬁﬁ 
processedBy
ﬂﬂ 
,
ﬂﬂ 

terminalId
‡‡ 
,
‡‡ 
globalId
·· 
:
·· 
globalId
·· "
)
··" #
,
··# $
PaymentType
‚‚ 
.
‚‚ 
GiftCertificate
‚‚ '
=>
‚‚( *
originalPayment
„„ 
is
„„  "$
GiftCertificatePayment
„„# 9
	gcPayment
„„: C
?
‰‰ $
GiftCertificatePayment
‰‰ ,
.
‰‰, -
Create
‰‰- 3
(
‰‰3 4
originalPayment
ÂÂ '
.
ÂÂ' (
TicketId
ÂÂ( 0
,
ÂÂ0 1
refundAmount
ÊÊ $
,
ÊÊ$ %
processedBy
ÁÁ #
,
ÁÁ# $

terminalId
ËË "
,
ËË" #
	gcPayment
ÈÈ !
.
ÈÈ! "#
GiftCertificateNumber
ÈÈ" 7
,
ÈÈ7 8
	gcPayment
ÍÍ !
.
ÍÍ! "
OriginalAmount
ÍÍ" 0
,
ÍÍ0 1
	gcPayment
ÎÎ !
.
ÎÎ! "
RemainingBalance
ÎÎ" 2
+
ÎÎ3 4
refundAmount
ÎÎ5 A
,
ÎÎA B
globalId
ÏÏ  
)
ÏÏ  !
:
ÌÌ 
throw
ÌÌ 
new
ÌÌ 

Exceptions
ÌÌ  *
.
ÌÌ* +'
InvalidOperationException
ÌÌ+ D
(
ÌÌD E
$strÌÌE ä
)ÌÌä ã
,ÌÌã å
PaymentType
ÓÓ 
.
ÓÓ 
CustomPayment
ÓÓ %
=>
ÓÓ& (
originalPayment
ÔÔ 
is
ÔÔ  "
CustomPayment
ÔÔ# 0
customPayment
ÔÔ1 >
?
 
CustomPayment
 #
.
# $
Create
$ *
(
* +
originalPayment
ÒÒ '
.
ÒÒ' (
TicketId
ÒÒ( 0
,
ÒÒ0 1
refundAmount
ÚÚ $
,
ÚÚ$ %
processedBy
ÛÛ #
,
ÛÛ# $

terminalId
ÙÙ "
,
ÙÙ" #
customPayment
ıı %
.
ıı% &
PaymentName
ıı& 1
,
ıı1 2
null
ˆˆ 
,
ˆˆ 
null
˜˜ 
,
˜˜ 
globalId
¯¯  
)
¯¯  !
:
˘˘ 
throw
˘˘ 
new
˘˘ 

Exceptions
˘˘  *
.
˘˘* +'
InvalidOperationException
˘˘+ D
(
˘˘D E
$str
˘˘E |
)
˘˘| }
,
˘˘} ~
_
˙˙ 
=>
˙˙ 
throw
˙˙ 
new
˙˙ 

Exceptions
˙˙ %
.
˙˙% &'
InvalidOperationException
˙˙& ?
(
˙˙? @
$"
˙˙@ B
$str
˙˙B h
{
˙˙h i
originalPayment
˙˙i x
.
˙˙x y
PaymentType˙˙y Ñ
}˙˙Ñ Ö
$str˙˙Ö Ü
"˙˙Ü á
)˙˙á à
}
˚˚ 	
;
˚˚	 

var
˛˛ %
transactionTypeProperty
˛˛ #
=
˛˛$ %
typeof
˛˛& ,
(
˛˛, -
Payment
˛˛- 4
)
˛˛4 5
.
˛˛5 6
GetProperty
˛˛6 A
(
˛˛A B
$str
˛˛B S
,
˛˛S T
System
ˇˇ 
.
ˇˇ 

Reflection
ˇˇ 
.
ˇˇ 
BindingFlags
ˇˇ *
.
ˇˇ* +
Instance
ˇˇ+ 3
|
ˇˇ4 5
System
ˇˇ6 <
.
ˇˇ< =

Reflection
ˇˇ= G
.
ˇˇG H
BindingFlags
ˇˇH T
.
ˇˇT U
Public
ˇˇU [
|
ˇˇ\ ]
System
ˇˇ^ d
.
ˇˇd e

Reflection
ˇˇe o
.
ˇˇo p
BindingFlags
ˇˇp |
.
ˇˇ| }
	NonPublicˇˇ} Ü
)ˇˇÜ á
;ˇˇá à%
transactionTypeProperty
ÄÄ 
?
ÄÄ  
.
ÄÄ  !
SetValue
ÄÄ! )
(
ÄÄ) *
refundPayment
ÄÄ* 7
,
ÄÄ7 8
TransactionType
ÄÄ9 H
.
ÄÄH I
Debit
ÄÄI N
)
ÄÄN O
;
ÄÄO P
var
ÉÉ 
noteProperty
ÉÉ 
=
ÉÉ 
typeof
ÉÉ !
(
ÉÉ! "
Payment
ÉÉ" )
)
ÉÉ) *
.
ÉÉ* +
GetProperty
ÉÉ+ 6
(
ÉÉ6 7
$str
ÉÉ7 =
,
ÉÉ= >
System
ÑÑ 
.
ÑÑ 

Reflection
ÑÑ 
.
ÑÑ 
BindingFlags
ÑÑ *
.
ÑÑ* +
Instance
ÑÑ+ 3
|
ÑÑ4 5
System
ÑÑ6 <
.
ÑÑ< =

Reflection
ÑÑ= G
.
ÑÑG H
BindingFlags
ÑÑH T
.
ÑÑT U
Public
ÑÑU [
|
ÑÑ\ ]
System
ÑÑ^ d
.
ÑÑd e

Reflection
ÑÑe o
.
ÑÑo p
BindingFlags
ÑÑp |
.
ÑÑ| }
	NonPublicÑÑ} Ü
)ÑÑÜ á
;ÑÑá à
noteProperty
ÖÖ 
?
ÖÖ 
.
ÖÖ 
SetValue
ÖÖ 
(
ÖÖ 
refundPayment
ÖÖ ,
,
ÖÖ, -
$"
ÖÖ. 0
$str
ÖÖ0 B
{
ÖÖB C
originalPayment
ÖÖC R
.
ÖÖR S
Id
ÖÖS U
}
ÖÖU V
$str
ÖÖV `
{
ÖÖ` a
reason
ÖÖa g
??
ÖÖh j
$str
ÖÖk p
}
ÖÖp q
"
ÖÖq r
)
ÖÖr s
;
ÖÖs t
return
áá 
refundPayment
áá 
;
áá 
}
àà 
}ââ ≥B
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
}44 Ëº
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
}??2 3
privateBB 
	OrderLineBB 
(BB 
)BB 
{CC 
	UnitPriceDD 
=DD 
MoneyDD 
.DD 
ZeroDD 
(DD 
)DD  
;DD  !
SubtotalAmountEE 
=EE 
MoneyEE 
.EE 
ZeroEE #
(EE# $
)EE$ %
;EE% &*
SubtotalAmountWithoutModifiersFF &
=FF' (
MoneyFF) .
.FF. /
ZeroFF/ 3
(FF3 4
)FF4 5
;FF5 6
DiscountAmountGG 
=GG 
MoneyGG 
.GG 
ZeroGG #
(GG# $
)GG$ %
;GG% &
	TaxAmountHH 
=HH 
MoneyHH 
.HH 
ZeroHH 
(HH 
)HH  
;HH  !%
TaxAmountWithoutModifiersII !
=II" #
MoneyII$ )
.II) *
ZeroII* .
(II. /
)II/ 0
;II0 1
TotalAmountJJ 
=JJ 
MoneyJJ 
.JJ 
ZeroJJ  
(JJ  !
)JJ! "
;JJ" #'
TotalAmountWithoutModifiersKK #
=KK$ %
MoneyKK& +
.KK+ ,
ZeroKK, 0
(KK0 1
)KK1 2
;KK2 3
}LL 
publicQQ 

staticQQ 
	OrderLineQQ 
CreateQQ "
(QQ" #
GuidRR 
ticketIdRR 
,RR 
GuidSS 

menuItemIdSS 
,SS 
stringTT 
menuItemNameTT 
,TT 
decimalUU 
quantityUU 
,UU 
MoneyVV 
	unitPriceVV 
,VV 
decimalWW 
taxRateWW 
=WW 
$numWW 
,WW 
stringXX 
?XX 
categoryNameXX 
=XX 
nullXX #
,XX# $
stringYY 
?YY 
	groupNameYY 
=YY 
nullYY  
)YY  !
{ZZ 
if[[ 

([[ 
quantity[[ 
<=[[ 
$num[[ 
)[[ 
{\\ 	
throw]] 
new]] *
BusinessRuleViolationException]] 4
(]]4 5
$str]]5 Z
)]]Z [
;]][ \
}^^ 	
if`` 

(`` 
	unitPrice`` 
<`` 
Money`` 
.`` 
Zero`` "
(``" #
)``# $
)``$ %
{aa 	
throwbb 
newbb *
BusinessRuleViolationExceptionbb 4
(bb4 5
$strbb5 U
)bbU V
;bbV W
}cc 	
varee 
	orderLineee 
=ee 
newee 
	OrderLineee %
{ff 	
Idgg 
=gg 
Guidgg 
.gg 
NewGuidgg 
(gg 
)gg 
,gg  
TicketIdhh 
=hh 
ticketIdhh 
,hh  

MenuItemIdii 
=ii 

menuItemIdii #
,ii# $
MenuItemNamejj 
=jj 
menuItemNamejj '
,jj' (
CategoryNamekk 
=kk 
categoryNamekk '
,kk' (
	GroupNamell 
=ll 
	groupNamell !
,ll! "
Quantitymm 
=mm 
quantitymm 
,mm  
	ItemCountnn 
=nn 
(nn 
intnn 
)nn 
Mathnn !
.nn! "
Ceilingnn" )
(nn) *
quantitynn* 2
)nn2 3
,nn3 4
	UnitPriceoo 
=oo 
	unitPriceoo !
,oo! "
TaxRatepp 
=pp 
taxRatepp 
,pp 
	CreatedAtqq 
=qq 
DateTimeqq  
.qq  !
UtcNowqq! '
}rr 	
;rr	 

	orderLinett 
.tt 
CalculatePricett  
(tt  !
)tt! "
;tt" #
returnuu 
	orderLineuu 
;uu 
}vv 
public{{ 

void{{ 
CalculatePrice{{ 
({{ 
){{  
{|| *
SubtotalAmountWithoutModifiers~~ &
=~~' (
	UnitPrice~~) 2
*~~3 4
Quantity~~5 =
;~~= >
var
ÅÅ 
modifierTotal
ÅÅ 
=
ÅÅ 

_modifiers
ÅÅ &
.
ÅÅ& '
	Aggregate
ÅÅ' 0
(
ÅÅ0 1
Money
ÅÅ1 6
.
ÅÅ6 7
Zero
ÅÅ7 ;
(
ÅÅ; <
)
ÅÅ< =
,
ÅÅ= >
(
ÅÅ? @
sum
ÅÅ@ C
,
ÅÅC D
m
ÅÅE F
)
ÅÅF G
=>
ÅÅH J
sum
ÅÅK N
+
ÅÅO P
m
ÅÅQ R
.
ÅÅR S
TotalAmount
ÅÅS ^
)
ÅÅ^ _
;
ÅÅ_ `
var
ÑÑ 
sizeModifierTotal
ÑÑ 
=
ÑÑ 
SizeModifier
ÑÑ  ,
?
ÑÑ, -
.
ÑÑ- .
TotalAmount
ÑÑ. 9
??
ÑÑ: <
Money
ÑÑ= B
.
ÑÑB C
Zero
ÑÑC G
(
ÑÑG H
)
ÑÑH I
;
ÑÑI J
SubtotalAmount
ÜÜ 
=
ÜÜ ,
SubtotalAmountWithoutModifiers
ÜÜ 7
+
ÜÜ8 9
modifierTotal
ÜÜ: G
+
ÜÜH I
sizeModifierTotal
ÜÜJ [
;
ÜÜ[ \'
TaxAmountWithoutModifiers
ââ !
=
ââ" #,
SubtotalAmountWithoutModifiers
ââ$ B
*
ââC D
TaxRate
ââE L
;
ââL M
var
ää 
modifierTax
ää 
=
ää 

_modifiers
ää $
.
ää$ %
	Aggregate
ää% .
(
ää. /
Money
ää/ 4
.
ää4 5
Zero
ää5 9
(
ää9 :
)
ää: ;
,
ää; <
(
ää= >
sum
ää> A
,
ääA B
m
ääC D
)
ääD E
=>
ääF H
sum
ääI L
+
ääM N
m
ääO P
.
ääP Q
	TaxAmount
ääQ Z
)
ääZ [
;
ää[ \
var
ãã 
sizeModifierTax
ãã 
=
ãã 
SizeModifier
ãã *
?
ãã* +
.
ãã+ ,
	TaxAmount
ãã, 5
??
ãã6 8
Money
ãã9 >
.
ãã> ?
Zero
ãã? C
(
ããC D
)
ããD E
;
ããE F
	TaxAmount
çç 
=
çç '
TaxAmountWithoutModifiers
çç -
+
çç. /
modifierTax
çç0 ;
+
çç< =
sizeModifierTax
çç> M
;
ççM N
DiscountAmount
êê 
=
êê 

_discounts
êê #
.
êê# $
	Aggregate
êê$ -
(
êê- .
Money
êê. 3
.
êê3 4
Zero
êê4 8
(
êê8 9
)
êê9 :
,
êê: ;
(
êê< =
sum
êê= @
,
êê@ A
d
êêB C
)
êêC D
=>
êêE G
sum
êêH K
+
êêL M
d
êêN O
.
êêO P
Amount
êêP V
)
êêV W
;
êêW X
if
íí 

(
íí 
DiscountAmount
íí 
>
íí 
SubtotalAmount
íí +
)
íí+ ,
{
ìì 	
DiscountAmount
îî 
=
îî 
SubtotalAmount
îî +
;
îî+ ,
}
ïï 	)
TotalAmountWithoutModifiers
òò #
=
òò$ %,
SubtotalAmountWithoutModifiers
òò& D
+
òòE F'
TaxAmountWithoutModifiers
òòG `
-
òòa b
DiscountAmount
òòc q
;
òòq r
TotalAmount
ôô 
=
ôô 
SubtotalAmount
ôô $
+
ôô% &
	TaxAmount
ôô' 0
-
ôô1 2
DiscountAmount
ôô3 A
;
ôôA B
if
úú 

(
úú 
TotalAmount
úú 
<
úú 
Money
úú 
.
úú  
Zero
úú  $
(
úú$ %
)
úú% &
)
úú& '
{
ùù 	
TotalAmount
ûû 
=
ûû 
Money
ûû 
.
ûû  
Zero
ûû  $
(
ûû$ %
)
ûû% &
;
ûû& '
}
üü 	
}
†† 
public
•• 

void
•• 
UpdateQuantity
•• 
(
•• 
decimal
•• &
quantity
••' /
)
••/ 0
{
¶¶ 
if
ßß 

(
ßß 
quantity
ßß 
<=
ßß 
$num
ßß 
)
ßß 
{
®® 	
throw
©© 
new
©© ,
BusinessRuleViolationException
©© 4
(
©©4 5
$str
©©5 Z
)
©©Z [
;
©©[ \
}
™™ 	
Quantity
¨¨ 
=
¨¨ 
quantity
¨¨ 
;
¨¨ 
	ItemCount
≠≠ 
=
≠≠ 
(
≠≠ 
int
≠≠ 
)
≠≠ 
Math
≠≠ 
.
≠≠ 
Ceiling
≠≠ %
(
≠≠% &
quantity
≠≠& .
)
≠≠. /
;
≠≠/ 0
CalculatePrice
ÆÆ 
(
ÆÆ 
)
ÆÆ 
;
ÆÆ 
}
ØØ 
public
¥¥ 

bool
¥¥ 
CanMerge
¥¥ 
(
¥¥ 
	OrderLine
¥¥ "
other
¥¥# (
)
¥¥( )
{
µµ 
if
∂∂ 

(
∂∂ 
other
∂∂ 
==
∂∂ 
null
∂∂ 
)
∂∂ 
return
∂∂ !
false
∂∂" '
;
∂∂' (
if
∑∑ 

(
∑∑ 

MenuItemId
∑∑ 
!=
∑∑ 
other
∑∑ 
.
∑∑  

MenuItemId
∑∑  *
)
∑∑* +
return
∑∑, 2
false
∑∑3 8
;
∑∑8 9
if
∏∏ 

(
∏∏ 
	UnitPrice
∏∏ 
!=
∏∏ 
other
∏∏ 
.
∏∏ 
	UnitPrice
∏∏ (
)
∏∏( )
return
∏∏* 0
false
∏∏1 6
;
∏∏6 7
if
ππ 

(
ππ 
TaxRate
ππ 
!=
ππ 
other
ππ 
.
ππ 
TaxRate
ππ $
)
ππ$ %
return
ππ& ,
false
ππ- 2
;
ππ2 3
if
∫∫ 

(
∫∫ 

_modifiers
∫∫ 
.
∫∫ 
Count
∫∫ 
!=
∫∫ 
other
∫∫  %
.
∫∫% &

_modifiers
∫∫& 0
.
∫∫0 1
Count
∫∫1 6
)
∫∫6 7
return
∫∫8 >
false
∫∫? D
;
∫∫D E
return
¡¡ 
true
¡¡ 
;
¡¡ 
}
¬¬ 
public
«« 

void
«« 
Merge
«« 
(
«« 
	OrderLine
«« 
other
««  %
)
««% &
{
»» 
if
…… 

(
…… 
!
…… 
CanMerge
…… 
(
…… 
other
…… 
)
…… 
)
…… 
{
   	
throw
ÀÀ 
new
ÀÀ ,
BusinessRuleViolationException
ÀÀ 4
(
ÀÀ4 5
$str
ÀÀ5 h
)
ÀÀh i
;
ÀÀi j
}
ÃÃ 	
Quantity
ŒŒ 
+=
ŒŒ 
other
ŒŒ 
.
ŒŒ 
Quantity
ŒŒ "
;
ŒŒ" #
	ItemCount
œœ 
+=
œœ 
other
œœ 
.
œœ 
	ItemCount
œœ $
;
œœ$ %
CalculatePrice
–– 
(
–– 
)
–– 
;
–– 
}
—— 
public
÷÷ 

void
÷÷ 
AddModifier
÷÷ 
(
÷÷ 
OrderLineModifier
÷÷ -
modifier
÷÷. 6
)
÷÷6 7
{
◊◊ 
if
ÿÿ 

(
ÿÿ 
modifier
ÿÿ 
==
ÿÿ 
null
ÿÿ 
)
ÿÿ 
throw
ÿÿ #
new
ÿÿ$ '#
ArgumentNullException
ÿÿ( =
(
ÿÿ= >
nameof
ÿÿ> D
(
ÿÿD E
modifier
ÿÿE M
)
ÿÿM N
)
ÿÿN O
;
ÿÿO P

_modifiers
⁄⁄ 
.
⁄⁄ 
Add
⁄⁄ 
(
⁄⁄ 
modifier
⁄⁄ 
)
⁄⁄  
;
⁄⁄  !
CalculatePrice
€€ 
(
€€ 
)
€€ 
;
€€ 
}
‹‹ 
public
·· 

void
·· 
RemoveModifier
·· 
(
·· 
OrderLineModifier
·· 0
modifier
··1 9
)
··9 :
{
‚‚ 
if
„„	 
(
„„ 
modifier
„„ 
==
„„ 
null
„„ 
)
„„ 
throw
„„ $
new
„„% (#
ArgumentNullException
„„) >
(
„„> ?
nameof
„„? E
(
„„E F
modifier
„„F N
)
„„N O
)
„„O P
;
„„P Q

_modifiers
ÂÂ 
.
ÂÂ 
Remove
ÂÂ 
(
ÂÂ 
modifier
ÂÂ "
)
ÂÂ" #
;
ÂÂ# $
CalculatePrice
ÊÊ 
(
ÊÊ 
)
ÊÊ 
;
ÊÊ 
}
ÁÁ 
public
ÏÏ 

void
ÏÏ 
ApplyDiscount
ÏÏ 
(
ÏÏ 
OrderLineDiscount
ÏÏ /
discount
ÏÏ0 8
)
ÏÏ8 9
{
ÌÌ 
if
ÓÓ 

(
ÓÓ 
discount
ÓÓ 
==
ÓÓ 
null
ÓÓ 
)
ÓÓ 
throw
ÓÓ #
new
ÓÓ$ '#
ArgumentNullException
ÓÓ( =
(
ÓÓ= >
nameof
ÓÓ> D
(
ÓÓD E
discount
ÓÓE M
)
ÓÓM N
)
ÓÓN O
;
ÓÓO P
if
 

(
 
discount
 
.
 
OrderLineId
  
!=
! #
Id
$ &
)
& '
throw
ÒÒ 
new
ÒÒ ,
BusinessRuleViolationException
ÒÒ 4
(
ÒÒ4 5
$str
ÒÒ5 c
)
ÒÒc d
;
ÒÒd e

_discounts
ÛÛ 
.
ÛÛ 
Add
ÛÛ 
(
ÛÛ 
discount
ÛÛ 
)
ÛÛ  
;
ÛÛ  !
CalculatePrice
ÙÙ 
(
ÙÙ 
)
ÙÙ 
;
ÙÙ 
}
ıı 
public
˙˙ 

void
˙˙ "
MarkPrintedToKitchen
˙˙ $
(
˙˙$ %
)
˙˙% &
{
˚˚ 
if
¸¸ 

(
¸¸ 
!
¸¸ "
ShouldPrintToKitchen
¸¸ !
)
¸¸! "
{
˝˝ 	
throw
˛˛ 
new
˛˛ ,
BusinessRuleViolationException
˛˛ 4
(
˛˛4 5
$str
˛˛5 h
)
˛˛h i
;
˛˛i j
}
ˇˇ 	
PrintedToKitchen
ÅÅ 
=
ÅÅ 
true
ÅÅ 
;
ÅÅ  
foreach
ÑÑ 
(
ÑÑ 
var
ÑÑ 
modifier
ÑÑ 
in
ÑÑ  

_modifiers
ÑÑ! +
.
ÑÑ+ ,
Where
ÑÑ, 1
(
ÑÑ1 2
m
ÑÑ2 3
=>
ÑÑ4 6
m
ÑÑ7 8
.
ÑÑ8 9"
ShouldPrintToKitchen
ÑÑ9 M
)
ÑÑM N
)
ÑÑN O
{
ÖÖ 	
modifier
ÜÜ 
.
ÜÜ "
MarkPrintedToKitchen
ÜÜ )
(
ÜÜ) *
)
ÜÜ* +
;
ÜÜ+ ,
}
áá 	
}
àà 
public
çç 

void
çç 
SetPrinterGroup
çç 
(
çç  
Guid
çç  $
?
çç$ %
printerGroupId
çç& 4
)
çç4 5
{
éé 
PrinterGroupId
èè 
=
èè 
printerGroupId
èè '
;
èè' ("
ShouldPrintToKitchen
êê 
=
êê 
printerGroupId
êê -
.
êê- .
HasValue
êê. 6
;
êê6 7
}
ëë 
public
ññ 

void
ññ 
SetInstructions
ññ 
(
ññ  
string
ññ  &
?
ññ& '
instructions
ññ( 4
)
ññ4 5
{
óó 
Instructions
òò 
=
òò 
instructions
òò #
;
òò# $
}
ôô 
public
ûû 

void
ûû 
UpdateModifiers
ûû 
(
ûû  
IEnumerable
ûû  +
<
ûû+ ,
OrderLineModifier
ûû, =
>
ûû= >
newModifiers
ûû? K
)
ûûK L
{
üü 

_modifiers
†† 
.
†† 
Clear
†† 
(
†† 
)
†† 
;
†† 
foreach
¢¢ 
(
¢¢ 
var
¢¢ 
modifier
¢¢ 
in
¢¢  
newModifiers
¢¢! -
)
¢¢- .
{
££ 	
AddModifier
§§ 
(
§§ 
modifier
§§  
)
§§  !
;
§§! "
}
•• 	
CalculatePrice
ßß 
(
ßß 
)
ßß 
;
ßß 
}
®® 
public
¨¨ 

void
¨¨ 
SetSeatNumber
¨¨ 
(
¨¨ 
int
¨¨ !
?
¨¨! "

seatNumber
¨¨# -
)
¨¨- .
{
≠≠ 
if
ÆÆ 

(
ÆÆ 

seatNumber
ÆÆ 
.
ÆÆ 
HasValue
ÆÆ 
&&
ÆÆ  "

seatNumber
ÆÆ# -
.
ÆÆ- .
Value
ÆÆ. 3
<
ÆÆ4 5
$num
ÆÆ6 7
)
ÆÆ7 8
{
ØØ 	
throw
∞∞ 
new
∞∞ ,
BusinessRuleViolationException
∞∞ 5
(
∞∞5 6
$str
∞∞6 W
)
∞∞W X
;
∞∞X Y
}
±± 	

SeatNumber
≤≤ 
=
≤≤ 

seatNumber
≤≤ 
;
≤≤  
}
≥≥ 
}¥¥ äQ
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
}+ ,
private 
readonly 
List 
< 
MenuModifier &
>& '

_modifiers( 2
=3 4
new5 8
(8 9
)9 :
;: ;
public 

IReadOnlyCollection 
< 
MenuModifier +
>+ ,
	Modifiers- 6
=>7 9

_modifiers: D
.D E

AsReadOnlyE O
(O P
)P Q
;Q R
private 
ModifierGroup 
( 
) 
{ 
Name 
= 
string 
. 
Empty 
; 
} 
public## 

static## 
ModifierGroup## 
Create##  &
(##& '
string$$ 
name$$ 
,$$ 
bool%% 

isRequired%% 
=%% 
false%% 
,%%  
int&& 
minSelections&& 
=&& 
$num&& 
,&& 
int'' 
maxSelections'' 
='' 
$num'' 
,'' 
bool(( #
allowMultipleSelections(( $
=((% &
false((' ,
,((, -
string)) 
?)) 
description)) 
=)) 
null)) "
,))" #
int** 
displayOrder** 
=** 
$num** 
,** 
bool++ 
isActive++ 
=++ 
true++ 
)++ 
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
new// 

Exceptions//  
.//  !*
BusinessRuleViolationException//! ?
(//? @
$str//@ f
)//f g
;//g h
}00 	
if22 

(22 
minSelections22 
<22 
$num22 
)22 
{33 	
throw44 
new44 

Exceptions44  
.44  !*
BusinessRuleViolationException44! ?
(44? @
$str44@ h
)44h i
;44i j
}55 	
if77 

(77 
maxSelections77 
<77 
minSelections77 )
)77) *
{88 	
throw99 
new99 

Exceptions99  
.99  !*
BusinessRuleViolationException99! ?
(99? @
$str99@ |
)99| }
;99} ~
}:: 	
if<< 

(<< 
maxSelections<< 
><< 
$num<< 
&&<<  
!<<! "#
allowMultipleSelections<<" 9
)<<9 :
{== 	
throw>> 
new>> 

Exceptions>>  
.>>  !*
BusinessRuleViolationException>>! ?
(>>? @
$str	>>@ å
)
>>å ç
;
>>ç é
}?? 	
returnAA 
newAA 
ModifierGroupAA  
{BB 	
IdCC 
=CC 
GuidCC 
.CC 
NewGuidCC 
(CC 
)CC 
,CC  
NameDD 
=DD 
nameDD 
,DD 
DescriptionEE 
=EE 
descriptionEE %
,EE% &

IsRequiredFF 
=FF 

isRequiredFF #
,FF# $
MinSelectionsGG 
=GG 
minSelectionsGG )
,GG) *
MaxSelectionsHH 
=HH 
maxSelectionsHH )
,HH) *#
AllowMultipleSelectionsII #
=II$ %#
allowMultipleSelectionsII& =
,II= >
DisplayOrderJJ 
=JJ 
displayOrderJJ '
,JJ' (
IsActiveKK 
=KK 
isActiveKK 
,KK  
VersionLL 
=LL 
$numLL 
}MM 	
;MM	 

}NN 
publicSS 

voidSS 

UpdateNameSS 
(SS 
stringSS !
nameSS" &
)SS& '
{TT 
ifUU 

(UU 
stringUU 
.UU 
IsNullOrWhiteSpaceUU %
(UU% &
nameUU& *
)UU* +
)UU+ ,
{VV 	
throwWW 
newWW 

ExceptionsWW  
.WW  !*
BusinessRuleViolationExceptionWW! ?
(WW? @
$strWW@ f
)WWf g
;WWg h
}XX 	
NameZZ 
=ZZ 
nameZZ 
;ZZ 
}[[ 
public`` 

void`` 
UpdateDescription`` !
(``! "
string``" (
?``( )
description``* 5
)``5 6
{aa 
Descriptionbb 
=bb 
descriptionbb !
;bb! "
}cc 
publichh 

voidhh 
SetIsRequiredhh 
(hh 
boolhh "

isRequiredhh# -
)hh- .
{ii 

IsRequiredjj 
=jj 

isRequiredjj 
;jj  
}kk 
publicpp 

voidpp &
UpdateSelectionConstraintspp *
(pp* +
intpp+ .
minSelectionspp/ <
,pp< =
intpp> A
maxSelectionsppB O
,ppO P
boolppQ U#
allowMultipleSelectionsppV m
)ppm n
{qq 
ifrr 

(rr 
minSelectionsrr 
<rr 
$numrr 
)rr 
{ss 	
throwtt 
newtt 

Exceptionstt  
.tt  !*
BusinessRuleViolationExceptiontt! ?
(tt? @
$strtt@ h
)tth i
;tti j
}uu 	
ifww 

(ww 
maxSelectionsww 
<ww 
minSelectionsww )
)ww) *
{xx 	
throwyy 
newyy 

Exceptionsyy  
.yy  !*
BusinessRuleViolationExceptionyy! ?
(yy? @
$stryy@ |
)yy| }
;yy} ~
}zz 	
if|| 

(|| 
maxSelections|| 
>|| 
$num|| 
&&||  
!||! "#
allowMultipleSelections||" 9
)||9 :
{}} 	
throw~~ 
new~~ 

Exceptions~~  
.~~  !*
BusinessRuleViolationException~~! ?
(~~? @
$str	~~@ å
)
~~å ç
;
~~ç é
} 	
MinSelections
ÅÅ 
=
ÅÅ 
minSelections
ÅÅ %
;
ÅÅ% &
MaxSelections
ÇÇ 
=
ÇÇ 
maxSelections
ÇÇ %
;
ÇÇ% &%
AllowMultipleSelections
ÉÉ 
=
ÉÉ  !%
allowMultipleSelections
ÉÉ" 9
;
ÉÉ9 :
}
ÑÑ 
public
ââ 

void
ââ  
UpdateDisplayOrder
ââ "
(
ââ" #
int
ââ# &
displayOrder
ââ' 3
)
ââ3 4
{
ää 
DisplayOrder
ãã 
=
ãã 
displayOrder
ãã #
;
ãã# $
}
åå 
public
ëë 

void
ëë 
Activate
ëë 
(
ëë 
)
ëë 
{
íí 
IsActive
ìì 
=
ìì 
true
ìì 
;
ìì 
}
îî 
public
ôô 

void
ôô 

Deactivate
ôô 
(
ôô 
)
ôô 
{
öö 
IsActive
õõ 
=
õõ 
false
õõ 
;
õõ 
}
úú 
public
°° 

bool
°° #
IsValidSelectionCount
°° %
(
°°% &
int
°°& )
selectionCount
°°* 8
)
°°8 9
{
¢¢ 
if
££ 

(
££ 

IsRequired
££ 
&&
££ 
selectionCount
££ (
<
££) *
MinSelections
££+ 8
)
££8 9
{
§§ 	
return
•• 
false
•• 
;
•• 
}
¶¶ 	
if
®® 

(
®® 
selectionCount
®® 
<
®® 
MinSelections
®® *
||
®®+ -
selectionCount
®®. <
>
®®= >
MaxSelections
®®? L
)
®®L M
{
©© 	
return
™™ 
false
™™ 
;
™™ 
}
´´ 	
return
≠≠ 
true
≠≠ 
;
≠≠ 
}
ÆÆ 
}ØØ Ê4
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
}%% ˛_
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
publicrr 

voidrr 
SetCategoryrr 
(rr 
Guidrr  

categoryIdrr! +
)rr+ ,
{ss 
iftt 

(tt 

categoryIdtt 
==tt 
Guidtt 
.tt 
Emptytt $
)tt$ %
throwtt& +
newtt, /
ArgumentExceptiontt0 A
(ttA B
$strttB W
)ttW X
;ttX Y

CategoryIduu 
=uu 

categoryIduu 
;uu  
}vv 
publicxx 

voidxx 
SetGroupxx 
(xx 
Guidxx 
groupIdxx %
)xx% &
{yy 
ifzz 

(zz 
groupIdzz 
==zz 
Guidzz 
.zz 
Emptyzz !
)zz! "
throwzz# (
newzz) ,
ArgumentExceptionzz- >
(zz> ?
$strzz? Q
)zzQ R
;zzR S
GroupId{{ 
={{ 
groupId{{ 
;{{ 
}|| 
public~~ 

void~~ 
SetPrinterGroup~~ 
(~~  
Guid~~  $
?~~$ %
printerGroupId~~& 4
)~~4 5
{ 
PrinterGroupId
ÄÄ 
=
ÄÄ 
printerGroupId
ÄÄ '
;
ÄÄ' (
}
ÅÅ 
public
ÉÉ 

void
ÉÉ  
SetComboDefinition
ÉÉ "
(
ÉÉ" #
Guid
ÉÉ# '
?
ÉÉ' (
comboDefinitionId
ÉÉ) :
)
ÉÉ: ;
{
ÑÑ 
ComboDefinitionId
ÖÖ 
=
ÖÖ 
comboDefinitionId
ÖÖ -
;
ÖÖ- .
}
ÜÜ 
public
àà 

void
àà 
AddModifierGroup
àà  
(
àà  !
ModifierGroup
àà! .
group
àà/ 4
,
àà4 5
int
àà6 9
displayOrder
àà: F
=
ààG H
$num
ààI J
)
ààJ K
{
ââ 
}
éé 
}èè ∑2
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
}aa £,
nC:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\Magidesk.Domain\Entities\MenuCategory.cs
	namespace 	
Magidesk
 
. 
Domain 
. 
Entities "
;" #
public		 
class		 
MenuCategory		 
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

int 
	SortOrder 
{ 
get 
; 
private  '
set( +
;+ ,
}- .
public 

bool 
	IsVisible 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 

bool 

IsBeverage 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
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
}- .
private 
MenuCategory 
( 
) 
{ 
Name 
= 
string 
. 
Empty 
; 
} 
public 

Guid 
? 
PrinterGroupId 
{  !
get" %
;% &
private' .
set/ 2
;2 3
}4 5
public 

static 
MenuCategory 
Create %
(% &
string 
name 
, 
int 
	sortOrder 
= 
$num 
, 
bool 

isBeverage 
= 
false 
)  
{ 
if   

(   
string   
.   
IsNullOrWhiteSpace   %
(  % &
name  & *
)  * +
)  + ,
throw!! 
new!! 

Exceptions!!  
.!!  !*
BusinessRuleViolationException!!! ?
(!!? @
$str!!@ `
)!!` a
;!!a b
return## 
new## 
MenuCategory## 
{$$ 	
Id%% 
=%% 
Guid%% 
.%% 
NewGuid%% 
(%% 
)%% 
,%%  
Name&& 
=&& 
name&& 
.&& 
Trim&& 
(&& 
)&& 
,&& 
	SortOrder'' 
='' 
	sortOrder'' !
,''! "
	IsVisible(( 
=(( 
true(( 
,(( 

IsBeverage)) 
=)) 

isBeverage)) #
,))# $
IsActive** 
=** 
true** 
}++ 	
;++	 

},, 
public.. 

void.. 

UpdateName.. 
(.. 
string.. !
name.." &
)..& '
{// 
if00 

(00 
string00 
.00 
IsNullOrWhiteSpace00 %
(00% &
name00& *
)00* +
)00+ ,
throw11 
new11 

Exceptions11  
.11  !*
BusinessRuleViolationException11! ?
(11? @
$str11@ `
)11` a
;11a b
Name33 
=33 
name33 
.33 
Trim33 
(33 
)33 
;33 
}44 
public66 

void66 
UpdateSortOrder66 
(66  
int66  #
	sortOrder66$ -
)66- .
{77 
	SortOrder88 
=88 
	sortOrder88 
;88 
}99 
public;; 

void;; 
SetVisibility;; 
(;; 
bool;; "
	isVisible;;# ,
);;, -
{<< 
	IsVisible== 
=== 
	isVisible== 
;== 
}>> 
public@@ 

void@@ 
SetBeverageFlag@@ 
(@@  
bool@@  $

isBeverage@@% /
)@@/ 0
{AA 

IsBeverageBB 
=BB 

isBeverageBB 
;BB  
}CC 
publicEE 

voidEE 
SetButtonColorEE 
(EE 
stringEE %
?EE% &
colorEE' ,
)EE, -
{FF 
ButtonColorGG 
=GG 
colorGG 
;GG 
}HH 
publicJJ 

voidJJ 

DeactivateJJ 
(JJ 
)JJ 
{KK 
IsActiveLL 
=LL 
falseLL 
;LL 
}MM 
publicOO 

voidOO 
ActivateOO 
(OO 
)OO 
{PP 
IsActiveQQ 
=QQ 
trueQQ 
;QQ 
}RR 
publicTT 

voidTT 
SetPrinterGroupTT 
(TT  
GuidTT  $
?TT$ %
printerGroupIdTT& 4
)TT4 5
{UU 
PrinterGroupIdVV 
=VV 
printerGroupIdVV '
;VV' (
}WW 
}XX ø
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
}:: ê*
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
)  
: 	
base
 
( 
ticketId 
, 
PaymentType $
.$ %
GiftCertificate% 4
,4 5
amount6 <
,< =
processedBy> I
,I J

terminalIdK U
,U V
globalIdW _
)_ `
{ 
if   

(   
string   
.   
IsNullOrWhiteSpace   %
(  % &!
giftCertificateNumber  & ;
)  ; <
)  < =
{!! 	
throw"" 
new"" 
ArgumentException"" '
(""' (
$str""( Z
,""Z [
nameof""\ b
(""b c!
giftCertificateNumber""c x
)""x y
)""y z
;""z {
}## 	!
GiftCertificateNumber%% 
=%% !
giftCertificateNumber%%  5
;%%5 6
OriginalAmount&& 
=&& 
originalAmount&& '
;&&' (
RemainingBalance'' 
='' 
remainingBalance'' +
;''+ ,
IsAuthorizable(( 
=(( 
false(( 
;(( 
})) 
public.. 

static.. "
GiftCertificatePayment.. (
Create..) /
(../ 0
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
string33 !
giftCertificateNumber33 $
,33$ %
Money44 
originalAmount44 
,44 
Money55 
remainingBalance55 
,55 
string66 
?66 
globalId66 
=66 
null66 
)66  
{77 
if88 

(88 
amount88 
>88 
remainingBalance88 %
)88% &
{99 	
throw:: 
new:: 

Exceptions::  
.::  !*
BusinessRuleViolationException::! ?
(::? @
$";; 
$str;; "
{;;" #
amount;;# )
};;) *
$str;;* G
{;;G H
remainingBalance;;H X
};;X Y
$str;;Y [
";;[ \
);;\ ]
;;;] ^
}<< 	
return>> 
new>> "
GiftCertificatePayment>> )
(>>) *
ticketId?? 
,?? 
amount@@ 
,@@ 
processedByAA 
,AA 

terminalIdBB 
,BB !
giftCertificateNumberCC !
,CC! "
originalAmountDD 
,DD 
remainingBalanceEE 
,EE 
globalIdFF 
)FF 
;FF 
}GG 
publicLL 

voidLL "
UpdateRemainingBalanceLL &
(LL& '
MoneyLL' ,

newBalanceLL- 7
)LL7 8
{MM 
ifNN 

(NN 

newBalanceNN 
<NN 
MoneyNN 
.NN 
ZeroNN #
(NN# $
)NN$ %
)NN% &
{OO 	
throwPP 
newPP 

ExceptionsPP  
.PP  !*
BusinessRuleViolationExceptionPP! ?
(PP? @
$strPP@ g
)PPg h
;PPh i
}QQ 	
RemainingBalanceSS 
=SS 

newBalanceSS %
;SS% &
}TT 
}UU Â
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
}¬¬ ∂
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
},, Ô-
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
}8 9
private 
Discount 
( 
) 
{ 
} 
public   

static   
Discount   
Create   !
(  ! "
string!! 
name!! 
,!! 
DiscountType"" 
type"" 
,"" 
decimal## 
value## 
,## 
QualificationType$$ 
qualificationType$$ +
,$$+ ,
ApplicationType%% 
applicationType%% '
,%%' (
Money&& 
?&& 

minimumBuy&& 
=&& 
null&&  
,&&  !
int'' 
?'' 
minimumQuantity'' 
='' 
null'' #
,''# $
bool(( 
	autoApply(( 
=(( 
false(( 
,(( 
string)) 
?)) 

couponCode)) 
=)) 
null)) !
,))! "
DateTime** 
?** 
expirationDate**  
=**! "
null**# '
)**' (
{++ 
if,, 

(,, 
string,, 
.,, 
IsNullOrWhiteSpace,, %
(,,% &
name,,& *
),,* +
),,+ ,
{-- 	
throw.. 
new.. 
ArgumentException.. '
(..' (
$str..( P
,..P Q
nameof..R X
(..X Y
name..Y ]
)..] ^
)..^ _
;.._ `
}// 	
if11 

(11 
value11 
<11 
$num11 
)11 
{22 	
throw33 
new33 

Exceptions33  
.33  !*
BusinessRuleViolationException33! ?
(33? @
$str33@ d
)33d e
;33e f
}44 	
return66 
new66 
Discount66 
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
,99 
Type:: 
=:: 
type:: 
,:: 
Value;; 
=;; 
value;; 
,;; 

MinimumBuy<< 
=<< 

minimumBuy<< #
,<<# $
MinimumQuantity== 
=== 
minimumQuantity== -
,==- .
QualificationType>> 
=>> 
qualificationType>>  1
,>>1 2
ApplicationType?? 
=?? 
applicationType?? -
,??- .
	AutoApply@@ 
=@@ 
	autoApply@@ !
,@@! "

CouponCodeAA 
=AA 

couponCodeAA #
,AA# $
ExpirationDateBB 
=BB 
expirationDateBB +
,BB+ ,
IsActiveCC 
=CC 
trueCC 
}DD 	
;DD	 

}EE 
publicGG 

voidGG 

DeactivateGG 
(GG 
)GG 
{HH 
IsActiveII 
=II 
falseII 
;II 
}JJ 
publicLL 

voidLL 
ActivateLL 
(LL 
)LL 
{MM 
IsActiveNN 
=NN 
trueNN 
;NN 
}OO 
}PP ı?
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
)##  
:$$ 	
base$$
 
($$ 
ticketId$$ 
,$$ 
PaymentType$$ $
.$$$ %
	DebitCard$$% .
,$$. /
amount$$0 6
,$$6 7
processedBy$$8 C
,$$C D

terminalId$$E O
,$$O P
globalId$$Q Y
)$$Y Z
{%% 

CardNumber&& 
=&& 

cardNumber&& 
;&&  
CardHolderName'' 
='' 
cardHolderName'' '
;''' (
AuthorizationCode(( 
=(( 
authorizationCode(( -
;((- .
ReferenceNumber)) 
=)) 
referenceNumber)) )
;))) *
CardType** 
=** 
cardType** 
;** 
	PinNumber++ 
=++ 
	pinNumber++ 
;++ 
IsAuthorizable,, 
=,, 
true,, 
;,, 
}-- 
public22 

static22 
DebitCardPayment22 "
Create22# )
(22) *
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
?<< 
	pinNumber<< 
=<< 
null<<  
,<<  !
string== 
?== 
globalId== 
=== 
null== 
)==  
{>> 
return?? 
new?? 
DebitCardPayment?? #
(??# $
ticketId@@ 
,@@ 
amountAA 
,AA 
processedByBB 
,BB 

terminalIdCC 
,CC 

cardNumberDD 
,DD 
cardHolderNameEE 
,EE 
authorizationCodeFF 
,FF 
referenceNumberGG 
,GG 
cardTypeHH 
,HH 
	pinNumberII 
,II 
globalIdJJ 
)JJ 
;JJ 
}KK 
publicPP 

voidPP 
	AuthorizePP 
(PP 
stringPP  
authorizationCodePP! 2
,PP2 3
stringPP4 :
?PP: ;
referenceNumberPP< K
=PPL M
nullPPN R
)PPR S
{QQ 
ifRR 

(RR 
IsVoidedRR 
)RR 
{SS 	
throwTT 
newTT 

ExceptionsTT  
.TT  !%
InvalidOperationExceptionTT! :
(TT: ;
$strTT; _
)TT_ `
;TT` a
}UU 	
ifWW 

(WW 

IsCapturedWW 
)WW 
{XX 	
throwYY 
newYY 

ExceptionsYY  
.YY  !%
InvalidOperationExceptionYY! :
(YY: ;
$strYY; Y
)YYY Z
;YYZ [
}ZZ 	
AuthorizationCode\\ 
=\\ 
authorizationCode\\ -
;\\- .
ReferenceNumber]] 
=]] 
referenceNumber]] )
;]]) *
AuthorizationTime^^ 
=^^ 
DateTime^^ $
.^^$ %
UtcNow^^% +
;^^+ ,
IsAuthorizable__ 
=__ 
true__ 
;__ 
}`` 
publicee 

newee 
voidee 
Captureee 
(ee 
)ee 
{ff 
ifgg 

(gg 
!gg 
IsAuthorizablegg 
)gg 
{hh 	
throwii 
newii 

Exceptionsii  
.ii  !%
InvalidOperationExceptionii! :
(ii: ;
$strii; k
)iik l
;iil m
}jj 	
ifll 

(ll 

IsCapturedll 
)ll 
{mm 	
thrownn 
newnn 

Exceptionsnn  
.nn  !%
InvalidOperationExceptionnn! :
(nn: ;
$strnn; Y
)nnY Z
;nnZ [
}oo 	
ifqq 

(qq 
stringqq 
.qq 
IsNullOrEmptyqq  
(qq  !
AuthorizationCodeqq! 2
)qq2 3
)qq3 4
{rr 	
throwss 
newss 

Exceptionsss  
.ss  !%
InvalidOperationExceptionss! :
(ss: ;
$strss; g
)ssg h
;ssh i
}tt 	

IsCapturedvv 
=vv 
truevv 
;vv 
CaptureTimeww 
=ww 
DateTimeww 
.ww 
UtcNowww %
;ww% &
}xx 
}{{ ﬂ,
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
)  
: 	
base
 
( 
ticketId 
, 
PaymentType $
.$ %
CustomPayment% 2
,2 3
amount4 :
,: ;
processedBy< G
,G H

terminalIdI S
,S T
globalIdU ]
)] ^
{ 
if 

( 
string 
. 
IsNullOrWhiteSpace %
(% &
paymentName& 1
)1 2
)2 3
{ 	
throw   
new   
ArgumentException   '
(  ' (
$str  ( O
,  O P
nameof  Q W
(  W X
paymentName  X c
)  c d
)  d e
;  e f
}!! 	
PaymentName## 
=## 
paymentName## !
;##! "
ReferenceNumber$$ 
=$$ 
referenceNumber$$ )
;$$) *

Properties%% 
=%% 

properties%% 
??%%  "
new%%# &

Dictionary%%' 1
<%%1 2
string%%2 8
,%%8 9
string%%: @
>%%@ A
(%%A B
)%%B C
;%%C D
IsAuthorizable&& 
=&& 
false&& 
;&& 
}'' 
public,, 

static,, 
CustomPayment,, 
Create,,  &
(,,& '
Guid-- 
ticketId-- 
,-- 
Money.. 
amount.. 
,.. 
UserId// 
processedBy// 
,// 
Guid00 

terminalId00 
,00 
string11 
paymentName11 
,11 
string22 
?22 
referenceNumber22 
=22  !
null22" &
,22& '

Dictionary33 
<33 
string33 
,33 
string33 !
>33! "
?33" #

properties33$ .
=33/ 0
null331 5
,335 6
string44 
?44 
globalId44 
=44 
null44 
)44  
{55 
return66 
new66 
CustomPayment66  
(66  !
ticketId77 
,77 
amount88 
,88 
processedBy99 
,99 

terminalId:: 
,:: 
paymentName;; 
,;; 
referenceNumber<< 
,<< 

properties== 
,== 
globalId>> 
)>> 
;>> 
}?? 
publicDD 

voidDD 
SetPropertyDD 
(DD 
stringDD "
keyDD# &
,DD& '
stringDD( .
valueDD/ 4
)DD4 5
{EE 
ifFF 

(FF 
stringFF 
.FF 
IsNullOrWhiteSpaceFF %
(FF% &
keyFF& )
)FF) *
)FF* +
{GG 	
throwHH 
newHH 
ArgumentExceptionHH '
(HH' (
$strHH( O
,HHO P
nameofHHQ W
(HHW X
keyHHX [
)HH[ \
)HH\ ]
;HH] ^
}II 	

PropertiesKK 
[KK 
keyKK 
]KK 
=KK 
valueKK 
;KK  
}LL 
publicQQ 

stringQQ 
?QQ 
GetPropertyQQ 
(QQ 
stringQQ %
keyQQ& )
)QQ) *
{RR 
returnSS 

PropertiesSS 
.SS 
TryGetValueSS %
(SS% &
keySS& )
,SS) *
outSS+ .
varSS/ 2
valueSS3 8
)SS8 9
?SS: ;
valueSS< A
:SSB C
nullSSD H
;SSH I
}TT 
}UU ˘M
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
)!!  
:"" 	
base""
 
("" 
ticketId"" 
,"" 
PaymentType"" $
.""$ %

CreditCard""% /
,""/ 0
amount""1 7
,""7 8
processedBy""9 D
,""D E

terminalId""F P
,""P Q
globalId""R Z
)""Z [
{## 

CardNumber%% 
=%% 
MaskCardNumber%% #
(%%# $

cardNumber%%$ .
)%%. /
;%%/ 0
CardHolderName&& 
=&& 
cardHolderName&& '
;&&' (
AuthorizationCode'' 
='' 
authorizationCode'' -
;''- .
ReferenceNumber(( 
=(( 
referenceNumber(( )
;(() *
CardType)) 
=)) 
cardType)) 
;)) 
IsAuthorizable** 
=** 
true** 
;** 
}++ 
public00 

static00 
CreditCardPayment00 #
Create00$ *
(00* +
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
string55 
?55 

cardNumber55 
=55 
null55 !
,55! "
string66 
?66 
cardHolderName66 
=66  
null66! %
,66% &
string77 
?77 
authorizationCode77 !
=77" #
null77$ (
,77( )
string88 
?88 
referenceNumber88 
=88  !
null88" &
,88& '
string99 
?99 
cardType99 
=99 
null99 
,99  
string:: 
?:: 
globalId:: 
=:: 
null:: 
)::  
{;; 
return<< 
new<< 
CreditCardPayment<< $
(<<$ %
ticketId== 
,== 
amount>> 
,>> 
processedBy?? 
,?? 

terminalId@@ 
,@@ 

cardNumberAA 
,AA 
cardHolderNameBB 
,BB 
authorizationCodeCC 
,CC 
referenceNumberDD 
,DD 
cardTypeEE 
,EE 
globalIdFF 
)FF 
;FF 
}GG 
publicLL 

voidLL 
	AuthorizeLL 
(LL 
stringLL  
authorizationCodeLL! 2
,LL2 3
stringLL4 :
?LL: ;
referenceNumberLL< K
=LLL M
nullLLN R
,LLR S
stringLLT Z
?LLZ [
cardTypeLL\ d
=LLe f
nullLLg k
)LLk l
{MM 
ifNN 

(NN 
IsVoidedNN 
)NN 
{OO 	
throwPP 
newPP 

ExceptionsPP  
.PP  !%
InvalidOperationExceptionPP! :
(PP: ;
$strPP; _
)PP_ `
;PP` a
}QQ 	
ifSS 

(SS 

IsCapturedSS 
)SS 
{TT 	
throwUU 
newUU 

ExceptionsUU  
.UU  !%
InvalidOperationExceptionUU! :
(UU: ;
$strUU; Y
)UUY Z
;UUZ [
}VV 	
AuthorizationCodeXX 
=XX 
authorizationCodeXX -
;XX- .
ifYY 

(YY 
referenceNumberYY 
!=YY 
nullYY #
)YY# $
{ZZ 	
ReferenceNumber[[ 
=[[ 
referenceNumber[[ -
;[[- .
}\\ 	
if]] 

(]] 
cardType]] 
!=]] 
null]] 
)]] 
{^^ 	
CardType__ 
=__ 
cardType__ 
;__  
}`` 	
AuthorizationTimeaa 
=aa 
DateTimeaa $
.aa$ %
UtcNowaa% +
;aa+ ,
IsAuthorizablebb 
=bb 
truebb 
;bb 
}cc 
internalhh 
voidhh !
UpdateReferenceNumberhh '
(hh' (
stringhh( .
referenceNumberhh/ >
)hh> ?
{ii 
ReferenceNumberjj 
=jj 
referenceNumberjj )
;jj) *
}kk 
publicpp 

newpp 
voidpp 
Capturepp 
(pp 
)pp 
{qq 
ifrr 

(rr 
!rr 
IsAuthorizablerr 
)rr 
{ss 	
throwtt 
newtt 

Exceptionstt  
.tt  !%
InvalidOperationExceptiontt! :
(tt: ;
$strtt; k
)ttk l
;ttl m
}uu 	
ifww 

(ww 

IsCapturedww 
)ww 
{xx 	
throwyy 
newyy 

Exceptionsyy  
.yy  !%
InvalidOperationExceptionyy! :
(yy: ;
$stryy; Y
)yyY Z
;yyZ [
}zz 	
if|| 

(|| 
string|| 
.|| 
IsNullOrEmpty||  
(||  !
AuthorizationCode||! 2
)||2 3
)||3 4
{}} 	
throw~~ 
new~~ 

Exceptions~~  
.~~  !%
InvalidOperationException~~! :
(~~: ;
$str~~; g
)~~g h
;~~h i
} 	

IsCaptured
ÅÅ 
=
ÅÅ 
true
ÅÅ 
;
ÅÅ 
CaptureTime
ÇÇ 
=
ÇÇ 
DateTime
ÇÇ 
.
ÇÇ 
UtcNow
ÇÇ %
;
ÇÇ% &
}
ÉÉ 
private
ãã 
static
ãã 
string
ãã 
?
ãã 
MaskCardNumber
ãã )
(
ãã) *
string
ãã* 0
?
ãã0 1

cardNumber
ãã2 <
)
ãã< =
{
åå 
if
çç 

(
çç 
string
çç 
.
çç  
IsNullOrWhiteSpace
çç %
(
çç% &

cardNumber
çç& 0
)
çç0 1
)
çç1 2
return
éé 
null
éé 
;
éé 
if
ëë 

(
ëë 

cardNumber
ëë 
.
ëë 
Contains
ëë 
(
ëë  
$char
ëë  #
)
ëë# $
)
ëë$ %
return
íí 

cardNumber
íí 
;
íí 
if
îî 

(
îî 

cardNumber
îî 
.
îî 
Length
îî 
<=
îî  
$num
îî! "
)
îî" #
return
ïï 

cardNumber
ïï 
;
ïï 
var
óó 
last4
óó 
=
óó 

cardNumber
óó 
.
óó 
	Substring
óó (
(
óó( )

cardNumber
óó) 3
.
óó3 4
Length
óó4 :
-
óó; <
$num
óó= >
)
óó> ?
;
óó? @
return
òò 
new
òò 
string
òò 
(
òò 
$char
òò 
,
òò 

cardNumber
òò )
.
òò) *
Length
òò* 0
-
òò1 2
$num
òò3 4
)
òò4 5
+
òò6 7
last4
òò8 =
;
òò= >
}
ôô 
}öö ˆ
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
}ÙÙ Á
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
)  
: 	
base
 
( 
ticketId 
, 
PaymentType $
.$ %
Cash% )
,) *
amount+ 1
,1 2
processedBy3 >
,> ?

terminalId@ J
,J K
globalIdL T
)T U
{ 
IsAuthorizable 
= 
false 
; 
} 
public 

static 
CashPayment 
Create $
($ %
Guid 
ticketId 
, 
Money 
amount 
, 
UserId   
processedBy   
,   
Guid!! 

terminalId!! 
,!! 
string"" 
?"" 
globalId"" 
="" 
null"" 
)""  
{## 
return$$ 
new$$ 
CashPayment$$ 
($$ 
ticketId$$ '
,$$' (
amount$$) /
,$$/ 0
processedBy$$1 <
,$$< =

terminalId$$> H
,$$H I
globalId$$J R
)$$R S
;$$S T
}%% 
}&& ß
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
}\\ Ω
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
} ≤

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
} «
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