import { Pipe, PipeTransform } from '@angular/core';
@Pipe({ name: 'highlight', standalone: true })
export class HighlightPipe implements PipeTransform {
  transform(text: string, search: string): any {
    if (!search) return text;
    const re = new RegExp(`(${search})`, 'gi');
    return text.replace(re, `<mark>$1</mark>`);
  }
}
