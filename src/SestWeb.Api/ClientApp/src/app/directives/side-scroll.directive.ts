import { Directive, OnInit, OnChanges, ElementRef } from '@angular/core';

@Directive({
  selector: '[side-scroll-directive]'
})
export class SideScrollDirective implements OnInit, OnChanges {

  constructor(private el: ElementRef) { }

  ngOnInit() {
    this.contentScrolling(this.el);

  }

  ngOnChanges() {
    this.contentScrolling(this.el);
  }

  contentScrolling(element: ElementRef) {
    setTimeout(() => {
      const el = element.nativeElement;

      el.addEventListener('wheel', function(e) {
        const target = e.target.tagName;
        if (target !== 'rect' && target !== 'path') {
          if (e.deltaY > 0) {
            el.scrollLeft += 150;
          } else {
            el.scrollLeft -= 150;
          }
        }
      });
    }, 100);
  }
}
