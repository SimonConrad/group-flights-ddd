- debugowanie requestów zdefiniowanych w **_Scenarios**
   - posiłkować się analizą na miro (na miro "Strategiczne DDD")
   - analizować publikowane events
   - analizować zapisy/odczyty do/z postgres-a
  
- podział na subdomeny i bounded-contexty i moduły - analiza na miro, modularny-monolit
- komunikacja między modułami - modelna miro, synchroniczna, asynchroniczna

**dokładne czytanie kodu**
   - wnikać w bardziej skomplikowany składniowo kod - weryfikacja jak gpchat tłumaczy (np mechanizm Specification), odpalić test dla takiego kodu
   - **warstwa dal** analiza: plików konfigurujących mapping encji / agregagtów / readmodeli na relacje w bazie; odczyty, dodawanie, aktualizacje przez repozytoria, dbcontexty per moduł z oddzielnymi schematami
   - **warstwa domain i aplikacyjna** identyfikacja użytych building blocks: taktyczne ddd, cqrs, clean-architecture, ports adapters, crud (serwis + anemiczna encja), warstwy (na miro "Taktyka - Fundamenty modelu)
   - building blocki modelu domenowego:
      - value object
      - encja (anemiczna bądź nie)
      - klucz (naturalny, techniczny)
      - agregat (ma gwarantować granicę zestawu danych które muszą być spójne natychmiast; poprzez centralne miejsce reguł pilnujących ich spójną zmianę, enkapsuluje te dane zapewnijąc ich zmianę tylko przez swoje publiczne api); reguła biznesowa; uważać na zbyt szeroko zakrojoną granicę agregatu
      - reguła biznesowa - problem z reguła biznesową w serwisie, nie ma scentralizowanego miejsca sprawdzenia reguły (jakim jest agregat).
      - polityka
      - serwis domenowy
      - specyfikacja
      - repozytorium
      - fabryka
   - **warstwa aplikacyjna** command-handlers, query-handlers, serwisy, validatory; w zależności od poziomu skomplikowania modelu z miro, 
   - jak crud to serwis plus praca na anemicznej encji i dbctxt bezposrenio w serwisie
   - jak z analizy deep model to clean architecture, cqrs i taktyczne ddd
   - **warstwa web-api** routing, minimal api, api gateway, extensions do dmoularyzacji i kontenera ioc
   - **testy**
   - **shared** - modułowść z analizy na miro moduły, komunikacja przez api i eventy (symulacja event based architecture z messagingiem)
   - **plumbing** - cqrs, eventy, security, modułowość, ioc, migracje, konfiguracja, logger
   - **ops docker**, **ci/cd pipeline**
   - **ADR**

    
- persystencja agregagta
   - mapowanie agregat - relacje (na bazie) (...Configuration)
- analiza jak zaciagane są relacje encji reprezentującej agregagt w repozytoriach i jak są dla nich zrobione konfiguracje mappingu obiekt-tabela
    - include bez virtual chyba, chodzi o zaciągnięcie agregagtu jako całosći
    - jak następuje ich aktulizacja
    - poptrzec tez na podejscia na modularny monolit i solid webapi;
    - w ddd to tabela chage-request i jej relacje
    - alternatywa to josnowa kolumna