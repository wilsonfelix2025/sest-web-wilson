import { Directive, HostListener } from '@angular/core';

@Directive({
  selector: '[appRightClick]'
})
export class RightClickDirective {

  @HostListener('contextmenu', ['$event'])
  onRightClick(event) {
    event.preventDefault();
    // console.log(event);

  }

  constructor() { }

}
