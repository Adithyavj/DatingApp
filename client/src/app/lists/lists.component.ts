import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { LikeService } from '../_services/like.service';
@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  members: Partial<Member[]>;
  predicate: string = 'liked';

  constructor(private likeService: LikeService) { }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    this.likeService.getLikes(this.predicate).subscribe(response => {
      this.members = response;
    });    
  }

}
