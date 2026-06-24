import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-file-dropzone',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './file-dropzone.component.html',
  styleUrl: './file-dropzone.component.scss'
})
export class FileDropzoneComponent {

  @Input()
  multiple = false;

  @Input()
  accept = '*';

  @Output()
  filesSelected =
    new EventEmitter<File[]>();

  onDragOver(
    event: DragEvent
  ) {

    event.preventDefault();
  }

  onDrop(
    event: DragEvent
  ) {

    event.preventDefault();

    if (!event.dataTransfer?.files) {
      return;
    }

    this.filesSelected.emit(
      Array.from(
        event.dataTransfer.files
      )
    );
  }

  onFileInput(
    event: Event
  ) {

    const input =
      event.target as HTMLInputElement;

    if (!input.files?.length) {
      return;
    }

    this.filesSelected.emit(
      Array.from(input.files)
    );
  }
}