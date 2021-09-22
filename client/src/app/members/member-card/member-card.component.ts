import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { LikeService } from 'src/app/_services/like.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  // recieve user details from parent
  @Input() member: Member;
  constructor(private likeService: LikeService, private toastr: ToastrService, 
    public presence: PresenceService) { }

  ngOnInit(): void {

  }

  addLike(member: Member) {
    this.likeService.addLike(member.userName).subscribe(() => {
      this.toastr.success('You have liked ' + member.knownAs);
    });
  }



}
